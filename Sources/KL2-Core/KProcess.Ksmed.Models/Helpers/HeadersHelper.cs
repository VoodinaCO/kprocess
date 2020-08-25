namespace KProcess.Ksmed.Models
{
    public static class HeadersHelper
    {
        #region Formation

        public const string FormationDate = nameof(PublishedAction.FormationDate);
        public const string TrainedBy = nameof(PublishedAction.TrainedBy);

        #endregion

        #region Evaluation

        public const string IsQualified = nameof(PublishedAction.IsQualified);
        public const string IsNotQualified = "IsNotQualified";
        public const string QualificationStep_Comment = nameof(QualificationStep) + "." + nameof(QualificationStep.Comment);

        #endregion

        #region Inspection

        public const string IsOk = nameof(PublishedAction.IsOk);
        public const string IsNotOk = "IsNotOk";
        public const string InspectionStep_Anomaly_Type = nameof(InspectionStep) + "." + nameof(InspectionStep.Anomaly) + "." + nameof(Anomaly.Type);

        #endregion
    }
}
