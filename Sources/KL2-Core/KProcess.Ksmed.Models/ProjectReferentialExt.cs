using System.Runtime.Serialization;

namespace KProcess.Ksmed.Models
{
    partial class ProjectReferential
    {

        string _label;
        /// <summary>
        /// Obtient ou définit le libellé.
        /// </summary>
        [DataMember]
        public string Label
        {
            get { return _label; }
            set
            {
                if (_label != value)
                {
                    _label = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
