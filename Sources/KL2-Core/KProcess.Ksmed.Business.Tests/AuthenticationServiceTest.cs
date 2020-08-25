using System;
using System.Linq;
using KProcess.Ksmed.Business.Impl;
using KProcess.Ksmed.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KProcess.Ksmed.Business.Tests;

namespace Business.Tests
{


    /// <summary>
    ///This is a test class for AuthenticationServiceTest and is intended
    ///to contain all AuthenticationServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AuthenticationServiceTest
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
        ///A test for IsUserValid
        ///</summary>
        [TestMethod()]
        public void IsUserValidTest()
        {
            SampleData.ClearDatabaseThenImportDefaultProject();

            AuthenticationService target = new AuthenticationService();

            string username = "rboyer";
            byte[] password = new System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(
                System.Text.Encoding.Default.GetBytes("pass"));

            Assert.IsTrue(target.IsUserValid(username, password));
        }

        /// <summary>
        ///A test for GetUser
        ///</summary>
        [TestMethod()]
        public void GetUserTest()
        {
            SampleData.ClearDatabaseThenImportDefaultProject();
            AuthenticationService target = new AuthenticationService();

            var mre = new System.Threading.ManualResetEvent(false);
            User user = null;
            target.GetUser("admin", u =>
                {
                    user = u;
                    mre.Set();
                }, null);

            mre.WaitOne();

            Assert.IsNotNull(user);
            Assert.IsNotNull(user.Username);
            Assert.IsNotNull(user.Password);
            Assert.IsNotNull(user.Firstname);

            Assert.IsNotNull(user.Roles);
            var firstRole = user.Roles.First();
            Assert.IsNotNull(firstRole.ShortLabel);
            Assert.IsNotNull(firstRole.LongLabel);

            Assert.IsNotNull(user.Firstname);
        }
    }
}
