using System.Runtime.Serialization;

namespace KProcess.Ksmed.Models
{
    [DataContract(IsReference = true, Namespace = ModelsConstants.DataContractNamespace)]
    public class RefsCollection : NotifiableObject
    {
        string _field;
        [DataMember]
        public string Field
        {
            get { return _field; }
            set
            {
                if (_field != value)
                {
                    _field = value;
                    OnPropertyChanged();
                }
            }
        }

        string _label;
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

        TrackableCollection<PublishedReferentialAction> _values;
        public TrackableCollection<PublishedReferentialAction> Values
        {
            get { return _values; }
            set
            {
                if (_values != value)
                {
                    _values = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
