using KProcess.Presentation.Windows;
using System;
using System.Threading.Tasks;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Définit un service permettant d'envoyer un ticket Freshdesk.
    /// </summary>
    public interface IFreshdeskService : IPresentationService
    {
        /// <summary>
        /// Envoie un rapport d'erreur par Ticket.
        /// </summary>
        /// <param name="e">L'exception.</param>
        /// <param name="username">Le nom d'utilisateur.</param>
        /// <param name="company">La société.</param>
        /// <param name="email">L'email.</param>
        /// <param name="additionalInformation">Des informations complémentaires.</param>
        /// <returns>
        ///   <c>true</c> si l'envoi a réussi.
        /// </returns>
        Task SendErrorReportTicketAsync(Exception e, string username, string company, string email, string additionalInformation);

    }
}
