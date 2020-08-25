using KProcess.KL2.APIClient;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Security.Activation;
using log4net;
using log4net.Appender;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.Shell
{
    /// <summary>
    /// Représente un service capable d'envoyer un email.
    /// </summary>
    public class EmailService : IEmailService
    {

        /// <summary>
        /// Le nombre de lignes à inclure pour chaque fichier de log.
        /// </summary>
        private const int LogFilesLinesToInclude = 500;

        /// <summary>
        /// L'adresse email du développeur pour l'envoi de rapport.
        /// </summary>
        private const string DevEmail = "ksmed@tekigo.com";

        /// <summary>
        /// Envoie un email.
        /// </summary>
        /// <param name="email">L'email à envoyer.</param>
        /// <returns>
        ///   <c>true</c> si l'envoi a réussi.
        /// </returns>
        public bool SendEmail(Email email)
        {
            var mailMessage = new MailMessage();

            if (string.IsNullOrEmpty(email.From))
                throw new InvalidOperationException("L'email doit avoir un expéditeur.");

            if (!email.PreferConfigurationFrom)
                mailMessage.From = new MailAddress(email.From);

            bool hasTo = false;

            if (email.To != null)
                foreach (var to in email.To)
                {
                    mailMessage.To.Add(new MailAddress(to));
                    hasTo = true;
                }

            if (email.CC != null)
                foreach (var cc in email.CC)
                {
                    mailMessage.CC.Add(new MailAddress(cc));
                    hasTo = true;
                }

            if (email.Bcc != null)
                foreach (var bcc in email.Bcc)
                {
                    mailMessage.Bcc.Add(new MailAddress(bcc));
                    hasTo = true;
                }

            if (!hasTo)
                throw new InvalidOperationException("L'email doit avoir un destinaire (To) ou une copie (CC)");

            if (!string.IsNullOrEmpty(email.Subject))
                mailMessage.Subject = email.Subject;

            if (!string.IsNullOrEmpty(email.Body))
                mailMessage.Body = email.Body;

            if (email.Attachments != null)
            {
                foreach (var att in email.Attachments)
                {
                    var attach = new Attachment(att);
                    mailMessage.Attachments.Add(attach);
                }
            }

            email.AttachmentsStreams?.ForEach(kvp =>
            {
                Attachment attach = new Attachment(kvp.Value, kvp.Key);
                mailMessage.Attachments.Add(attach);
            });

            try
            {
                TrySend(mailMessage);
                return true;
            }
            catch(InvalidOperationException e)
            {
                this.TraceError(e, e.Message);
                // Si l'envoi n'a pas réussi une première fois, réessayer mais cette fois-ci avec le destinataire spécifié.
                if (email.PreferConfigurationFrom)
                {
                    mailMessage.From = new MailAddress(email.From);
                    try
                    {
                        TrySend(mailMessage);
                        return true;
                    }
                    catch (Exception inner)
                    {
                        this.TraceError(inner, inner.Message);
                    }
                }
            }
            catch(Exception e)
            {
                this.TraceError(e, e.Message);
            }
            return false;
        }

        /// <summary>
        /// Tente d'envoyer l'email.
        /// </summary>
        /// <param name="mailMessage">L'email.</param>
        private void TrySend(MailMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                client.Send(mailMessage);
            }
        }

        /// <summary>
        /// Envoie un rapport d'erreur par email.
        /// </summary>
        /// <param name="e">L'exception.</param>
        /// <param name="username">Le nom d'utilisateur.</param>
        /// <param name="company">La société.</param>
        /// <param name="email">L'email.</param>
        /// <param name="additionalInformation">Des informations complémentaires.</param>
        /// <returns>
        ///   <c>true</c> si l'envoi a réussi.
        /// </returns>
        public async Task<bool> SendErrorReportEmail(Exception e, string username, string company, string email, string additionalInformation)
        {
            string errorReportEmail = ConfigurationManager.AppSettings["ErrorReportEmail"];

            bool sendCopyToDev = false;
#if !DEBUG
            string copyReportEmail = ConfigurationManager.AppSettings["SendReportCopy"];
            if (copyReportEmail != null)
                bool.TryParse(copyReportEmail, out sendCopyToDev);
#endif

            var emailessage = new Email()
            {
                From = errorReportEmail,
                PreferConfigurationFrom = true,
                To = new string[] { errorReportEmail },
                Subject = "[Ksmed] Rapport d'erreur",
            };

            if (sendCopyToDev)
                emailessage.Bcc = new string[] { DevEmail };

            emailessage.AttachmentsStreams = new Dictionary<string, Stream>();

            var sb = new StringBuilder();

            // HEURE
            sb.AppendLine(DateTimeOffset.Now.ToString());

            // Informations client 
            sb.AppendLine("Informations client :");
            sb.AppendLine("Nom : ");
            if (!string.IsNullOrEmpty(username))
                sb.AppendLine(username);
            else
                sb.AppendLine();

            sb.AppendLine("Société : ");
            if (!string.IsNullOrEmpty(company))
                sb.AppendLine(company);
            else
                sb.AppendLine();

            sb.AppendLine("Email : ");
            if (!string.IsNullOrEmpty(email))
                sb.AppendLine(email);
            else
                sb.AppendLine();

            sb.AppendLine();


            // VERSION APPLI
            sb.AppendLine("Version :");
            sb.AppendLine(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            sb.AppendLine();


            // Informations complémentaires
            sb.AppendLine("Informations complémentaires :");
            if (!string.IsNullOrEmpty(additionalInformation))
                sb.AppendLine(additionalInformation);
            sb.AppendLine();


            // Informations licence
            sb.AppendLine("Informations licence :");
            IAPIHttpClient apiClient = IoC.Resolve<IAPIHttpClient>();
            var licenseInfo = await apiClient.ServiceAsync<WebProductLicense>(KL2_Server.API, "LicenseService", "GetLicense");
            if (licenseInfo != null && licenseInfo.Status != WebLicenseStatus.NotFound)
            {
                sb.AppendFormat("{0} {1}", licenseInfo.Status, licenseInfo.StatusReason);
            }
            else
                sb.AppendLine("Aucune licence chargée");
            sb.AppendLine();

            // Informations erreur en cours
            sb.AppendLine("Erreur en cours");
            if (e != null)
                sb.AppendLine(e.ToString());
            sb.AppendLine();


            sb.AppendLine("Contenu des fichiers de logs");

            try
            {
                foreach (var appender in ((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).Root.Appenders.OfType<FileAppender>())
                {

                    using (var fs = new FileStream(appender.File, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (var sr = new StreamReader(fs, appender.Encoding))
                        {
                            var lines = new List<string>();
                            string line;
                            while ((line = await sr.ReadLineAsync()) != null)
                                lines.Add(line);

                            sb.AppendLine();
                            sb.AppendLine(appender.File);
                            sb.AppendLine();
                            foreach (string l in lines.Skip(lines.Count - LogFilesLinesToInclude).Take(LogFilesLinesToInclude))
                                sb.AppendLine(l);
                            sb.AppendLine();
                            sb.AppendLine();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine("Impossible de récupérer les fichiers de log. Erreur :");
                sb.AppendLine(ex.Message);
            }
            sb.AppendLine();

            emailessage.Body = sb.ToString();

            var ret = SendEmail(emailessage);

            return ret;
        }

    }
}
