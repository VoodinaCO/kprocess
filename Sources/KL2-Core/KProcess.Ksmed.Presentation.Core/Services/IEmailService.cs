using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Définit un service permettant d'envoyer un email.
    /// </summary>
    public interface IEmailService : IPresentationService
    {

        /// <summary>
        /// Envoie un email.
        /// </summary>
        /// <param name="email">L'email à envoyer.</param>
        /// <returns><c>true</c> si l'envoi a réussi.</returns>
        bool SendEmail(Email email);

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
        Task<bool> SendErrorReportEmail(Exception e, string username, string company, string email, string additionalInformation);

    }

    /// <summary>
    /// Représente un email.
    /// </summary>
    public class Email
    {
        /// <summary>
        /// Obtient ou définit l'expéditeur.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'expéditeur spécifié dans le fichier de configuration doit être utilisé en priorité par rapport à celui spécifié dans <see cref="From"/>.
        /// </summary>
        public bool PreferConfigurationFrom { get; set; }

        /// <summary>
        /// Obtient ou définit les destinataires.
        /// </summary>
        public string[] To { get; set; }

        /// <summary>
        /// Obtient ou définit les copies.
        /// </summary>
        public string[] CC { get; set; }

        /// <summary>
        /// Obtient ou définit les copies.
        /// </summary>
        public string[] Bcc { get; set; }

        /// <summary>
        /// Obtient ou définit le sujet.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Obtient ou définit le corps.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Obtient ou définit les attachements.
        /// </summary>
        public string[] Attachments { get; set; }

        /// <summary>
        /// Obtient ou définit les attachements sous forme de streams.
        /// </summary>
        public Dictionary<string, Stream> AttachmentsStreams { get; set; }
    }
}
