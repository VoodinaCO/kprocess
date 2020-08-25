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
    
    public partial class Project : ITrackable, IMergeable
    {
        public Project()
        {
            this.ProjectChilds = new List<Project>();
            this.Referentials = new List<ProjectReferential>();
            this.Scenarios = new List<Scenario>();
            this.UserRoleProjects = new List<UserRoleProject>();
            this.Videos = new List<Video>();
            this.Publications = new List<Publication>();
        }
    
        public int ProjectId { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string ObjectiveCode { get; set; }
        public string OtherObjectiveLabel { get; set; }
        public string CustomTextLabel { get; set; }
        public string CustomTextLabel2 { get; set; }
        public string CustomTextLabel3 { get; set; }
        public string CustomTextLabel4 { get; set; }
        public string CustomNumericLabel { get; set; }
        public string CustomNumericLabel2 { get; set; }
        public string CustomNumericLabel3 { get; set; }
        public string CustomNumericLabel4 { get; set; }
        public long TimeScale { get; set; }
        public int CreatedByUserId { get; set; }
        public System.DateTime CreationDate { get; set; }
        public int ModifiedByUserId { get; set; }
        public System.DateTime LastModificationDate { get; set; }
        public byte[] RowVersion { get; set; }
        public Nullable<int> ParentProjectId { get; set; }
        public int ProcessId { get; set; }
        public string Workshop { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> ForecastEndDate { get; set; }
        public Nullable<System.DateTime> RealEndDate { get; set; }
    
        public Objective Objective { get; set; }
        public ICollection<Project> ProjectChilds { get; set; }
        public Project ProjectParent { get; set; }
        public User Creator { get; set; }
        public User LastModifier { get; set; }
        public ICollection<ProjectReferential> Referentials { get; set; }
        public Procedure Process { get; set; }
        public ICollection<Scenario> Scenarios { get; set; }
        public ICollection<UserRoleProject> UserRoleProjects { get; set; }
        public ICollection<Video> Videos { get; set; }
        public ICollection<Publication> Publications { get; set; }
    
        public TrackingState TrackingState { get; set; }
        public ICollection<string> ModifiedProperties { get; set; }
        public Guid EntityIdentifier { get; set; }
    }
}