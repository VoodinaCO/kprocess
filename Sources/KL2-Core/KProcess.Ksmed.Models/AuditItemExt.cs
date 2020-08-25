namespace KProcess.Ksmed.Models
{
    partial class AuditItem
    {
        SurveyItem _surveyItem;
        /// <summary>
        /// Obtient ou définit si l'action est lié à un mode opératoire.
        /// </summary>
        public SurveyItem SurveyItem
        {
            get { return _surveyItem; }
            set
            {
                if (_surveyItem != value)
                {
                    _surveyItem = value;
                    OnPropertyChanged();
                }
            }
        }

        public AuditItem(SurveyItem surveyItem)
        {
            _surveyItem = surveyItem;
            Number = _surveyItem.Number;
        }
    }
}
