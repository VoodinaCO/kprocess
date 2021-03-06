//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kprocess.KL2.Models
{
    using System;
    using System.Collections.Generic;
    using TrackableEntities;
    
    public partial class PublishedActionCategory : ITrackable, IMergeable
    {
        public PublishedActionCategory()
        {
            this.PublishedActions = new List<PublishedAction>();
        }
    
        public int PublishedActionCategoryId { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string ActionTypeCode { get; set; }
        public string ActionValueCode { get; set; }
        public string FileHash { get; set; }
    
        public ActionType ActionType { get; set; }
        public ActionValue ActionValue { get; set; }
        public ICollection<PublishedAction> PublishedActions { get; set; }
        public PublishedFile File { get; set; }
    
        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
        public Guid EntityIdentifier { get; set; }
    }
}
