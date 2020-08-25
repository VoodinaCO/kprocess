using KProcess.Ksmed.Security.Activation;
using System.Text;

namespace KProcess.KL2.WebAdmin.Models.License
{
    public class LicenseViewModel
    {
        public LicenseInfoViewModel LicenseInfo { get; set; }

        public WebProductLicense LicenseFile { get; set; }

        public double LicenseFileSize =>
            LicenseFile == null ? 0 : Encoding.UTF8.GetBytes(LicenseFile.LicenseInfo.Signature).Length;
    }
}