using System;
using KProcess.Ksmed.Security.Activation;
using KProcess.Ksmed.Server.Activation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KProcess.Ksmed.Server.Activation.Tests
{


    /// <summary>
    ///This is a test class for ProductActivationTest and is intended
    ///to contain all ProductActivationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ProductActivationTest
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


        /// <summary>
        ///A test for ActivateProduct
        ///</summary>
        [TestMethod()]
        public void ActivateProductTest()
        {
            string privateXmlKey = KeyHelpers.GetPrivateKey();
            ProductKeyPublisher keyPublisher = new ProductKeyPublisher(privateXmlKey);

            string productKey = keyPublisher.GenerateProductKey(123, 456, 789, "C", "toto", "Company", "tott@email.com");

            var activator = new ProductActivation(privateXmlKey);
            var licenseInfo = activator.ActivateProduct(productKey, Convert.ToBase64String(new MachineIdentifierProviderMock().MachineHash));

            Assert.IsNotNull(licenseInfo);
            Assert.IsNotNull(licenseInfo.ActivationInfo);
            Assert.IsNotNull(licenseInfo.Signature);

        }
    }
}
