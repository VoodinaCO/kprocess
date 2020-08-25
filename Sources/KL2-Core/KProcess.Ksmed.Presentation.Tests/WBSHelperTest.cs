using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;

namespace KProcess.Ksmed.Presentation.Tests
{


    /// <summary>
    ///This is a test class for WBSHelperTest and is intended
    ///to contain all WBSHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class WBSHelperTest
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
        ///A test for CanBeSuccessive
        ///</summary>
        [TestMethod()]
        public void CanBeSuccessiveTest()
        {
            Assert.IsTrue(WBSHelper.CanBeSuccessive("1", "1.1"));
            Assert.IsTrue(WBSHelper.CanBeSuccessive("1", "2"));
            Assert.IsTrue(WBSHelper.CanBeSuccessive("1.1", "2"));
            Assert.IsTrue(WBSHelper.CanBeSuccessive("1.1.2", "2"));
            Assert.IsTrue(WBSHelper.CanBeSuccessive("1.1.2", "1.2"));

            Assert.IsFalse(WBSHelper.CanBeSuccessive("1", "3"));
            Assert.IsFalse(WBSHelper.CanBeSuccessive("1", "1.2"));
            Assert.IsFalse(WBSHelper.CanBeSuccessive("1.1", "1.3"));
            Assert.IsFalse(WBSHelper.CanBeSuccessive("1.1", "3"));
            Assert.IsFalse(WBSHelper.CanBeSuccessive("2", "2"));
            Assert.IsFalse(WBSHelper.CanBeSuccessive("3", "2"));
        }

        /// <summary>
        ///A test for GetSliblingsAndDescendents
        ///</summary>
        [TestMethod()]
        public void GetSliblingsAndDescendentsTest()
        {
            var g1 = new KAction() { Label = "G1", WBS = "1" };
            var t1 = new KAction() { Label = "T1", WBS = "1.1" };
            var t2 = new KAction() { Label = "T2", WBS = "1.2" };
            var t21 = new KAction() { Label = "T21", WBS = "1.2.1" };
            var t22 = new KAction() { Label = "T22", WBS = "1.2.2" };
            var t3 = new KAction() { Label = "T3", WBS = "2" };
            var actions = new List<KAction>() { g1, t1, t2, t21, t22, t3 };

            var expected = new List<KAction>() { t2, t21, t22 };
            var returned = WBSHelper.GetSucessiveSliblingsAndDescendents(t1, actions).ToArray();
            CollectionAssert.AreEqual(expected, returned);
        }

        /// <summary>
        ///A test for GetSliblingsAndDescendents
        ///</summary>
        [TestMethod()]
        public void GetSliblingsAndDescendents2Test()
        {
            var g1 = new KAction() { Label = "G1", WBS = "1" };
            var t1 = new KAction() { Label = "T1", WBS = "1.1" };
            var t2 = new KAction() { Label = "T2", WBS = "1.2" };
            var t21 = new KAction() { Label = "T21", WBS = "1.2.1" };
            var t22 = new KAction() { Label = "T22", WBS = "1.2.2" };
            var t3 = new KAction() { Label = "T3", WBS = "2" };
            var t31 = new KAction() { Label = "T31", WBS = "2.1" };
            var t32 = new KAction() { Label = "T32", WBS = "2.2" };
            var actions = new List<KAction>() { g1, t1, t2, t21, t22, t3, t31, t32 };

            var expected = new List<KAction>() { t32 };
            var returned = WBSHelper.GetSucessiveSliblingsAndDescendents(t31, actions).ToArray();
            CollectionAssert.AreEqual(expected, returned);
        }

        /// <summary>
        ///A test for Unindent
        ///</summary>
        [TestMethod()]
        public void UnindentTest()
        {
            Assert.AreEqual("2.1", WBSHelper.Unindent("1.2.1", 0));
            Assert.AreEqual("1.1", WBSHelper.Unindent("1.2.1", 1));
            Assert.AreEqual("1.2", WBSHelper.Unindent("1.2.1", 2));
        }

        /// <summary>
        ///A test for indent
        ///</summary>
        [TestMethod()]
        public void IndentTest()
        {
            Assert.AreEqual("1.2.1", WBSHelper.Indent("1.2"));
        }

