using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Kprocess.TabTip
{
    public static class TabTip
    {
        const string TabTipWindowClassName = "IPTip_Main_Window";
        const string TabTipExecPath = @"C:\Program Files\Common Files\microsoft shared\ink\TabTip.exe";
        const string TabTipRegistryKeyName = @"HKEY_CURRENT_USER\Software\Microsoft\TabletTip\1.7";

        [DllImport("user32.dll")]
        static extern int SendMessage(int hWnd, uint msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(String sClassName, String sAppName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public readonly int Left;        // x position of upper-left corner
            public readonly int Top;         // y position of upper-left corner
            public readonly int Right;       // x position of lower-right corner
            public readonly int Bottom;      // y position of lower-right corner
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);

        /// <summary>
        /// Signals that TabTip was closed after it was opened 
        /// with a call to StartPoolingForTabTipClosedEvent method
        /// </summary>
        internal static event Action Closed;

        static IntPtr GetTabTipWindowHandle() => FindWindow(TabTipWindowClassName, null);

        internal static void OpenUndockedAndStartPoolingForClosedEvent()
        {
            OpenUndocked();
            StartPoolingForTabTipClosedEvent();
        }

        /// <summary>
        /// Open TabTip
        /// </summary>
        public static void Open()
        {
            if (EnvironmentEx.GetOSVersion() == OSVersion.Win10)
                EnableTabTipOpenInDesctopModeOnWin10();

            Process.Start(TabTipExecPath);
        }

        static void EnableTabTipOpenInDesctopModeOnWin10()
        {
            const string TabTipAutoInvokeKey = "EnableDesktopModeAutoInvoke";

            int EnableDesktopModeAutoInvoke = (int)(Registry.GetValue(TabTipRegistryKeyName, TabTipAutoInvokeKey, -1) ?? -1);
            if (EnableDesktopModeAutoInvoke != 1)
                Registry.SetValue(TabTipRegistryKeyName, TabTipAutoInvokeKey, 1);
        }

        /// <summary>
        /// Open TabTip in undocked state
        /// </summary>
        public static void OpenUndocked()
        {
            const string TabTipDockedKey = "EdgeTargetDockedState";
            const string TabTipProcessName = "TabTip";

            int docked = (int)(Registry.GetValue(TabTipRegistryKeyName, TabTipDockedKey, 1) ?? 1);
            if (docked == 1)
            {
                Registry.SetValue(TabTipRegistryKeyName, TabTipDockedKey, 0);
                foreach (Process tabTipProcess in Process.GetProcessesByName(TabTipProcessName))
                    tabTipProcess.Kill();
            }
            Open();
        }

        /// <summary>
        /// Close TabTip
        /// </summary>
        public static void Close()
        {
            const int WM_SYSCOMMAND = 274;
            const int SC_CLOSE = 61536;
            SendMessage(GetTabTipWindowHandle().ToInt32(), WM_SYSCOMMAND, SC_CLOSE, 0);
        }

        static void StartPoolingForTabTipClosedEvent()
        {
            PoolingTimer.PoolUntilTrue(
                TabTipClosed,
                () => Closed?.Invoke(),
                TimeSpan.FromMilliseconds(700),
                TimeSpan.FromMilliseconds(50));
        }

        static bool TabTipClosed()
        {
            const int GWL_STYLE = -16; // Specifies we wish to retrieve window styles.
            const uint KeyboardClosedStyle = 2617245696;
            IntPtr KeyboardWnd = GetTabTipWindowHandle();
            return (KeyboardWnd.ToInt32() == 0 || GetWindowLong(KeyboardWnd, GWL_STYLE) == KeyboardClosedStyle);
        }

        // ReSharper disable once UnusedMember.Local
        static bool IsTabTipProcessRunning => GetTabTipWindowHandle() != IntPtr.Zero;

        /// <summary>
        /// Gets TabTip Window Rectangle
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        public static Rectangle GetTabTipRectangle()
        {
            if (TabTipClosed())
                return new Rectangle();

            return GetWouldBeTabTipRectangle();
        }

        static Rectangle previousTabTipRectangle;

        /// <summary>
        /// Gets Window Rectangle which would be occupied by TabTip if TabTip was opened.
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        internal static Rectangle GetWouldBeTabTipRectangle()
        {
            if (!GetWindowRect(new HandleRef(null, GetTabTipWindowHandle()), out RECT rect))
            {
                if (previousTabTipRectangle.Equals(new Rectangle())) //in case TabTip was closed and previousTabTipRectangle was not set
                    Task.Delay(TimeSpan.FromSeconds(1)).ContinueWith(TryGetTabTipRectangleToСache);

                return previousTabTipRectangle;
            }

            Rectangle wouldBeTabTipRectangle = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left + 1, rect.Bottom - rect.Top + 1);
            previousTabTipRectangle = wouldBeTabTipRectangle;

            return wouldBeTabTipRectangle;
        }

        static void TryGetTabTipRectangleToСache(Task task)
        {
            if (GetWindowRect(new HandleRef(null, GetTabTipWindowHandle()), out RECT rect))
                previousTabTipRectangle = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left + 1, rect.Bottom - rect.Top + 1);
        }
    }
}
