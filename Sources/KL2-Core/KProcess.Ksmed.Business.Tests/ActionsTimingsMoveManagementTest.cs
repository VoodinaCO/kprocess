using System;
using Business.Tests;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Business.Dtos.Prepare;
using KProcess.Ksmed.Business.Impl;
using KProcess.Ksmed.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KProcess.Ksmed.Business.Tests
{


    /// <summary>
    ///This is a test class for ActionsTimingsMoveManagementTest and is intended
    ///to contain all ActionsTimingsMoveManagementTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ActionsTimingsMoveManagementTest
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
        ///A test for IsActionInternal
        ///</summary>
        [TestMethod()]
        public void TestsIsActionInternalExternalDeletedMutuallyExclusive()
        {
            SampleData.ClearDatabaseThenImportDefaultProject();
            // Vérifie dans toutes les actions de la BDD si les actions sont soi I, E ou S et jamais aucun des trois ou plus d'un des 3 à la fois.
            var prepareService = new PrepareService();
            var analyzeService = new AnalyzeService();
            var mre = new System.Threading.ManualResetEvent(false);

            ProjectsData pData = null;
            Exception exception = null;
            prepareService.GetProjects(d =>
            {
                pData = d;
                mre.Set();
            }, ex =>
            {
                exception = ex;
                mre.Set();
            });

            mre.WaitOne();
            AssertExt.IsExceptionNull(exception);

            foreach (var project in pData.Projects)
            {
                mre.Reset();
                RestitutionData rData = null;
                analyzeService.GetFullProjectDetails(project.ProjectId, d =>
                {
                    rData = d;
                    mre.Set();
                }, ex =>
                {
                    exception = ex;
                    mre.Set();
                });

                mre.WaitOne();
                AssertExt.IsExceptionNull(exception);

                foreach (var scenario in rData.Scenarios)
                {
                    foreach (var action in scenario.Actions)
                    {
                        bool i = ActionsTimingsMoveManagement.IsActionInternal(action);
                        bool e = ActionsTimingsMoveManagement.IsActionExternal(action);
                        bool s = ActionsTimingsMoveManagement.IsActionDeleted(action);

                        Assert.IsTrue(i || e || s);

                        if (i)
                            Assert.IsFalse(e || s);
                        else if (e)
                            Assert.IsFalse(i || s);
                        else
                            Assert.IsFalse(i || e);
                    }
                }

            }
        }

        /// <summary>
        /// Test le recalcul des temps à partir des précécesseurs, lorsqu'on est en dehors du chemin critique
        /// </summary>
        [TestMethod]
        public void FixPredecessorsSuccessorsTimings_Outside_Of_CriticalPath()
        {
            // Création de 3 tâches successives

            // Voilà une représentation des tâches
            // -- T1
            //   -- T2
            //     -- T3
            //   - T4

            // Et les précédesseurs :
            // T3 -> T2 -> T1,
            // T3 -> T4 -> T1

            // Le test va consister à supprimer T2
            // Quand on le supprime, T3 devrait se décaller de 1 vers la droite que correspondre au précédesseur T4

            var t1 = CreateAction("1", 2);
            var t2 = CreateAction("2", 2, t1);
            var t4 = CreateAction("4", 1, t1);
            var t3 = CreateAction("3", 2, t2, t4);

            var actions = new KAction[] { t1, t2, t3, t4 };

            ActionsTimingsMoveManagement.FixPredecessorsSuccessorsTimings(actions, false);

            // On vérifie que le temps process a bien été modifié
            Assert.AreEqual(4, t3.BuildStart);
            Assert.AreEqual(6, t3.BuildFinish);

            // On supprime t2
            t3.Predecessors.Remove(t2);
            actions = new KAction[] { t1, t3, t4 };

            ActionsTimingsMoveManagement.FixPredecessorsSuccessorsTimings(actions, false);

            // On vérifie que le temps process a bien été modifié
            Assert.AreEqual(3, t3.BuildStart);
            Assert.AreEqual(5, t3.BuildFinish);
        }

        private static KAction CreateAction(string index)
        {
            return CreateAction(index, 1, new KAction[] { });
        }

        private static KAction CreateAction(string index, int duration, params KAction[] predecessors)
        {
            var t = new KAction
            {
                Label = "T" + index,
                WBS = index,
                Start = 0,
                Finish = 1,
                BuildStart = 0,
                BuildFinish = duration,
            };
            t.Predecessors.AddRange(predecessors);
            return t;
        }
    }
}
