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
    
    public partial class AppResourceKey : ITrackable, IMergeable
    {
        public AppResourceKey()
        {
            this.ActionTypesForLongLabel = new List<ActionType>();
            this.ActionTypesForShortLabel = new List<ActionType>();
            this.ActionValuesForLongLabel = new List<ActionValue>();
            this.ActionValuesForShortLabel = new List<ActionValue>();
            this.AppResourceValues = new List<AppResourceValue>();
            this.ObjectivesForLongLabel = new List<Objective>();
            this.ObjectivesForShortLabel = new List<Objective>();
            this.RolesForLongLabel = new List<Role>();
            this.RolesForShortLabel = new List<Role>();
            this.ScenarioNaturesForLongLabel = new List<ScenarioNature>();
            this.ScenarioNaturesForShortLabel = new List<ScenarioNature>();
            this.ScenarioStatesForLongLabel = new List<ScenarioState>();
            this.ScenarioStatesForShortLabel = new List<ScenarioState>();
            this.VideoNaturesForLongLabel = new List<VideoNature>();
            this.VideoNaturesForShortLabel = new List<VideoNature>();
        }
    
        public int ResourceId { get; set; }
        public string ResourceKey { get; set; }
    
        public ICollection<ActionType> ActionTypesForLongLabel { get; set; }
        public ICollection<ActionType> ActionTypesForShortLabel { get; set; }
        public ICollection<ActionValue> ActionValuesForLongLabel { get; set; }
        public ICollection<ActionValue> ActionValuesForShortLabel { get; set; }
        public ICollection<AppResourceValue> AppResourceValues { get; set; }
        public ICollection<Objective> ObjectivesForLongLabel { get; set; }
        public ICollection<Objective> ObjectivesForShortLabel { get; set; }
        public ICollection<Role> RolesForLongLabel { get; set; }
        public ICollection<Role> RolesForShortLabel { get; set; }
        public ICollection<ScenarioNature> ScenarioNaturesForLongLabel { get; set; }
        public ICollection<ScenarioNature> ScenarioNaturesForShortLabel { get; set; }
        public ICollection<ScenarioState> ScenarioStatesForLongLabel { get; set; }
        public ICollection<ScenarioState> ScenarioStatesForShortLabel { get; set; }
        public ICollection<VideoNature> VideoNaturesForLongLabel { get; set; }
        public ICollection<VideoNature> VideoNaturesForShortLabel { get; set; }
    
        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
        public Guid EntityIdentifier { get; set; }
    }
}
