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
    
    public partial class Solution : ITrackable, IMergeable
    {
        public int SolutionId { get; set; }
        public int ScenarioId { get; set; }
        public string SolutionDescription { get; set; }
        public bool Approved { get; set; }
        public Nullable<short> Cost { get; set; }
        public Nullable<short> Difficulty { get; set; }
        public Nullable<double> Investment { get; set; }
        public string Comments { get; set; }
        public bool IsEmpty { get; set; }
        public string Who { get; set; }
        public Nullable<System.DateTime> When { get; set; }
        public decimal P { get; set; }
        public decimal D { get; set; }
        public decimal C { get; set; }
        public decimal A { get; set; }
        public byte[] RowVersion { get; set; }
    
        public Scenario Scenario { get; set; }
    
        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
        public Guid EntityIdentifier { get; set; }
    }
}
