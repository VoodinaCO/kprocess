using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using KProcess.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KProcess.Ksmed.Business.Tests;
using KProcess.Ksmed.Data;

namespace KProcess.Ksmed.Business.Tests
{


    /// <summary>
    ///This is a test class for ObjectContextExtTest and is intended
    ///to contain all ObjectContextExtTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ObjectContextExtTest
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
        public void SetRelationShipReferenceValueTest()
        {
            SampleData.ClearDatabaseThenImportDefaultProject();

            var context = ContextFactory.GetNewContext();

            var actionDerived = context.KActions.First(a => a.OriginalActionId != null);
            var actionOriginal = context.KActions.First(a => a.ActionId == actionDerived.ActionId);

            Assert.IsTrue(
                ObjectContextExt.SetRelationShipReferenceValue(context, actionDerived, actionOriginal, "KProcess.KsmedModel.FK_Action_Orignal", "Action"));

        }

        [TestMethod()]
        public void SetRelationShipReferenceValueTestExp()
        {
            SampleData.ClearDatabaseThenImportDefaultProject();

            var context = ContextFactory.GetNewContext();

            var actionDerived = context.KActions.First(a => a.OriginalActionId != null);
            var actionOriginal = context.KActions.First(a => a.ActionId == actionDerived.ActionId);

            Assert.IsTrue(
                ObjectContextExt.SetRelationShipReferenceValue(context, actionDerived, actionOriginal,
                a => a.OriginalActionId));

        }
    }
}
