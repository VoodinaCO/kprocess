using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using KProcess.Ksmed.Security.Activation;
using KProcess.Ksmed.Security.Activation.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KProcess.Ksmed.Server.Activation.Tests
{
    [TestClass]
    public class ProductLicenseManagerTests
    {

        [TestMethod]
        public void ActivateProduct()
        {
            string productKey = GenerateProductKey(123, 456, 0, "C", "toto", "Company", "tott@email.com");
            var licenseActivated = GetProductActivated(productKey);

            string publicXmlKey = KeyHelpers.GetPrivateKey();
            ProductLicenseManager licenseManager = new ProductLicenseManager(null, publicXmlKey,
                new MockLicenseStore(),
                new MachineIdentifierProviderMock(),
                new UserInformationProviderMock()
                {
                    Username = "toto",
                    Company = "Company",
                    Email = "tott@email.com"
                });
            ProductLicense license = licenseManager.ActivateProduct(licenseActivated);

            Assert.AreEqual(LicenseStatus.Licensed, license.Status, license.StatusReason);
            Assert.AreEqual(123, license.ProductID);
            Assert.AreEqual(456, license.ProductFeatures);
            Assert.AreEqual(0, license.TrialDays);
        }

        [TestMethod]
        public void ActivateTrialProduct()
        {
            string productKey = GenerateProductKey(123, 456, 30, "C", "toto", "Company", "tott@email.com");
            var licenseActivated = GetProductActivated(productKey);

            string publicXmlKey = KeyHelpers.GetPrivateKey();
            ProductLicenseManager licenseManager = new ProductLicenseManager(null, publicXmlKey,
                new MockLicenseStore(),
                new MachineIdentifierProviderMock(),
                new UserInformationProviderMock()
                {
                    Username = "toto",
                    Company = "Company",
                    Email = "tott@email.com"
                });
            ProductLicense license = licenseManager.ActivateProduct(licenseActivated);

            Assert.AreEqual(LicenseStatus.TrialVersion, license.Status, license.StatusReason);
            Assert.AreEqual(123, license.ProductID);
            Assert.AreEqual(456, license.ProductFeatures);
            Assert.AreEqual(30, license.TrialDays);
            Assert.AreEqual(30, license.TrialDaysLeft);
        }

        [TestMethod]
        public void ActivateProductWrongUserInfo()
        {
            string productKey = GenerateProductKey(123, 456, 0, "C", "toto", "Company", "tott@email.com");
            var licenseActivated = GetProductActivated(productKey);

            string publicXmlKey = KeyHelpers.GetPrivateKey();
            ProductLicenseManager licenseManager = new ProductLicenseManager(null, publicXmlKey,
                new MockLicenseStore(),
                new MachineIdentifierProviderMock(),
                new UserInformationProviderMock()
                {
                    Username = "toto!!!",
                    Company = "Company",
                    Email = "tott@email.com"
                });
            ProductLicense license = licenseManager.ActivateProduct(licenseActivated);

            Assert.AreEqual(LicenseStatus.Invalid, license.Status, license.StatusReason);
        }

        [TestMethod]
        public void ActivateExpiredProduct()
        {
            string productKey = GenerateProductKey(123, 456, -1, "C", "toto", "Company", "tott@email.com");
            var licenseActivated = GetProductActivated(productKey);

            string publicXmlKey = KeyHelpers.GetPrivateKey();
            ProductLicenseManager licenseManager = new ProductLicenseManager(null, publicXmlKey,
                new MockLicenseStore(),
                new MachineIdentifierProviderMock(),
                new UserInformationProviderMock()
                {
                    Username = "toto",
                    Company = "Company",
                    Email = "tott@email.com"
                });
            ProductLicense license = licenseManager.ActivateProduct(licenseActivated);

            Assert.AreEqual(LicenseStatus.Expired, license.Status, license.StatusReason);
            Assert.AreEqual(123, license.ProductID);
            Assert.AreEqual(456, license.ProductFeatures);
            Assert.AreEqual(-1, license.TrialDays);
            Assert.AreEqual(-1, license.TrialDaysLeft);
        }

        [TestMethod]
        public void ActivateProductMachineMismatch()
        {
            MachineIdentifierProviderMock machineMock = new MachineIdentifierProviderMock(false);
            string productKey = GenerateProductKey(123, 456, 0, "C", "toto", "Company", "tott@email.com");
            var licenseActivated = GetProductActivated(productKey);

            string publicXmlKey = KeyHelpers.GetPrivateKey();
            ProductLicenseManager licenseManager = new ProductLicenseManager(null, publicXmlKey, null, machineMock,
                new UserInformationProviderMock()
                {
                    Username = "toto",
                    Company = "Company",
                    Email = "tott@email.com"
                });
            ProductLicense license = licenseManager.ActivateProduct(licenseActivated);

            Assert.AreEqual(LicenseStatus.MachineHashMismatch, license.Status, license.StatusReason);
        }

        [TestMethod]
        public void LoadSaveLicense()
        {
            string publicXmlKey = KeyHelpers.GetPrivateKey();
            ProductLicenseManager licenseManager = new ProductLicenseManager(null, publicXmlKey,
                new MockLicenseStore(),
                new MachineIdentifierProviderMock(),
                new UserInformationProviderMock()
                {
                    Username = "toto",
                    Company = "Company",
                    Email = "tott@email.com"
                });
            ProductLicense license = licenseManager.LoadLicense("MyProductName");

            string productKey = GenerateProductKey(123, 456, 0, "C", "toto", "Company", "tott@email.com");
            var licenseActivated = GetProductActivated(productKey);

            Assert.AreEqual(LicenseStatus.NotFound, license.Status, license.StatusReason);

            license = licenseManager.ActivateProduct(licenseActivated);
            licenseManager.SaveLicense("MyProductName", license);
            license = licenseManager.LoadLicense("MyProductName");

            Assert.AreEqual(LicenseStatus.Licensed, license.Status, license.StatusReason);
            Assert.AreEqual(123, license.ProductID);
            Assert.AreEqual(456, license.ProductFeatures);
            Assert.AreEqual(0, license.TrialDays);
        }

        private string GenerateProductKey(short productID, short productFeatures, short trialDays, string clientID, string username, string company, string email)
        {
            string privateXmlKey = KeyHelpers.GetPrivateKey();
            ProductKeyPublisher keyPublisher = new ProductKeyPublisher(privateXmlKey);

            return keyPublisher.GenerateProductKey(productID, productFeatures, trialDays, clientID, username, company, email);
        }

        private ProductLicenseInfo GetProductActivated(string productKey)
        {
            string privateXmlKey = KeyHelpers.GetPrivateKey();
            return new ProductActivation(privateXmlKey).ActivateProduct(productKey, Convert.ToBase64String(new MachineIdentifierProviderMock().MachineHash));
        }

        [TestMethod]
        public void LoadSaveLicenseRealHash()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("fr-FR");
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("fr-FR");

            string publicXmlKey = KeyHelpers.GetPrivateKey();
            ProductLicenseManager licenseManager = new ProductLicenseManager(null, publicXmlKey,
                new MockLicenseStore(),
                new MachineIdentifierProvider(null, new Security.Activation.MachineIdentifiers.VolumeInfoIdentifier(null)),
                new UserInformationProviderMock()
                {
                    Username = "toto",
                    Company = "Company",
                    Email = "tott@email.com"
                });

            var hash = licenseManager.GetMachineHash();

            ProductLicense license = licenseManager.LoadLicense("MyProductName");

            string productKey = GenerateProductKey(123, 456, 0, "C", "toto", "Company", "tott@email.com");
            var licenseActivated = GetProductActivatedRealHash(productKey, hash);

            try
            {
                using (StreamWriter writer = new StreamWriter("out.ksk", false))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ProductLicenseInfo));
                    serializer.Serialize(writer, licenseActivated);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
                return;
            }

            Assert.AreEqual(LicenseStatus.NotFound, license.Status, license.StatusReason);

            KProcess.Ksmed.Security.Activation.ProductLicenseInfo licenseInfo = null;
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader("out.ksk"))
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(KProcess.Ksmed.Security.Activation.ProductLicenseInfo));
                    licenseInfo = (KProcess.Ksmed.Security.Activation.ProductLicenseInfo)serializer.Deserialize(reader);
                }
                license = licenseManager.ActivateProduct(licenseInfo);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }

            license = licenseManager.ActivateProduct(licenseInfo);
            licenseManager.SaveLicense("MyProductName", license);
            license = licenseManager.LoadLicense("MyProductName");

            Assert.AreEqual(LicenseStatus.Licensed, license.Status, license.StatusReason);
            Assert.AreEqual(123, license.ProductID);
            Assert.AreEqual(456, license.ProductFeatures);
            Assert.AreEqual(0, license.TrialDays);
        }

        private ProductLicenseInfo GetProductActivatedRealHash(string productKey, string hash)
        {
            string privateXmlKey = KeyHelpers.GetPrivateKey();
            return new ProductActivation(privateXmlKey).ActivateProduct(productKey, hash);
        }

    }
}
