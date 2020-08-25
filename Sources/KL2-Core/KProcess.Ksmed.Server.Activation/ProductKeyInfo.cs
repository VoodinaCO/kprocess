using System;

namespace KProcess.Ksmed.Server.Activation
{
    public class ProductKeyInfo
    {
        public short ProductID { get; set; }
        public short ProductFeatures { get; set; }
        public short TrialDays { get; set; }
        public DateTime GeneratedDate { get; set; }

        public string ClientID { get; set; }
        public string UsernameHash { get; set; }
        public string CompanyHash { get; set; }
        public string UserEmailHash { get; set; }
    }

    public class WebProductKeyInfo
    {
        public short ProductID { get; set; }
        public short TrialDays { get; set; }
        public short NumberOfUsers { get; set; }
        public DateTime GeneratedDate { get; set; }

        public string UsernameHash { get; set; }
        public string CompanyHash { get; set; }
        public string UserEmailHash { get; set; }
    }
}
