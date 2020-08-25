using System;
using System.Collections.Generic;
using System.Linq;
using KProcess.Ksmed.Business.Impl;
using KProcess.Ksmed.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Business.Tests
{


    /// <summary>
    ///This is a test class for ProjectServiceTest and is intended
    ///to contain all ProjectServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ProjectServiceTest
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

        ///// <summary>
        /////A test for GetUsersAndRoles
        /////</summary>
        //[TestMethod()]
        //public void GetUsersAndRolesTest()
        //{
        //    PrepareService target = new PrepareService();
        //    var mre = new System.Threading.ManualResetEvent(false);

        //    IEnumerable<User> users = null;
        //    IEnumerable<Role> roles = null;
        //    target.GetMembers((u, r) =>
        //    {
        //        users = u;
        //        roles = r;
        //        mre.Set();
        //    }, FailServiceException);

        //    mre.WaitOne();

        //    Assert.IsNotNull(users);
        //    Assert.IsNotNull(users.FirstOrDefault());

        //    Assert.IsNotNull(roles);
        //    Assert.IsNotNull(roles.FirstOrDefault());
        //    Assert.IsNotNull(roles.FirstOrDefault().ShortLabel);
        //}

        ///// <summary>
        /////A test for SaveProject
        /////</summary>
        //[TestMethod()]
        //public void SaveProjectTest()
        //{
        //    var rboyer = new User() { UserId = 1, Username = "rboyer" };
        //    KProcess.Ksmed.Security.Context.CurrentUser = new KProcess.Ksmed.Security.User(rboyer);

        //    var project = new Project()
        //    {
        //        Label = "Project from test",
        //        Description = "Description Project from test",
        //        ObjectiveCode = "Obj001",
        //        Workshop = "Atelier from test"
        //    };

        //    var mre = new System.Threading.ManualResetEvent(false);

        //    IEnumerable<User> users = null;
        //    IEnumerable<Role> roles = null;
        //    new PrepareService().LoadMembers((u, r) =>
        //    {
        //        users = u;
        //        roles = r;
        //        mre.Set();
        //    }, FailServiceException);

        //    mre.WaitOne();

        //    project.UserRoleProjects.Add(new UserRoleProject() { Role = roles.First(), User = users.First() });
        //    project.UserRoleProjects.Add(new UserRoleProject() { Role = roles.Skip(1).First(), User = users.Skip(1).First() });

        //    mre = new System.Threading.ManualResetEvent(false);
        //    var service = new PrepareService();
        //    service.SaveProject(project, () =>
        //    {
        //        mre.Set();
        //    }, FailServiceException);
        //    mre.WaitOne();

        //    Assert.Inconclusive("Il faut une méthode pour récupérer un projet afin de valider le test.");
        //}

        private void FailServiceException(Exception e)
        {
            Assert.Fail(e.Message);
        }
    }
}
