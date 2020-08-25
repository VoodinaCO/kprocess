using System.Runtime.Serialization;

namespace KProcess.Ksmed.Models
{
    [DataContract(IsReference = true, Namespace = ModelsConstants.DataContractNamespace)]
    public class CustomLabel : NotifiableObject
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

        string _Value;
        [DataMember]
        public string Value
        {
            get { return _Value; }
            set
            {
                if (_Value != value)
                {
                    _Value = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
