using System;
using System.IO;
using System.Xml.Serialization;

namespace KProcess.Ksmed.Security.Activation.Providers
{
    class FileLicenseStore : ILicenseStore
    {
        public ProductLicenseInfo LoadLicense(string productName)
        {
            if (File.Exists(productName))
            {
                using (StreamReader reader = new StreamReader(productName))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ProductLicenseInfo));
                    return (ProductLicenseInfo)serializer.Deserialize(reader);
                }
            }
            else
            {
                throw new LicenseNotFoundException("License not found.");
            }
        }

        public UsersPool LoadUsersPoolLicense(string productName)
        {
            throw new NotImplementedException();
        }

        public void SaveLicense(string productName, ProductLicenseInfo licenseInfo)
        {
            using (StreamWriter writer = new StreamWriter(productName, false))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ProductLicenseInfo));
                serializer.Serialize(writer, licenseInfo);
            }
        }

        public void SaveUserPoolLicense(string productName, UsersPool usersPool)
        {
            throw new NotImplementedException();
        }
    }
}
