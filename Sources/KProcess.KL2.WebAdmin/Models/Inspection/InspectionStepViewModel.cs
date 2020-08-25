using KProcess.KL2.WebAdmin.Models.Action;
using System;
using System.Collections.Generic;

namespace KProcess.KL2.WebAdmin.Models.Inspection
{
    public class InspectionStepViewModel
    {
        public int Id { get; set; }
        public string Wbs { get; set; }

        public DateTime Date { get; set; }
        public GenericActionViewModel Action { get; set; }
        public int InspectorId { get; set; }
        public string Inspector { get; set; }
        public bool? IsOK { get; set; }
        public string Comment { get; set; }
        public int? AnomalyId { get; set; }
        public string TypeLabel { get; set; }
        public string Description { get; set; }

        public string InspectionDate { get; set; }
        public string ActionLabel { get; set; }
        public List<string> Teams { get; set; }
        public string ThumbnailHash { get; set; }
        public string ThumbnailExt { get; set; }
        public bool HasThumbnail{ get; set; }

        public int Level { get; set; }
        public bool? IsParent { get; set; }
        public bool ChildTask { get; set; }
        public bool IsKeyTask { get; set; }
        
        public string colorCondition { get; set; }
    }
}