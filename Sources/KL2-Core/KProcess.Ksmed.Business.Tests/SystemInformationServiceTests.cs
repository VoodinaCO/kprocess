using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KProcess.Ksmed.Business.Tests
{
    [TestClass]
    public class SystemInformationServiceTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void GetBasicInformationTests()
        {
            var info = new SystemInformationService().GetBasicInformation();

            Assert.IsNotNull(info.MachineName);
            Assert.IsNotNull(info.OperatingSystem);
            Assert.IsNotNull(info.OperatingSystemArchitecture);
            Assert.IsNotNull(info.OperatingSystemVersion);
            Assert.IsNotNull(info.OperatingSystemLanguage);
            Assert.IsNotNull(info.Manufacturer);
            Assert.IsNotNull(info.Model);

            Assert.IsTrue(info.Processors.Length > 0);
            foreach (var p in info.Processors)
            {
                Assert.IsNotNull(p);
            }

            Assert.AreNotEqual(0, info.Memory);
            Assert.AreNotEqual(0, info.MachineName);

            Assert.IsTrue(info.VideoControllers.Length > 0);
            foreach (var vc in info.VideoControllers)
                Assert.IsNotNull(vc.Name);
        }
    }
}
