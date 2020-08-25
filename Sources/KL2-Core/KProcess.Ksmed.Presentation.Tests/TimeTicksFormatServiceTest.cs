using System;
using KProcess.Ksmed.Presentation.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KProcess.Ksmed.Presentation.Tests
{


    /// <summary>
    ///This is a test class for TimeTicksFormatServiceTest and is intended
    ///to contain all TimeTicksFormatServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TimeTicksFormatServiceTest
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

        private long Minutes(double m)
        {
            return TimeSpan.FromMinutes(m).Ticks;
        }

        private long Seconds(double s)
        {
            return TimeSpan.FromSeconds(s).Ticks;
        }

        /// <summary>
        ///A test for RoundTime
        ///</summary>
        [TestMethod()]
        public void RoundTimeTest()
        {
            TimeTicksFormatService target = new TimeTicksFormatService(); // TODO: Initialize to an appropriate value

            Assert.AreEqual(0, Math.Round(0.0));
            Assert.AreEqual(Minutes(0), target.RoundTime(Minutes(0), Minutes(1)));

            Assert.AreEqual(1, Math.Round(1.0));
            Assert.AreEqual(Minutes(1), target.RoundTime(Minutes(1), Minutes(1)));
            Assert.AreEqual(1, Math.Round(0.51));
            Assert.AreEqual(Minutes(1), target.RoundTime(Minutes(0.51), Minutes(1)));
            Assert.AreEqual(2, Math.Round(1.5));
            Assert.AreEqual(Minutes(2), target.RoundTime(Minutes(1.5), Minutes(1)));
            Assert.AreEqual(2, Math.Round(1.6));
            Assert.AreEqual(Minutes(2), target.RoundTime(Minutes(1.6), Minutes(1)));


            Assert.AreEqual(-1, Math.Round(-1.0));
            Assert.AreEqual(Minutes(-1), target.RoundTime(Minutes(-1), Minutes(1)));
            Assert.AreEqual(-1, Math.Round(-0.51));
            Assert.AreEqual(Minutes(-1), target.RoundTime(Minutes(-0.51), Minutes(1)));
            Assert.AreEqual(-2, Math.Round(-1.5));
            Assert.AreEqual(Minutes(-2), target.RoundTime(Minutes(-1.5), Minutes(1)));
            Assert.AreEqual(-2, Math.Round(-1.6));
            Assert.AreEqual(Minutes(-2), target.RoundTime(Minutes(-1.6), Minutes(1)));

            Assert.AreEqual(Minutes(4), target.RoundTime(Minutes(3), Minutes(2)));
            Assert.AreEqual(Minutes(2), target.RoundTime(Minutes(2.9), Minutes(2)));
            Assert.AreEqual(Minutes(4), target.RoundTime(Minutes(3.1), Minutes(2)));
        }

        [TestMethod]
        public void TicksToStringTests()
        {
            var service = new TimeTicksFormatService();

            var scale1Second = TimeSpan.FromSeconds(1).Ticks;
            var scale10Second = TimeSpan.FromSeconds(.1).Ticks;
            var scale100Second = TimeSpan.FromSeconds(.01).Ticks;
            var scale1000Second = TimeSpan.FromSeconds(.001).Ticks;

            var ts = new TimeSpan(0, 12, 46, 54, 745);

            Assert.AreEqual("12:46:55", service.TicksToString(ts.Ticks, scale1Second));
            Assert.AreEqual("12:46:54.7", service.TicksToString(ts.Ticks, scale10Second));
            Assert.AreEqual("12:46:54.74", service.TicksToString(ts.Ticks, scale100Second));
            Assert.AreEqual("12:46:54.745", service.TicksToString(ts.Ticks, scale1000Second));

            ts = new TimeSpan(46, 12, 46, 54, 745);

            Assert.AreEqual("1116:46:55", service.TicksToString(ts.Ticks, scale1Second));
            Assert.AreEqual("1116:46:54.7", service.TicksToString(ts.Ticks, scale10Second));
            Assert.AreEqual("1116:46:54.74", service.TicksToString(ts.Ticks, scale100Second));
            Assert.AreEqual("1116:46:54.745", service.TicksToString(ts.Ticks, scale1000Second));

            Assert.AreEqual("12:01:55", service.TicksToString(new TimeSpan(0, 12, 01, 54, 745).Ticks, scale1Second));
        }


        [TestMethod]
        public void ParseTets()
        {
            var service = new TimeTicksFormatService();

            Assert.AreEqual(new TimeSpan(1, 1, 1).Ticks, service.ParseToTicks("1:1:1"));
            Assert.AreEqual(new TimeSpan(0, 40, 10).Ticks, service.ParseToTicks("00:40:10"));

        }
    }
}
