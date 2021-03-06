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
    
    public partial class Procedure : ITrackable, IMergeable
    {
        public Procedure()
        {
            this.Projects = new List<Project>();
            this.Refs1 = new List<Ref1>();
            this.Refs2 = new List<Ref2>();
            this.Refs3 = new List<Ref3>();
            this.Refs4 = new List<Ref4>();
            this.Refs5 = new List<Ref5>();
            this.Refs6 = new List<Ref6>();
            this.Refs7 = new List<Ref7>();
            this.ActionCategories = new List<ActionCategory>();
            this.Equipments = new List<Equipment>();
            this.Operators = new List<Operator>();
            this.Publications = new List<Publication>();
        }
    
        public int ProcessId { get; set; }
        public string Label { get; set; }
        public Nullable<int> ProjectDirId { get; set; }
        public string Description { get; set; }
    
        public ICollection<Project> Projects { get; set; }
        public ProjectDir ProjectDir { get; set; }
        public ICollection<Ref1> Refs1 { get; set; }
        public ICollection<Ref2> Refs2 { get; set; }
        public ICollection<Ref3> Refs3 { get; set; }
        public ICollection<Ref4> Refs4 { get; set; }
        public ICollection<Ref5> Refs5 { get; set; }
        public ICollection<Ref6> Refs6 { get; set; }
        public ICollection<Ref7> Refs7 { get; set; }
        public ICollection<ActionCategory> ActionCategories { get; set; }
        public ICollection<Equipment> Equipments { get; set; }
        public ICollection<Operator> Operators { get; set; }
        public ICollection<Publication> Publications { get; set; }
    
        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
        public Guid EntityIdentifier { get; set; }
    }
}
