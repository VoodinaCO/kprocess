using KProcess.KL2.WebAdmin.Models.Action;
using KProcess.Ksmed.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace KProcess.KL2.WebAdmin.Models.Documentation
{
    public class FormatActionsViewModel
    {
        public int ProcessId { get; set; }
        public int ScenarioId { get; set; }
        public List<FormatActionsElementViewModel> TrainingReferentials { get; set; }
        public List<FormatActionsElementViewModel> InspectionReferentials { get; set; }
        public List<FormatActionsElementViewModel> EvaluationReferentials { get; set; }

        public bool PublishForTraining { get; set; }
        public bool PublishForInspection { get; set; }
        public bool PublishForEvaluation { get; set; }
    }

    public class FormatActionsElementViewModel
    {
        public string Text { get; set; }
        public string MappingName { get; set; }
        public ReferentialCategory Category { get; set; }
        public int CategoryKey { get; set; }
        public string TranslatedCategory { get; set; }
        public bool IsChecked { get; set; }
    }

    public class FormatActionsElementValue
    {
        public string Label { get; set; }
        public ReferentialCategory Category { get; set; }
    }
}