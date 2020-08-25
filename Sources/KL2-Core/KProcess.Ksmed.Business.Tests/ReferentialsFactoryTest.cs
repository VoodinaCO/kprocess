using System;
using KProcess.Ksmed.Business.Impl;
using KProcess.Ksmed.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KProcess.Ksmed.Business.Referentials;

namespace KProcess.Ksmed.Business.Tests
{


    /// <summary>
    ///This is a test class for ReferentialsFactoryTest and is intended
    ///to contain all ReferentialsFactoryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ReferentialsFactoryTest
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
        ///A test for ConvertToNewProject
        ///</summary>
        [TestMethod()]
        public void ConvertToNewProjectTest()
        {
            var std = new ActionCategoryStandard()
            {
                ActionTypeCode = "ABCD",
                ActionValueCode = "EFGH",
                Color = "#AB",
                Label = "Label",

                ActionCategoryId = 10,
                CreatedByUserId = 1,
                CreationDate = DateTime.Now,
                ModifiedByUserId = 2,
                LastModificationDate = DateTime.Now,
            };

            var project = (ActionCategoryProject)ReferentialsFactory.CopyToNewProject(std);

            Assert.AreEqual(std.ActionTypeCode, project.ActionTypeCode);
            Assert.AreEqual(std.ActionValueCode, project.ActionValueCode);
            Assert.AreEqual(std.Color, project.Color);
            Assert.AreEqual(std.Label, project.Label);

            Assert.AreEqual(default(int), project.ActionCategoryId);
            Assert.AreEqual(default(int), project.CreatedByUserId);
            Assert.AreEqual(default(int), project.ModifiedByUserId);
            Assert.AreEqual(default(DateTime), project.CreationDate);
            Assert.AreEqual(default(DateTime), project.LastModificationDate);

            Assert.IsNull(project.Project);
            Assert.AreEqual(default(int), project.ProjectId);

            Assert.IsNotNull(ReferentialsFactory.CopyToNewProject(new Ref2Standard()));
            Assert.IsNotNull(ReferentialsFactory.CopyToNewProject(new Ref4Standard()));
            Assert.IsNotNull(ReferentialsFactory.CopyToNewProject(new Ref3Standard()));
            Assert.IsNotNull(ReferentialsFactory.CopyToNewProject(new EquipmentStandard()));
            Assert.IsNotNull(ReferentialsFactory.CopyToNewProject(new OperatorStandard()));
            Assert.IsNotNull(ReferentialsFactory.CopyToNewProject(new Ref1Standard()));
        }
    }
}
