using System;
using System.Runtime.Serialization;

namespace KProcess.Ksmed.Security.Activation
{
    [DataContract]
    public class ProductLicenseInfo
    {
        [DataMember]
        public string ActivationInfo { get; set; }
        [DataMember]
        public string Signature { get; set; }
    }
}
