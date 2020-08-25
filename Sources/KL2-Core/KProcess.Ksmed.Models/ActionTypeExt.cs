namespace KProcess.Ksmed.Models
{
    partial class ActionType
    {

        string _contextualLabel;
        /// <summary>
        /// Obtient ou définit le libellé contextuel.
        /// </summary>
        public string ContextualLabel
        {
            get { return _contextualLabel; }
            set
            {
                if (_contextualLabel != value)
                {
                    _contextualLabel = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
