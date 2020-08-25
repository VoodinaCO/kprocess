using FreshDeskLib;
using KProcess.KL2.APIClient;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Security.Activation;
using log4net;
using log4net.Appender;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.Shell
{
    /// <summary>
    /// Représente un service capable d'envoyer un ticket Freshdesk.
    /// </summary>
    public class FreshdeskService : IFreshdeskService
    {
        /// <summary>
        /// Le nombre de lignes à inclure pour chaque fichier de log.
        /// </summary>
        const int LogFilesLinesToInclude = 500;

        /// <summary>
        /// L'adresse email du développeur pour l'envoi de rapport.
        /// </summary>
        const string DevEmail = "pierre-yves.cassard@k-process.com";

        /// <summary>
        /// L'adresse email cc.
        /// </summary>
        const string ccmail = "KL2Suite@k-process.com";

        /// <summary>
        /// L'id du mail k-processcomsupport@kprocess.freshdesk.com
        /// </summary>
        const long email_config_id = 16000018357;

        /// <summary>
        /// L'id du groupe Développement
        /// </summary>
        const long group_id = 16000053569;

        const string API_KEY = "Q2SIdFBsNAxYJ2Xhew0D";
        const string API_FRESHDESK_OUTBOUND_EMAIL = "https://kprocess.freshdesk.com/api/v2/tickets/outbound_email";
        const string API_FRESHDESK_TICKETS = "https://kprocess.freshdesk.com/api/v2/tickets";

        public FreshdeskService()
        {
            FreshdeskClient.Instance.Initialize(DevEmail, API_KEY, API_FRESHDESK_TICKETS, API_FRESHDESK_OUTBOUND_EMAIL);
        }

        /// <summary>
        /// Envoie un rapport d'erreur par ticket.
        /// </summary>
        /// <param name="e">L'exception.</param>
        /// <param name="username">Le nom d'utilisateur.</param>
        /// <param name="company">La société.</param>
        /// <param name="email">L'email.</param>
        /// <param name="additionalInformation">Des informations complémentaires.</param>
        /// <returns>
        ///   <c>true</c> si l'envoi a réussi.
        /// </returns>
        public async Task SendErrorReportTicketAsync(Exception e, string username, string company, string email, string additionalInformation)
        {
            Ticket ticket = new Ticket()
            {
                Email = DevEmail,
                Name = username,
                Subject = "[KL2] Rapport d'erreur",
                EmailConfigId = email_config_id,
                GroupId = group_id,
                Type = "Bug",
                CcEmails = new[] { ccmail }
            };

            StringBuilder sb = new StringBuilder();

            // HEURE
            sb.AppendLine(DateTimeOffset.Now.ToString());

            // Informations client 
            sb.AppendLine("Informations client :");
            sb.AppendLine($"Nom : {(!string.IsNullOrEmpty(username) ? username : string.Empty)}");

            sb.AppendLine($"Société : {(!string.IsNullOrEmpty(company) ? company : string.Empty)}");

            sb.AppendLine($"Email : {(!string.IsNullOrEmpty(email) ? email : string.Empty)}");

            sb.AppendLine();


            // VERSION APPLI
            sb.AppendLine($"Version : {System.Reflection.Assembly.GetExecutingAssembly().GetName().Version}");
            sb.AppendLine();


            // Informations complémentaires
            /*sb.AppendLine("Informations complémentaires :");
            if (!string.IsNullOrEmpty(additionalInformation))
                sb.AppendLine(additionalInformation);
            sb.AppendLine();*/


            // Inforamtions licence
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

            StringBuilder log = new StringBuilder();
            log.AppendLine("Contenu des fichiers de logs");

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

                            log.AppendLine();
                            log.AppendLine(appender.File);
                            log.AppendLine();
                            foreach (string l in lines.Skip(lines.Count - LogFilesLinesToInclude).Take(LogFilesLinesToInclude))
                                log.AppendLine(l);
                            log.AppendLine();
                            log.AppendLine();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.AppendLine("Impossible de récupérer les fichiers de log. Erreur :");
                log.AppendLine(ex.Message);
            }

            ticket.Description = sb.Replace(Environment.NewLine, "<br />").ToString();

            ticket.Attachments = new Dictionary<string, byte[]>
            {
                ["Log.txt"] = Encoding.UTF8.GetBytes(log.ToString())
            };

            try
            {
                await FreshdeskClient.Instance.SendTicketAsync(ticket);
            }
            catch (Exception ex)
            {
                TraceManager.TraceWarning(ex.Message);
            }
        }
    }
}
