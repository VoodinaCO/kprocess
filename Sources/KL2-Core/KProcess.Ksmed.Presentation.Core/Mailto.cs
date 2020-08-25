using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Représente une classe permettant d'écrire un mailto: pour une url.
    /// </summary>
    public class Maito
    {
        /// <summary>
        /// Obtient ou définit les destinataires.
        /// </summary>
        public string[] To { get; set; }

        /// <summary>
        /// Obtient ou définit les copies.
        /// </summary>
        public string[] Cc { get; set; }

        /// <summary>
        /// Obtient ou définit les copies cachées.
        /// </summary>
        public string[] Bcc { get; set; }

        /// <summary>
        /// Obtient ou définit le sujet.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Obtient ou définit le contenu.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Retourne une <see cref="System.String"/> représentant cette instance.
        /// </summary>
        /// <returns>
        /// Une <see cref="System.String"/> représentant cette instance.
        /// </returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("mailto:");

            string to = GetAddresses(To);
            if (to != null)
                sb.Append(to);

            sb.Append("?");

            string cc = GetAddresses(Cc);
            if (cc != null)
            {
                sb.Append("cc=");
                sb.Append(cc);
                sb.Append("&");
            }

            string bcc = GetAddresses(Bcc);
            if (bcc != null)
            {
                sb.Append("bcc=");
                sb.Append(bcc);
                sb.Append("&");
            }

            if (Subject != null)
            {
                sb.Append("subject=");
                sb.Append(Encode(Subject));
                sb.Append("&");
            }

            if (Body != null)
            {
                sb.Append("body=");
                sb.Append(Encode(Body));
                sb.Append("&");
            }

            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();

        }

        /// <summary>
        /// Encore le texte spécifié pour l'url.
        /// </summary>
        /// <param name="str">le texte.</param>
        /// <returns>Le texte encodé.</returns>
        private string Encode(string str)
        {
            str = Uri.EscapeUriString(str);
            str = str.Replace("&", "%26");
            return str;
        }

        /// <summary>
        /// Joint les adresses.
        /// </summary>
        /// <param name="adresses">les adresses.</param>
        /// <returns>Le texte joint</returns>
        private string GetAddresses(string[] adresses)
        {
            if (adresses != null && adresses.Any())
                return string.Join("; ", adresses);
            else
                return null;
        }
    }
}
