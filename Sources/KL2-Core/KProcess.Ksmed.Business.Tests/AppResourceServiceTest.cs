using System;
using System.Collections.Generic;
using System.Linq;
using KProcess.Ksmed.Business.Impl;
using KProcess.Ksmed.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Business.Tests
{
    /// <summary>
    ///This is a test class for AppResourceServiceTest and is intended
    ///to contain all AppResourceServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AppResourceServiceTest
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
        ///A test for GetResources
        ///</summary>
        [TestMethod()]
        public void GetResourcesTest()
        {
            AppResourceService target = new AppResourceService();
            string languageCode = "fr-FR";

            var actual = target.GetResources(languageCode);

            Assert.IsTrue(actual.Length > 0);

            var first = actual.First();
            Assert.IsNotNull(first.AppResourceKey);
            Assert.IsNotNull(first.AppResourceKey.ResourceId);
            Assert.IsNotNull(first.AppResourceKey.ResourceKey);

            Assert.IsNotNull(first.LanguageCode);
            Assert.IsNotNull(first.Value);

        }
    }
}
