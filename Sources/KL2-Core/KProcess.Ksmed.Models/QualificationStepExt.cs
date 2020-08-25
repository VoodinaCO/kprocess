using KProcess.Ksmed.Models.Validation;
using System.Collections.Generic;

namespace KProcess.Ksmed.Models
{
    partial class QualificationStep
    {
        public List<QualificationStep> Childs { get; set;}      

        public int Level { get; set; }
        public bool? IsParent { get; set; }

        public bool _isEditableComment;
        public bool IsEditableComment
        {
            get => _isEditableComment;
            set
            {
                if (_isEditableComment != value)
                {
                    _isEditableComment = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
