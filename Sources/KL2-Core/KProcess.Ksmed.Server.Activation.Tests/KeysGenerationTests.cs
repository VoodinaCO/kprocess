using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Server.Activation.Tests
{
    [TestClass]
    public class KeysGenerationTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void GenerateKeys()
        {
            var results = KeyHelpers.GenerateNewPublicPrivateKeyPair();

            TestContext.WriteLine("Public:\r\n{0}", results.Item1);
            TestContext.WriteLine("Private:\r\n{0}", results.Item2);
        }

    }
}
