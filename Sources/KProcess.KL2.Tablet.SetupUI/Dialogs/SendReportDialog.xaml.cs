using FreshDeskLib;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace KProcess.KL2.Tablet.SetupUI
{
    /// <summary>
    /// Logique d'interaction pour SendReportDialog.xaml
    /// </summary>
    public partial class SendReportDialog : CustomDialog
    {
        #region FreshDesk

        private const string DevEmail = "pierre-yves.cassard@k-process.com";
        private const string ccmail = "KL2Suite@k-process.com";
        private const long email_config_id = 16000018357;
        private const long group_id = 16000053569;
        private const string API_KEY = "Q2SIdFBsNAxYJ2Xhew0D";
        private const string API_FRESHDESK_OUTBOUND_EMAIL = "https://kprocess.freshdesk.com/api/v2/tickets/outbound_email";
        private const string API_FRESHDESK_TICKETS = "https://kprocess.freshdesk.com/api/v2/tickets";

        #endregion

        public SendReportDialog()
        {
            InitializeComponent();

            FreshdeskClient.Instance.Initialize(DevEmail, API_KEY, API_FRESHDESK_TICKETS, API_FRESHDESK_OUTBOUND_EMAIL);
        }

        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register("Username", typeof(string), typeof(SendReportDialog), new PropertyMetadata(default(string), InfoPropertyChanged));
        public static readonly DependencyProperty CompanyProperty = DependencyProperty.Register("Company", typeof(string), typeof(SendReportDialog), new PropertyMetadata(default(string), InfoPropertyChanged));
        public static readonly DependencyProperty EmailProperty = DependencyProperty.Register("Email", typeof(string), typeof(SendReportDialog), new PropertyMetadata(default(string), InfoPropertyChanged));
        
        private static void InfoPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SendReportDialog currentDialog = d as SendReportDialog;
            (currentDialog.SendLogCommand as DelegateCommand<object>)?.RaiseCanExecuteChanged();
        }

        public string Username
        {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }

        public string Company
        {
            get { return (string)GetValue(CompanyProperty); }
            set { SetValue(CompanyProperty, value); }
        }

        public string Email
        {
            get { return (string)GetValue(EmailProperty); }
            set { SetValue(EmailProperty, value); }
        }

        public ICommand SendLogCommand { get; } = new DelegateCommand<object>(async e =>
        {
            SendReportDialog currentDialog = e as SendReportDialog;
            await MainViewModel.Instance.DialogService.HideMetroDialogAsync(MainViewModel.Instance, currentDialog);
            var progressController = await MainViewModel.Instance.DialogService.ShowProgressAsync(MainViewModel.Instance, LocalizationExt.CurrentLanguage.GetLocalizedValue("LogProgressDialogTitle"), LocalizationExt.CurrentLanguage.GetLocalizedValue("LogProgressDialogInProgress"));
            progressController.SetIndeterminate();
#if DEBUG
            var ticket = new Ticket()
            {
                Email = DevEmail,
                Name = string.Empty,
                Subject = "[KL2 Tablet] Rapport d'erreur (test)",
                EmailConfigId = email_config_id,
                GroupId = group_id,
                Type = "Installation",
                CcEmails = new[] { ccmail }
            };
            var sb = new StringBuilder();
            // HEURE
            sb.AppendLine(DateTimeOffset.Now.ToString());
            // Informations client 
            sb.AppendLine("Informations client :");
            sb.AppendLine($"Nom : {currentDialog.Username}");
            sb.AppendLine($"Société : {currentDialog.Company}");
            sb.AppendLine($"Email : {currentDialog.Email}");
            sb.AppendLine();
            // VERSION APPLI
            sb.AppendLine("Version : 4.0.0.0 beta");
            ticket.Description = sb.Replace(Environment.NewLine, "<br />").ToString();

            try
            {
                if (await Task.Run(async () => await FreshdeskClient.Instance.SendTicketAsync(ticket)))
                {
                    progressController.SetProgress(0);
                    progressController.SetMessage(LocalizationExt.CurrentLanguage.GetLocalizedValue("LogProgressDialogSuccess"));
                }
                else
                {
                    progressController.SetProgress(0);
                    progressController.SetMessage(LocalizationExt.CurrentLanguage.GetLocalizedValue("LogProgressDialogFailure"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(FormatException(ex));
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
                await progressController.CloseAsync();
            }
#else
            byte[] logsZipFile = CreateLogZip(ManagedBA.Instance.Engine.StringVariables["WixBundleLog"]);

            var ticket = new Ticket()
            {
                Email = DevEmail,
                Name = string.Empty,
                Subject = "[KL2 Tablet] Rapport d'erreur",
                EmailConfigId = email_config_id,
                GroupId = group_id,
                Type = "Installation"
            };

            var sb = new StringBuilder();

            // HEURE
            sb.AppendLine(DateTimeOffset.Now.ToString());

            // Informations client 
            sb.AppendLine("Informations client :");
            sb.AppendLine($"Nom : {currentDialog.Username}");
            sb.AppendLine($"Société : {currentDialog.Company}");
            sb.AppendLine($"Email : {currentDialog.Email}");
            sb.AppendLine();

            // VERSION APPLI
            sb.AppendLine($"Version : {ManagedBA.Instance.Engine.VersionVariables["WixBundleVersion"].ToString()}");
            ticket.Description = sb.Replace(Environment.NewLine, "<br />").ToString();

            ticket.Attachments = new Dictionary<string, byte[]>();
            ticket.Attachments.Add("Logs.zip", logsZipFile);

            try
            {
                if (await Task.Run(async () => await FreshdeskClient.Instance.SendTicketAsync(ticket)))
                {
                    progressController.SetProgress(0);
                    progressController.SetMessage(LocalizationExt.CurrentLanguage.GetLocalizedValue("LogProgressDialogSuccess"));
                }
                else
                {
                    progressController.SetProgress(0);
                    progressController.SetMessage(LocalizationExt.CurrentLanguage.GetLocalizedValue("LogProgressDialogFailure"));
                }
            }
            catch (Exception ex)
            {
                ManagedBA.Instance.LogExceptionWithInner(ex);
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
                await progressController.CloseAsync();
            }
#endif
        }, e =>
        {
            SendReportDialog currentDialog = e as SendReportDialog;
            return !string.IsNullOrEmpty(currentDialog.Username) && !string.IsNullOrEmpty(currentDialog.Company) && !string.IsNullOrEmpty(currentDialog.Email);
        });

        private static string FormatException(Exception e) =>
            e.InnerException != null ? $"{e.Message}\nInner : {FormatException(e)}" : e.Message;

        private async void PART_NegativeButton_Click(object sender, RoutedEventArgs e)
        {
            await MainViewModel.Instance.DialogService.HideMetroDialogAsync(MainViewModel.Instance, this);
        }

        public static byte[] CreateLogZip(string mainLogFilePath)
        {
            byte[] retVal = null;

            var dirInfo = Directory.GetParent(mainLogFilePath);
            var baseFileName = System.IO.Path.GetFileNameWithoutExtension(mainLogFilePath);
            var logFiles = dirInfo.EnumerateFiles($"{baseFileName}*.log").Select(i => i.FullName);

            using (MemoryStream memStream = new MemoryStream())
            {
                using (ZipArchive archive = new ZipArchive(memStream, ZipArchiveMode.Create, true))
                {
                    foreach (string file in logFiles)
                    {
                        var entry = archive.CreateEntry(System.IO.Path.GetFileName(file));
                        using (var entryStream = entry.Open())
                        {
                            using (var fileStream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            {
                                fileStream.CopyTo(entryStream);
                            }
                        }
                    }
                }
                retVal = memStream.ToArray();
            }

            return retVal;
        }
    }
}
