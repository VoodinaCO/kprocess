using FFmpeg.AutoGen;
using GalaSoft.MvvmLight.Threading;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using Unosquare.FFME;

namespace Kprocess.KL2.TabletClient
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        public SplashScreen SplashScreen { get; private set; }

        static App()
        {
            /*AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
            {
                Locator.TraceManager.TraceError(e.Exception, e.Exception.Message);
            };*/

            DispatcherHelper.Initialize();

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("fr-FR");
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage("fr-FR")));

            /*TabTipAutomation.IgnoreHardwareKeyboard = HardwareKeyboardIgnoreOptions.IgnoreAll;
            TabTipAutomation.BindTo<PasswordBox>();
            TabTipAutomation.BindTo<TextBox>();*/
        }

        public App()
        {
            SplashScreen = new SplashScreen("Resources/kl2_FieldService-splash.png");
            SplashScreen.Show(false, false);

            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTA3NjkwQDMxMzcyZTMxMmUzMGRpNUF0QThQTklxRFlMeDhXR3NRZFlHcnU4VjN5Z2tGS3FaT2sxM1hER1E9;MTA3NjkxQDMxMzcyZTMxMmUzMFVqQ0tFMGcxclFlNUVKVUJtdWpGcWlmQ1gvbE9PMGxZYWxHTFZYSmxHZDQ9;MTA3NjkyQDMxMzcyZTMxMmUzMEZMTENrQjNCYjUvNmtKTHFNRHBGRkxXVlFXNGZReDVrbWl2WXZsczFORlk9;MTA3NjkzQDMxMzcyZTMxMmUzME5MY3VCaXEydmZyS085RmhzdWtMK2pjNE4wQmY1RUZxMHorMHhHZGlpOFU9;MTA3Njk0QDMxMzcyZTMxMmUzMERPVCtOWHBlT0NZVkM5aExMOUhtL3JMdDFyMWcySEl1cGdlZjBubm1wQW89;MTA3Njk1QDMxMzcyZTMxMmUzMFM5YmYyTDhoL1N4Z2RIeEZ4S1p6KzF5eGxkdkp2VDNIMFpkZHBwYWVndFE9;MTA3Njk2QDMxMzcyZTMxMmUzMEtFZTNrcnBaelBHUXg4TC9kdGE0TjVQTHNIVVloTDZ3N1I4VHQ0NysvTEE9;MTA3Njk3QDMxMzcyZTMxMmUzMExlamxrNmhmL0RyR3Y2QzBoYitxOXhyazRnR0JZUUJBZThJZmhENG1oVGs9;MTA3Njk4QDMxMzcyZTMxMmUzMGRrTFo4clJIR29kZEZrdlRSUlFsQ3VvckJGRjNiZCtscFNYbE0zRThQSGc9;MTA3Njk5QDMxMzcyZTMxMmUzMERPVCtOWHBlT0NZVkM5aExMOUhtL3JMdDFyMWcySEl1cGdlZjBubm1wQW89");

            DispatcherUnhandledException += (sender, e) =>
            {
                Locator.TraceManager.TraceError(e.Exception, e.Exception.Message);
                e.Handled = true;
            };

            Library.FFmpegDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "ffme");
            try
            {
                Library.FFmpegLoadModeFlags = FFmpegLoadMode.FullFeatures;
                Library.LoadFFmpeg();

                Locator.TraceManager.TraceDebug($"FFmpeg Libraries {Library.FFmpegVersionInfo} loaded");
            }
            catch(Exception ex)
            {
                Locator.TraceManager.TraceError(ex, $"Unable to Load FFmpeg Libraries from path : {Library.FFmpegDirectory}");
            }
        }

        /// <summary>
        /// Active la première fenêtre.
        /// </summary>
        internal void Activate()
        {
            if (Windows.Count >= 0)
                Windows[0].Activate();
        }
    }
}
