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
    
    public partial class ProjectDir : ITrackable, IMergeable
    {
        public ProjectDir()
        {
            this.Childs = new List<ProjectDir>();
            this.Processes = new List<Procedure>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> ParentId { get; set; }
        public byte[] RowVersion { get; set; }
    
        public ICollection<ProjectDir> Childs { get; set; }
        public ProjectDir Parent { get; set; }
        public ICollection<Procedure> Processes { get; set; }
    
        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
        public Guid EntityIdentifier { get; set; }
    }
}