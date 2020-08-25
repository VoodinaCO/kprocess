using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FreshDeskLib
{
    /// <summary>
    /// Représente un OutboundMail
    /// </summary>
    public class OutboundMail : SerializableObject, IAttachments
    {
        /// <summary>
        /// Obtient ou définit le nom du demandeur.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Obtient ou définit l'email du demandeur. Si aucun contact n'existe avec cette adresse dans Freshdesk, il est ajouté en tant que nouveau contact.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Obtient ou définit l'objet du ticket.
        /// La valeur par défaut est null.
        /// </summary>
        [JsonProperty("subject")]
        public string Subject { get; set; } = null;

        /// <summary>
        /// Obtient ou définit le type du ticket. Aide à catégoriser les tickets en fonction des différents types d'issues.
        /// La valeur par défaut est null.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; } = null;

        /// <summary>
        /// Obtient ou définit le statut du ticket.
        /// La valeur par défaut est Open.
        /// </summary>
        [JsonProperty("status")]
        public virtual TicketStatus Status { get; set; } = TicketStatus.Closed;

        /// <summary>
        /// Obtient ou définit la priorité du ticket.
        /// La valeur par défaut est Low.
        /// </summary>
        [JsonProperty("priority")]
        public TicketPriority Priority { get; set; } = TicketPriority.Low;

        /// <summary>
        /// Obtient ou définit le contenu HTML du ticket.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Obtient ou définit les attachements.
        /// La taille totale ne doit pas dépasser 15MB.
        /// </summary>
        [JsonIgnore]
        public IDictionary<string, byte[]> Attachments { get; set; }

        /// <summary>
        /// Obtient ou définit les champs personnalisés.
        /// </summary>
        [JsonProperty("custom_fields")]
        public IDictionary<string, string> CustomFields { get; set; }

        /// <summary>
        /// Obtient ou définit la date à laquelle le ticket est attendu d'être résolu.
        /// </summary>
        [JsonProperty("due_by")]
        public DateTime? ResolvedDueBy { get; set; }

        /// <summary>
        /// Obtient ou définit l'ID du mail de config qui est utilisé pour ce ticket (ex : support@yourcompany.com ou sales@yourcompany.com).
        /// Si <see cref="product_id"/> est défini mais pas email_config_id, email_config_id est défini au premier mail du produit.
        /// </summary>
        [JsonProperty("email_config_id")]
        public long EmailConfigId { get; set; }

        /// <summary>
        /// Obtient ou définit la date à laquelle une première réponse au ticket est attendu.
        /// </summary>
        [JsonProperty("fr_due_by")]
        public DateTime? FirstResponseDueBy { get; set; }

        /// <summary>
        /// Obtient ou définit l'ID du groupe dans lequel a été assigné le ticket.
        /// La valeur par défaut est l'ID du groupe qui est associé à <see cref="email_config_id"/>
        /// </summary>
        [JsonProperty("group_id")]
        public long GroupId { get; set; }

        /// <summary>
        /// Obtient ou définit les mots clé asociés au ticket.
        /// </summary>
        [JsonProperty("tags")]
        public string[] Tags { get; set; }
    }
}
