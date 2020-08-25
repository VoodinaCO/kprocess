using KProcess.Ksmed.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KProcess.KL2.WebAdmin.Models.Inspection
{
    [Serializable]
    public class AnomalyViewModel
    {
        public int AnomalyId { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }
        public string RawPhoto { get; set; }
        public bool HasPhoto { get; set; }
        public int InspectionId { get; set; }
        public string ProcessLabel { get; set; }
        public int InspectorId { get; set; }
        public string InspectorName { get; set; }
        public DateTime Date { get; set; }
        public int Type { get; set; }
        public string TypeLabel { get; set; }
        public string TypeColor { get; set; }
        public string Line { get; set; }
        public string Machine { get; set; }
        public int Priority { get; set; }
        public string Label { get; set; }
        public string Category { get; set; }
        public string AnomalyType { get; set; }

        public bool HasThumbnail { get; set; }
        public string ThumbnailHash { get; set; }
        public string ThumbnailExt { get; set; }

        //1 : Standard
        //2 : Non standard
        public int AnomalyTypeIdentifier { get; set; }

        public List<Tuple<int, string>> PriorityLists { get; set; } 
        public List<IAnomalyKindItem> KindItems { get; set; }
        public List<AnomalyKindItemViewModel> Items { get; set; }
    }
    [Serializable]
    public class AnomalyKindItemViewModel
    {
        public string Label { get; set; }
        public string Category { get; set; }
        public int Number { get; set; }
    }
}