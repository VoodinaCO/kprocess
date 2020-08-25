namespace KProcess.KL2.WebAdmin.Models.Documentation
{
    public class SummaryViewModel
    {
        public int ScenarioId { get; set; }
        public int ProcessId { get; set; }
        public bool PublishForTraining { get; set; }
        public bool PublishForInspection{ get; set; }
        public bool PublishForEvaluation { get; set; }
        public string TrainingReleaseNote { get; set; }
        public string InspectionReleaseNote { get; set; }
        public string EvaluationReleaseNote { get; set; }
        public bool TrainingVersioningIsMajor { get; set; }
        public bool InspectionVersioningIsMajor { get; set; }
        public bool EvaluationVersioningIsMajor { get; set; }
    }
}