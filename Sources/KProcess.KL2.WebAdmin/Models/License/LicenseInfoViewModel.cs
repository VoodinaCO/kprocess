using KProcess.KL2.WebAdmin.Models.Users;
using KProcess.Ksmed.Security.Activation;
using System;
using System.Collections.Generic;

namespace KProcess.KL2.WebAdmin.Models.License
{
    public class LicenseInfoViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string IDMachine { get; set; }
        public WebLicenseStatus Status { get; set; }
        public string StatusReason { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string LicenseInformation { get; set; }

        public string EmailTarget { get; set; }
        public string EmailBody { get; set; }
        public string CopyBodyText { get; set; }

        public int LicenseMaxActivateUser { get; set; }
        public int LicenseTotalActivatedUsers { get; set; }
        public List<UserViewModel> ActivatedUsers { get; set; }

        public WebProductLicense LicenseFile { get; set; }
    }
}