        /// <summary>
        ///A test for CopyNumberAtLevel
        ///</summary>
        [TestMethod()]
        public void CopyNumberAtLevelTest()
        {
            Assert.AreEqual("1.2.1", WBSHelper.CopyNumberAtLevel("2.2.2", "1.3.1", 1));
            Assert.AreEqual("1.2.1", WBSHelper.CopyNumberAtLevel("2.2.2", "1.3.1", 1));
        }

        /// <summary>
        /// Teste le comparer.
        /// </summary>
        [TestMethod()]
        public void TestComparer()
        {
            var wbs = new List<int[]>()
            {
                null,
                new int[] {},
                new int[] {1},
                new int[] {2},
                new int[] {1, 12},
                new int[] {1, 1},
                new int[] {3},
                new int[] {4,5,1,6},
                new int[] {0},
            };

            wbs.Sort(new WBSHelper.WBSComparer());

            var expected = new List<int[]>()
            {
                null,
                new int[] {},
                new int[] {0},
                new int[] {1},
                new int[] {1, 1},
                new int[] {1, 12},
                new int[] {2},
                new int[] {3},
                new int[] {4,5,1,6},
            };

            Assert.AreEqual(Dump(expected), Dump(wbs));

            Assert.AreEqual(0, new WBSHelper.WBSComparer().Compare(new int[] { 1 }, new int[] { 1 }));
        }

        private string Dump(List<int[]> wbses)
        {
            var sb = new StringBuilder();

            foreach (var item in wbses)
            {
                if (item == null)
                    sb.Append("null");
                else
                {
                    for (int i = 0; i < item.Length; i++)
                    {
                        sb.Append(item[i]);
                        sb.Append(".");
                    }
                    sb.Remove(sb.Length - 1, 1);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        [TestMethod]
        public void TestStartWith()
        {
            Assert.IsTrue(WBSHelper.StartsWith(new int[] { 1 }, new int[] { 1 }));
            Assert.IsTrue(WBSHelper.StartsWith(new int[] { 1, 1 }, new int[] { 1 }));
            Assert.IsTrue(WBSHelper.StartsWith(new int[] { 1, 5 }, new int[] { 1 }));

            Assert.IsFalse(WBSHelper.StartsWith(new int[] { 1 }, new int[] { 1, 1 }));
            Assert.IsFalse(WBSHelper.StartsWith(new int[] { 1 }, new int[] { 2 }));
        }


        // Lié au bug 749

        //kdmmùù 1
        //  dsfng 1.1
        //    xxcbxc 1.1.1
        //  xqcccd 1.2
        //  sqdwfx 1.3
        //  sdfxcv 1.4
        //xcvb 2
        //xbc 3
        //  vcxbv  3.1
        [TestMethod]
        public void TestGetLastPrecedingSibling()
        {
            var g1 = new KAction() { Label = "G1", WBS = "1" };
            var t1 = new KAction() { Label = "T1", WBS = "1.1" };
            var t11 = new KAction() { Label = "T11", WBS = "1.1.1" };
            var t2 = new KAction() { Label = "T2", WBS = "1.2" };
            var t3 = new KAction() { Label = "T3", WBS = "1.3" };
            var t4 = new KAction() { Label = "T4", WBS = "1.4" };
            var xcvb = new KAction() { Label = "xcvb", WBS = "2" };
            var xbc = new KAction() { Label = "xbc", WBS = "3" };
            var vcxbv = new KAction() { Label = "vcxbv", WBS = "3.1" };

            var actions = new List<KAction>() { g1, t1, t11, t2, t3, t4, xcvb, xbc, vcxbv };

            Assert.AreEqual(actions[1], WBSHelper.GetLastPrecedingSibling(actions[3].WBS, actions));


        }

        /// <summary>
        ///A test for AreSiblings
        ///</summary>
        [TestMethod()]
        public void AreSiblingsTest()
        {
            Assert.IsFalse(WBSHelper.AreSiblings("1", "1.1"));
            Assert.IsFalse(WBSHelper.AreSiblings("2.2", "4"));
            Assert.IsFalse(WBSHelper.AreSiblings("1.1", "2.1"));
            Assert.IsTrue(WBSHelper.AreSiblings("1", "2"));
            Assert.IsTrue(WBSHelper.AreSiblings("2.1", "2.6"));
        }
    }
}
