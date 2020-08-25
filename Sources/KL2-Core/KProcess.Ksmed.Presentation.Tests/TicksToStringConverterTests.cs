using KProcess.Ksmed.Presentation.Core.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.Tests
{
    [TestClass]
    public class TicksToStringConverterTests
    {
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            ServicesHelper.RegisterMockServices();
        }

        [TestMethod]
        public void ConvertBackTests()
        {
            var converter = new TicksToStringConverter();

            Assert.AreEqual(new TimeSpan(0, 00, 00, 00).Ticks, converter.ConvertBack("", null, null, null));
            Assert.AreEqual(new TimeSpan(0, 00, 00, 00).Ticks, converter.ConvertBack(null, null, null, null));

            Assert.AreEqual(new TimeSpan(13, 00, 00, 00).Ticks, converter.ConvertBack("13", null, null, null));
            Assert.AreEqual(new TimeSpan(0, 13, 46, 00).Ticks, converter.ConvertBack("13:46", null, null, null));
            Assert.AreEqual(new TimeSpan(0, 13, 46, 55).Ticks, converter.ConvertBack("13:46:55", null, null, null));
            Assert.AreEqual(new TimeSpan(0, 13, 46, 55, 456).Ticks, converter.ConvertBack("13:46:55.456", null, null, null));

            Assert.AreEqual(new TimeSpan(2, 12, 46, 55, 456).Ticks, converter.ConvertBack("60:46:55.456", null, null, null));
        }

    }
}
