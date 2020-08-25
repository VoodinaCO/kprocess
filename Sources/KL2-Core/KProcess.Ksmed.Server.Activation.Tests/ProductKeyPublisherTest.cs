using System;
using System.Security.Cryptography;
using System.Text;
using KProcess.Ksmed.Security.Activation;
using KProcess.Ksmed.Server.Activation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KProcess.Ksmed.Server.Activation.Tests
{


    /// <summary>
    ///This is a test class for ProductKeyPublisherTest and is intended
    ///to contain all ProductKeyPublisherTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ProductKeyPublisherTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod()]
        public void GenerateProductKeyTest()
        {
            string privateXmlKey = KeyHelpers.GetPrivateKey();
            ProductKeyPublisher keyPublisher = new ProductKeyPublisher(privateXmlKey);

            string productKey1 = keyPublisher.GenerateProductKey(0, 0, 0, "C", "toto", "Company", "tott@email.com");
            System.Threading.Thread.Sleep(1);
            string productKey2 = keyPublisher.GenerateProductKey(0, 0, 0, "C", "toto", "Company", "tott@email.com");
            System.Threading.Thread.Sleep(1);
            string productKey3 = keyPublisher.GenerateProductKey(0, 0, 0, "C", "toto", "Company", "tott@email.com");
            System.Threading.Thread.Sleep(1);
            string productKey4 = keyPublisher.GenerateProductKey(0, 0, 0, "C", "toto", "Company", "tott@email.com");

            Assert.AreNotEqual(productKey1, productKey2);
            Assert.AreNotEqual(productKey1, productKey3);
            Assert.AreNotEqual(productKey1, productKey4);
            Assert.AreNotEqual(productKey2, productKey3);
            Assert.AreNotEqual(productKey2, productKey4);
            Assert.AreNotEqual(productKey3, productKey4);
        }


        [TestMethod]
        public void ValidateProductKey()
        {
            string privateXmlKey = KeyHelpers.GetPrivateKey();
            ProductKeyPublisher keyPublisher = new ProductKeyPublisher(privateXmlKey);

            string productKey = keyPublisher.GenerateProductKey(123, 456, 789, "C", "toto", "Company", "tott@email.com");
            Console.WriteLine(productKey);

            keyPublisher = new ProductKeyPublisher(privateXmlKey);
            ProductKeyInfo result = keyPublisher.ValidateProductKey(productKey);

            var shaHasher = new SHA1CryptoServiceProvider();
            var usernameHash = Encoding.ASCII.GetString(shaHasher.ComputeHash(Encoding.ASCII.GetBytes("toto")));
            var companyHash = Encoding.ASCII.GetString(shaHasher.ComputeHash(Encoding.ASCII.GetBytes("Company")));
            var userEmailHash = Encoding.ASCII.GetString(shaHasher.ComputeHash(Encoding.ASCII.GetBytes("tott@email.com")));

            Assert.AreEqual(123, result.ProductID);
            Assert.AreEqual(456, result.ProductFeatures);
            Assert.AreEqual(789, result.TrialDays);
            Assert.AreEqual(usernameHash, result.UsernameHash);
            Assert.AreEqual(companyHash, result.CompanyHash);
            Assert.AreEqual(userEmailHash, result.UserEmailHash);
            Assert.AreEqual(DateTime.Now.Date, result.GeneratedDate.Date);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidProductKeyException))]
        public void ValidateFakeProductKey()
        {
            string privateXmlKey = KeyHelpers.GetPrivateKey();
            ProductKeyPublisher keyPublisher = new ProductKeyPublisher(privateXmlKey);

            string productKey = keyPublisher.GenerateProductKey(123, 456, 789, "C", "toto", "Company", "tott@email.com");
            Console.WriteLine(productKey);
            productKey = productKey.Replace('1', '8');
            productKey = productKey.Replace('2', '7');
            productKey = productKey.Replace('3', '6');
            productKey = productKey.Replace('4', '5');
            Console.WriteLine(productKey);

            keyPublisher = new ProductKeyPublisher(privateXmlKey);
            ProductKeyInfo result = keyPublisher.ValidateProductKey(productKey);
        }

        [TestMethod]
        [ExpectedException(typeof(CryptographicException))]
        public void TryValidateWithPublicKey()
        {
            string privateXmlKey = KeyHelpers.GetPrivateKey();
            ProductKeyPublisher keyPublisher = new ProductKeyPublisher(privateXmlKey);

            string productKey = keyPublisher.GenerateProductKey(123, 456, 789, "C", "toto", "Company", "tott@email.com");
            Console.WriteLine(productKey);

            privateXmlKey = KeyHelpers.GetPublicKey();
            keyPublisher = new ProductKeyPublisher(privateXmlKey);
            ProductKeyInfo result = keyPublisher.ValidateProductKey(productKey);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidProductKeyException))]
        public void TryValidateWithFakeCryptProvider()
        {
            string privateXmlKey = KeyHelpers.GetPrivateKey();
            ProductKeyPublisher keyPublisher = new ProductKeyPublisher(privateXmlKey);

            string productKey = keyPublisher.GenerateProductKey(123, 456, 789, "C", "toto", "Company", "tott@email.com");
            Console.WriteLine(productKey);

            privateXmlKey = (new RSACryptoServiceProvider()).ToXmlString(true);
            keyPublisher = new ProductKeyPublisher(privateXmlKey);
            ProductKeyInfo result = keyPublisher.ValidateProductKey(productKey);
        }

        [TestMethod]
        public void TryRandomKeyGeneration()
        {
            string privateXmlKey = KeyHelpers.GetPrivateKey();
            ProductKeyPublisher keyPublisher = new ProductKeyPublisher(privateXmlKey);

            string productKey = keyPublisher.GenerateProductKey(0, 0, 0, "", "", "", "");
            Console.WriteLine(productKey);

            keyPublisher.ValidateProductKey(productKey);

            for (int n = 1; n < 0x100; n++)
            {
                byte[] bytes = BitConverter.GetBytes(n);
                string fakeSegment = Convert.ToBase64String(new byte[] { bytes[2], bytes[1], bytes[0] });
                if (productKey.Length > 6 && fakeSegment != productKey.Substring(6))
                {
                    productKey = String.Format("{0}{1}", fakeSegment, productKey.Substring(5));
                    try
                    {
                        keyPublisher.ValidateProductKey(productKey);
                        Assert.Fail("This should raise an InvalidProductKeyException");
                    }
                    catch (InvalidProductKeyException)
                    {
                        // OK. We are expecting this exception.
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
    }
}
