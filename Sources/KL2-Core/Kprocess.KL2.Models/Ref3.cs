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
    
    public partial class Ref3 : ITrackable, IMergeable
    {
        public Ref3()
        {
            this.Ref3Actions = new List<Ref3Action>();
        }
    
        public int RefId { get; set; }
        public string Label { get; set; }
        public string Color { get; set; }
        public string Uri { get; set; }
        public string Description { get; set; }
        public int CreatedByUserId { get; set; }
        public System.DateTime CreationDate { get; set; }
        public int ModifiedByUserId { get; set; }
        public System.DateTime LastModificationDate { get; set; }
        public byte[] RowVersion { get; set; }
        public Nullable<int> ProcessId { get; set; }
    
        public User Creator { get; set; }
        public User LastModifier { get; set; }
        public ICollection<Ref3Action> Ref3Actions { get; set; }
        public Procedure Process { get; set; }
    
        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
        public Guid EntityIdentifier { get; set; }
    }
}
