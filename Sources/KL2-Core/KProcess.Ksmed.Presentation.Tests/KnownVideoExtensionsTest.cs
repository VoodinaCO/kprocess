using System;
using KProcess.Globalization;
using KProcess.Ksmed.Presentation.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KProcess.Ksmed.Presentation.Tests
{


    /// <summary>
    ///This is a test class for KnownVideoExtensionsTest and is intended
    ///to contain all KnownVideoExtensionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class KnownVideoExtensionsTest
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


        [TestMethod]
        public void GetFileDialogFilterTest()
        {
            LocalizationManager.ResourceProvider = new KProcess.Presentation.Windows.DesignTime.DesignTimeResourceProvider();

            Assert.IsNotNull(FileExtensionsDialogHelper.GetVideosFileDialogFilter());
        }
    }
}
