using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KProcess.Ksmed.Models
{
    partial class Publication
    {
        public Publication(Scenario s)
        {
            ProcessId = s.Project.ProcessId;
            ProjectId = s.ProjectId;
            ScenarioId = s.ScenarioId;
            Label = s.Project.Process.Label;
            Description = s.Description;
            CriticalPathIDuration = s.CriticalPathIDuration;
            TimeScale = s.Project.TimeScale;
            IsSkill = s.Project.Process.IsSkill;
        }

        [DataMember]
        public Dictionary<int, DateTime> LastMajorReadDates { get; set; } = new Dictionary<int, DateTime>();

        [DataMember]
        public Dictionary<int, DateTime> LastMajorTrainedDates { get; set; } = new Dictionary<int, DateTime>();

        [DataMember]
        public Dictionary<int, DateTime> LastMajorQualifiedDates { get; set; } = new Dictionary<int, DateTime>();

        [DataMember]
        public List<Qualification> LastMajorAncestorQualifications { get; set; } = new List<Qualification>();
        [DataMember]
        public List<Training> LastMajorAncestorTrainings { get; set; } = new List<Training>();
        [DataMember]
        public List<UserReadPublication> LastMajorAncestorReads { get; set; } = new List<UserReadPublication>();

        public bool AuditorHaveActiveAudit { get; set; }

        /// <summary>
        /// Retrieve the full path of the publication
        /// </summary>
        /// <returns></returns>
        public string GetFolderPath()
        {
            var folder = Process.ProjectDir != null ? Process.ProjectDir.Name : "";
            var path = "/" + folder;
            var parent = Process.ProjectDir?.Parent;
            while (parent != null)
            {
                path = "/" + parent.Name + path;
                parent = parent.Parent;
            }
            return path;
        }

        /// <summary>
        /// Retrieve the folder of the publication
        /// </summary>
        /// <returns></returns>
        public string GetFolder()
        {
            var folder = Process.ProjectDir != null ? Process.ProjectDir.Name : "";
            return folder;
        }
    }
}
