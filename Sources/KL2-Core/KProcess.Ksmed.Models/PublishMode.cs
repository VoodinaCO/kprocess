using KProcess.Business;
using KProcess.KL2.Languages;

namespace KProcess.Ksmed.Models
{
    public class PublishMode : ModelBase
    {
        public PublishModeEnum Id { get; set; }
        public string LabelResourceId { get; set; }
        public bool IsEnabled { get; set; }

        bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Label =>
            IoC.Resolve<ILocalizationManager>().GetString(LabelResourceId);
    }
}
