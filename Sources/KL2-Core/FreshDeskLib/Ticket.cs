using Newtonsoft.Json;

namespace FreshDeskLib
{
    /// <summary>
    /// Représente un ticket.
    /// </summary>
    public class Ticket : OutboundMail
    {
        /// <summary>
        /// Obtient ou définit le statut du ticket.
        /// La valeur par défaut est Open.
        /// </summary>
        [JsonProperty("status")]
        public override TicketStatus Status { get; set; } = TicketStatus.Open;

        /// <summary>
        /// Obtient ou définit l'User ID du demandeur. Pour les contacts existants, requester_id peut être défini au lieu de l'<see cref="email"/> du demandeur.
        /// </summary>
        [JsonProperty("requester_id")]
        public int? RequesterId { get; set; }

        /// <summary>
        /// Obtient ou définit le Facebook ID du demandeur. Si aucun contact n'existe avec ce facebook_id, alors un nouveau contact est créé.
        /// </summary>
        [JsonProperty("facebook_id")]
        public string FacebookId { get; set; }

        /// <summary>
        /// Obtient ou définit le numéro de téléphone du demandeur. Si aucun contact n'existe avec ce numéro dans Freshdesk, il est ajouté en tant que nouveau contact.
        /// Si phone est défini mais pas <see cref="email"/>, alors <see cref="name"/> est obligatoire.
        /// </summary>
        [JsonProperty("phone")]
        public string Phone { get; set; }

        /// <summary>
        /// Obtient ou définit le Twitter ID du demandeur. Si aucun contact n'existe avec ce twitter_id, alors un nouveau contact est créé.
        /// </summary>
        [JsonProperty("twitter_id")]
        public string TwitterId { get; set; }

        /// <summary>
        /// Obtient ou définit l'ID de l'agent à qui a été assigné le ticket.
        /// </summary>
        [JsonProperty("responder_id")]
        public int? ResponderId { get; set; }

        /// <summary>
        /// Obtient ou définit les adresses mail en 'cc'.
        /// </summary>
        [JsonProperty("cc_emails")]
        public string[] CcEmails { get; set; }

        /// <summary>
        /// Obtient ou définit l'ID du produit auquel est associé le ticket.
        /// Il est ignoré si <see cref="email_config_id"/> est défini dans la requête.
        /// </summary>
        [JsonProperty("product_id")]
        public int? ProductId { get; set; }

        /// <summary>
        /// Obtient ou définit l'origine du ticket.
        /// La valeur par défaut est Portal.
        /// </summary>
        [JsonProperty("source")]
        public TicketSource Source { get; set; } = TicketSource.Portal;

        /// <summary>
        /// Obtient ou définit l'ID de l'entreprise du demandeur.
        /// Cet attribut ne peut être défini uniquement si l'option "Multiple Companies" est activée.
        /// </summary>
        [JsonProperty("company_id")]
        public int? CompanyId { get; set; }
    }
}
