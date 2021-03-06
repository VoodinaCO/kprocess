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
    
    public partial class ActionType : ITrackable, IMergeable
    {
        public ActionType()
        {
            this.ActionsReduced = new List<KActionReduced>();
            this.ActionCategories = new List<ActionCategory>();
            this.PublishedActionCategory = new List<PublishedActionCategory>();
        }
    
        public string ActionTypeCode { get; set; }
        public int ShortLabelResourceId { get; set; }
        public int LongLabelResourceId { get; set; }
        public byte[] RowVersion { get; set; }
    
        public ICollection<KActionReduced> ActionsReduced { get; set; }
        public ICollection<ActionCategory> ActionCategories { get; set; }
        public AppResourceKey LongLabelResource { get; set; }
        public AppResourceKey ShortLabelResource { get; set; }
        public ICollection<PublishedActionCategory> PublishedActionCategory { get; set; }
    
        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
        public Guid EntityIdentifier { get; set; }
    }
}
