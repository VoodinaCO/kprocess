using System;
using System.Runtime.InteropServices;

namespace KProcess.Presentation.Windows.Controls.DirectShow
{
    [ComImport, System.Security.SuppressUnmanagedCodeSecurity, Guid("388EEF20-40CC-4752-A0FF-66AA5C4AF8FA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ISettingsInterface
    {
        [PreserveSig]
        int GetParameter(
            [MarshalAs(UnmanagedType.LPStr)] String type,
            [MarshalAs(UnmanagedType.I4)] int buffersize,
            [In, Out, MarshalAs(UnmanagedType.LPStr)] String value,
            [In, Out, MarshalAs(UnmanagedType.I4)] ref int length
            );

        [PreserveSig]
        int SetParameter(
            [MarshalAs(UnmanagedType.LPStr)] String type,
            [MarshalAs(UnmanagedType.LPStr)] String value
            );

        [PreserveSig]
        int GetParameterSettings(
            [MarshalAs(UnmanagedType.LPStr)] ref String szResult,
            [In] int nSize
            );
    }
}
