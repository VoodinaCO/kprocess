using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Security.Activation;

namespace KProcess.Ksmed.Server.Activation.Tests
{
    class MockLicenseStore : ILicenseStore
    {
        private ProductLicenseInfo _license;

        public ProductLicenseInfo LoadLicense(string productName)
        {
            if (_license == null)
                throw new LicenseNotFoundException("License not found.");
            else
                return _license;
        }

        public UsersPool LoadUsersPoolLicense(string productName)
        {
            throw new NotImplementedException();
        }

        public void SaveLicense(string productName, ProductLicenseInfo licenseInfo)
        {
            _license = licenseInfo;
        }

        public void SaveUserPoolLicense(string productName, UsersPool usersPool)
        {
            throw new NotImplementedException();
        }
    }
}
