namespace DlhSoft.Windows.Licensing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Security;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    internal static class LicenseWarningGenerator
    {
        private static Assembly callingAssembly;
        private static bool hasWarningUsedMainWindow;
        private static TimeSpan initialTimeSpan = TimeSpan.Parse("00:00:01");
        private static TimeSpan standardTimeSpan = TimeSpan.Parse("00:10:00");
        private static Page warningPage;
        private static SecurityException warningSecurityException;
        private static DispatcherTimer warningTimer;
        private static Dictionary<Type, DateTime> warningTypes;

        private static void ContinueLink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            KeyValuePair<Window, object> pair = (hyperlink != null) ? ((KeyValuePair<Window, object>) hyperlink.Tag) : new KeyValuePair<Window, object>();
            Window key = pair.Key;
            if (key != null)
            {
                key.Content = pair.Value;
            }
        }

        private static void GenerateWarning()
        {
            if (!LicenseValidator.IsEntryAssemblyLicenseCompiler())
            {
                string str = "DlhSoft Component Licensing Warning";
                string str2 = "http://DlhSoft.com/Purchase.aspx";
                string assemblyName = GetAssemblyName(callingAssembly);
                string assemblyVersion = GetAssemblyVersion(callingAssembly);
                string purchaseUrl = string.Format("{0}?Assemblies={1},{2}", str2, assemblyName, assemblyVersion);
                string callingAssemblyNameDisplayText = string.Format("{0} (version {1})", assemblyName, assemblyVersion);
                List<string> typeNameDisplayTextList = new List<string>();
                List<Type> list2 = new List<Type>();
                foreach (Type type in warningTypes.Keys)
                {
                    DateTime time = warningTypes[type];
                    if (time.Add(initialTimeSpan) < DateTime.Now)
                    {
                        list2.Add(type);
                        typeNameDisplayTextList.Add(string.Format("{0}", type.Name));
                    }
                }
                string str7 = string.Join(", ", typeNameDisplayTextList.ToArray());
                string securityExceptionDisplayText = (warningSecurityException != null) ? warningSecurityException.Message : null;
                if ((typeNameDisplayTextList.Count > 0) || (securityExceptionDisplayText != null))
                {
                    string str13;
                    string componentsHeaderDisplayText = "These components are currently licensed to be used for testing purposes only";
                    string assemblyHeaderDisplayText = "These components are part of this assembly";
                    string purchaseHeaderDisplayText = "To purchase a standard usage license for a product that includes this assembly you can use the Purchase License page from the DlhSoft Web site";
                    string securityExceptionHeaderDisplayText = "An exception has been generated when the system tried to access and validate the component license data. To resolve the issue you may want to increase the trust level of your application";
                    if (securityExceptionDisplayText == null)
                    {
                        str13 = string.Format("{0}: {1}. {2}: {3}. {4} ({5}).", new object[] { componentsHeaderDisplayText, str7, assemblyHeaderDisplayText, callingAssemblyNameDisplayText, purchaseHeaderDisplayText, str2 });
                    }
                    else
                    {
                        str13 = string.Format("{0}: {1}. {2}: {3}. {4}: {5}", new object[] { componentsHeaderDisplayText, str7, assemblyHeaderDisplayText, callingAssemblyNameDisplayText, securityExceptionHeaderDisplayText, securityExceptionDisplayText });
                    }
                    Console.WriteLine("{0}. {1}", str, str13);
                    Window window = (Application.Current != null) ? Application.Current.MainWindow : null;
                    bool flag = window != null;
                    if (flag && hasWarningUsedMainWindow)
                    {
                        flag = false;
                    }
                    if ((flag && (warningPage != null)) && (window.Content == warningPage))
                    {
                        flag = false;
                    }
                    if (flag)
                    {
                        InitializeWarningPage(purchaseUrl, str, callingAssemblyNameDisplayText, typeNameDisplayTextList, securityExceptionHeaderDisplayText, securityExceptionDisplayText, componentsHeaderDisplayText, assemblyHeaderDisplayText, purchaseHeaderDisplayText, window);
                        window.Content = warningPage;
                    }
                    else
                    {
                        MessageBox.Show(str13, str, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    foreach (Type type2 in list2)
                    {
                        warningTypes[type2] = DateTime.Now;
                    }
                    hasWarningUsedMainWindow = flag;
                }
            }
            if (warningTimer.Interval < standardTimeSpan)
            {
                warningTimer.Interval = standardTimeSpan;
            }
        }

        private static string GetAssemblyName(Assembly assembly)
        {
            string str = assembly.ToString();
            int startIndex = 0;
            int index = str.IndexOf(',');
            str = (startIndex >= 0) ? ((index >= 0) ? str.Substring(startIndex, index - startIndex) : str.Substring(startIndex)) : string.Empty;
            return str.Trim();
        }

        private static string GetAssemblyVersion(Assembly assembly)
        {
            string str = assembly.ToString();
            string str2 = "Version=";
            int index = str.IndexOf(str2);
            if (index >= 0)
            {
                index += str2.Length;
            }
            int num2 = (index >= 0) ? str.IndexOf(',', index) : -1;
            str = (index >= 0) ? ((num2 >= 0) ? str.Substring(index, num2 - index) : str.Substring(index)) : string.Empty;
            str = str.Trim();
            int length = str.IndexOf('.');
            if (length >= 0)
            {
                str = str.Substring(0, length);
            }
            return str.Trim();
        }

        private static void InitializeWarningPage(string purchaseUrl, string titleHeaderDisplayText, string callingAssemblyNameDisplayText, List<string> typeNameDisplayTextList, string securityExceptionHeaderDisplayText, string securityExceptionDisplayText, string componentsHeaderDisplayText, string assemblyHeaderDisplayText, string purchaseHeaderDisplayText, Window window)
        {
            if (warningPage == null)
            {
                warningPage = new Page();
            }
            warningPage.Title = titleHeaderDisplayText;
            Style style = new Style(typeof(FlowDocument)) {
                Setters = { new Setter(FlowDocument.TextAlignmentProperty, TextAlignment.Left), new Setter(FlowDocument.FontSizeProperty, 16.0) }
            };
            Style basedOn = new Style(typeof(Paragraph)) {
                Setters = { new Setter(Block.MarginProperty, new Thickness(0.0, 8.0, 0.0, 8.0)) }
            };
            Style style3 = new Style(typeof(Paragraph), basedOn) {
                Setters = { new Setter(TextElement.FontSizeProperty, 20.0), new Setter(TextElement.FontWeightProperty, FontWeights.Bold), new Setter(Block.MarginProperty, new Thickness(0.0, 16.0, 0.0, 8.0)), new Setter(Paragraph.KeepWithNextProperty, true) }
            };
            Border element = new Border();
            warningPage.Content = element;
            element.Padding = new Thickness(4.0);
            element.Background = SystemColors.InfoBrush;
            TextElement.SetForeground(element, SystemColors.InfoTextBrush);
            FlowDocumentScrollViewer viewer = new FlowDocumentScrollViewer();
            element.Child = viewer;
            ScrollViewer.SetVerticalScrollBarVisibility(viewer, ScrollBarVisibility.Auto);
            FlowDocument document = new FlowDocument();
            viewer.Document = document;
            document.Style = style;
            Paragraph item = new Paragraph(new System.Windows.Documents.Run(titleHeaderDisplayText));
            document.Blocks.Add(item);
            item.Style = style3;
            Paragraph paragraph2 = new Paragraph(new System.Windows.Documents.Run(string.Format("{0}:", componentsHeaderDisplayText)));
            document.Blocks.Add(paragraph2);
            paragraph2.Style = basedOn;
            BlockUIContainer container = new BlockUIContainer();
            document.Blocks.Add(container);
            container.Foreground = Brushes.Red;
            ItemsControl control = new ItemsControl();
            container.Child = control;
            control.ItemsSource = typeNameDisplayTextList;
            Paragraph paragraph3 = new Paragraph(new System.Windows.Documents.Run(string.Format("{0}:", assemblyHeaderDisplayText)));
            document.Blocks.Add(paragraph3);
            paragraph3.Style = basedOn;
            BlockUIContainer container2 = new BlockUIContainer();
            document.Blocks.Add(container2);
            container2.Foreground = Brushes.Red;
            ContentControl control2 = new ContentControl();
            container2.Child = control2;
            control2.Content = callingAssemblyNameDisplayText;
            KeyValuePair<Window, object> pair = new KeyValuePair<Window, object>(window, (window != null) ? window.Content : null);
            if (securityExceptionDisplayText == null)
            {
                Paragraph paragraph4 = new Paragraph(new System.Windows.Documents.Run(string.Format("{0}:", purchaseHeaderDisplayText)));
                document.Blocks.Add(paragraph4);
                paragraph4.Style = basedOn;
                Paragraph paragraph5 = new Paragraph();
                document.Blocks.Add(paragraph5);
                paragraph5.Style = basedOn;
                Hyperlink hyperlink = new Hyperlink();
                paragraph5.Inlines.Add(hyperlink);
                hyperlink.Inlines.Add("Open Purchase License page");
                hyperlink.NavigateUri = new Uri(purchaseUrl);
                if (!(window is NavigationWindow))
                {
                    hyperlink.Click += new RoutedEventHandler(LicenseWarningGenerator.PurchaseLink_Click);
                }
                Rectangle uiElement = new Rectangle {
                    Width = 8.0
                };
                paragraph5.Inlines.Add(uiElement);
                Hyperlink hyperlink2 = new Hyperlink();
                paragraph5.Inlines.Add(hyperlink2);
                hyperlink2.Inlines.Add("Continue trial");
                hyperlink2.Tag = pair;
                hyperlink2.Click += new RoutedEventHandler(LicenseWarningGenerator.ContinueLink_Click);
            }
            else
            {
                Paragraph paragraph6 = new Paragraph(new System.Windows.Documents.Run(string.Format("{0}:", securityExceptionHeaderDisplayText)));
                document.Blocks.Add(paragraph6);
                paragraph6.Style = basedOn;
                Paragraph paragraph7 = new Paragraph(new System.Windows.Documents.Run(securityExceptionDisplayText));
                document.Blocks.Add(paragraph7);
                paragraph7.Foreground = Brushes.Red;
                paragraph7.Style = basedOn;
                Paragraph paragraph8 = new Paragraph();
                document.Blocks.Add(paragraph8);
                paragraph8.Style = basedOn;
                Hyperlink hyperlink3 = new Hyperlink();
                paragraph8.Inlines.Add(hyperlink3);
                hyperlink3.Inlines.Add("Continue");
                hyperlink3.Tag = pair;
                hyperlink3.Click += new RoutedEventHandler(LicenseWarningGenerator.ContinueLink_Click);
            }
        }

        private static void LicenseWarningTimer_Tick(object sender, EventArgs e)
        {
            GenerateWarning();
        }

        private static void PurchaseLink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hyperlink = sender as Hyperlink;
            Uri uri = (hyperlink != null) ? hyperlink.NavigateUri : null;
            if (uri != null)
            {
                Process.Start(uri.AbsoluteUri);
            }
        }

        internal static void Register(SecurityException securityException)
        {
            warningSecurityException = securityException;
        }

        internal static void Register(Type type)
        {
            if (callingAssembly == null)
            {
                callingAssembly = Assembly.GetCallingAssembly();
                warningTypes = new Dictionary<Type, DateTime>();
                warningTimer = new DispatcherTimer();
                warningTimer.Interval = initialTimeSpan;
                warningTimer.Tick += new EventHandler(LicenseWarningGenerator.LicenseWarningTimer_Tick);
            }
            if (!warningTypes.ContainsKey(type))
            {
                warningTimer.Stop();
                warningTypes.Add(type, DateTime.MinValue);
                warningTimer.Interval = initialTimeSpan;
                if (Application.Current != null)
                {
                    warningTimer.Start();
                }
                else
                {
                    GenerateWarning();
                }
            }
        }
    }
}

