using System;
using System.Linq;
using System.Runtime.InteropServices;
using Business.Tests;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Business.Dtos.Prepare;
using KProcess.Ksmed.Business.Impl;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KProcess.Ksmed.Business.Tests
{


    /// <summary>
    ///This is a test class for ScenarioCloneManagerTest and is intended
    ///to contain all ScenarioCloneManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ScenarioCloneManagerTest
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
        public void CreateDerivatedScenarioTest1()
        {
            var scenarioCible1 = new Scenario()
            {
                NatureCode = KnownScenarioNatures.Target,
            };

            var actionEScenario1 = new KAction
            {
                BuildDuration = 1,
                Scenario = scenarioCible1,
                Reduced = new KActionReduced()
                {
                    ActionTypeCode = KnownActionCategoryTypes.E,
                },
                WBS = "1",
            };

            var actionIScenario1 = new KAction
            {
                BuildDuration = 1,
                Scenario = scenarioCible1,
                Reduced = new KActionReduced()
                {
                    ActionTypeCode = KnownActionCategoryTypes.I,
                },
                WBS = "2",
            };

            var newScenario = ScenarioCloneManager.CreateDerivatedScenario(null, scenarioCible1, KnownScenarioNatures.Target, false, 2);

            var actionEScenario2 = newScenario.Actions[0];
            var actionIScenario2 = newScenario.Actions[1];

            Assert.IsTrue(actionEScenario2.IsReduced);
            Assert.AreEqual(KnownActionCategoryTypes.E, actionEScenario2.Reduced.ActionTypeCode);
            Assert.AreEqual(KnownActionCategoryTypes.I, actionIScenario2.Reduced.ActionTypeCode);
        }

        [TestMethod()]
        public void CreateDerivatedScenarioTest2()
        {
            var scenarioCible1 = new Scenario()
            {
                NatureCode = KnownScenarioNatures.Initial,
            };

            var cat = new ActionCategoryProject { };
            var catI = new ActionCategoryProject { ActionTypeCode = KnownActionCategoryTypes.I };
            var catE = new ActionCategoryProject { ActionTypeCode = KnownActionCategoryTypes.E };
            var catS = new ActionCategoryProject { ActionTypeCode = KnownActionCategoryTypes.S };

            var a = new KAction { Scenario = scenarioCible1, Category = cat, WBS = "1" };
            var aI = new KAction { Scenario = scenarioCible1, Category = catI, WBS = "2" };
            var aE = new KAction { Scenario = scenarioCible1, Category = catE, WBS = "3" };
            var aS = new KAction { Scenario = scenarioCible1, Category = catS, WBS = "4" };

            var newScenario = ScenarioCloneManager.CreateDerivatedScenario(null, scenarioCible1, KnownScenarioNatures.Target, false, 2);

            Assert.AreEqual(KnownActionCategoryTypes.I, newScenario.Actions[0].Reduced.ActionTypeCode);
            Assert.AreEqual(KnownActionCategoryTypes.I, newScenario.Actions[1].Reduced.ActionTypeCode);
            Assert.AreEqual(KnownActionCategoryTypes.E, newScenario.Actions[2].Reduced.ActionTypeCode);
            Assert.AreEqual(KnownActionCategoryTypes.S, newScenario.Actions[3].Reduced.ActionTypeCode);
        }

        /// <summary>
        /// Test de création d'un scénario dérivé - bug 1552.
        /// Exigence : Une tache E d'un scénario cible parent doit rester E dans le scénario cible enfant.
        /// </summary>
        [TestMethod()]
        public void CreateDerivatedScenario_Bug1552_Test()
        {
            var scenarioCible1 = new Scenario()
            {
                NatureCode = KnownScenarioNatures.Target,
            };

            var actionEScenario1 = new KAction
            {
                Scenario = scenarioCible1,
                BuildDuration = 1,
                Reduced = new KActionReduced()
                {
                    ActionTypeCode = KnownActionCategoryTypes.E,
                    ReductionRatio = .1,
                },
                WBS = "1",
            };

            var newScenario = ScenarioCloneManager.CreateDerivatedScenario(null, scenarioCible1, KnownScenarioNatures.Target, false, 2);

            var actionEScenario2 = newScenario.Actions.First();

            Assert.IsTrue(actionEScenario2.IsReduced);
            Assert.AreEqual(KnownActionCategoryTypes.E, actionEScenario2.Reduced.ActionTypeCode);
        }

        [TestMethod()]
        public void CreateDerivatedScenario_TargetToTarget_Test()
        {
            var scenario = new Scenario
            {
                NatureCode = KnownScenarioNatures.Target,
            };


            var catI = new ActionCategoryStandard { ActionTypeCode = KnownActionCategoryTypes.I };
            var catE = new ActionCategoryStandard { ActionTypeCode = KnownActionCategoryTypes.E };
            var catS = new ActionCategoryStandard { ActionTypeCode = KnownActionCategoryTypes.S };

            var actionIGrandParent = new KAction
            {
                Scenario = scenario,
                Duration = 27,
                BuildDuration = 27,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.I,
                    ReductionRatio = .1,
                    OriginalBuildDuration = 30,
                },
                OriginalActionId = -1, // simule un original en dehors du scénario
                Category = catE,
                WBS = "1",
            };

            var actionIparentCatI = new KAction
            {
                Scenario = scenario,
                Duration = 27,
                BuildDuration = 27,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.I,
                    ReductionRatio = .1,
                    OriginalBuildDuration = 30,
                },
                Category = catI,
                WBS = "2",
            };

            var actionIparentCatE = new KAction
            {
                Scenario = scenario,
                Duration = 27,
                BuildDuration = 27,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.I,
                    ReductionRatio = .1,
                    OriginalBuildDuration = 30,
                },
                Category = catE,
                WBS = "3",
            };

            var actionIparentCatS = new KAction
            {
                Label = "actionIparentCatS",
                Scenario = scenario,
                Duration = 27,
                BuildDuration = 27,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.I,
                    ReductionRatio = .1,
                    OriginalBuildDuration = 30,
                },
                Category = catS,
                WBS = "4",
            };

            var actionE = new KAction
            {
                Scenario = scenario,
                Duration = 27,
                BuildDuration = 27,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.E,
                    ReductionRatio = .1,
                    OriginalBuildDuration = 30,
                },
                Category = catI,
                WBS = "5",
            };

            var actionS = new KAction
            {
                Label = "actionS",
                Scenario = scenario,
                Duration = 0,
                BuildDuration = 0,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.S,
                    ReductionRatio = 1,
                },
                WBS = "6",
            };

            var newScenario = ScenarioCloneManager.CreateDerivatedScenario(null, scenario, KnownScenarioNatures.Target, false, 2);

            // Tache I provenant du grand parent, on n'applique pas le prétype mais on applique la réduc
            var cloneIGrandParent = newScenario.Actions[0];
            AssertClonedAction(cloneIGrandParent, 27, true, 0, 27, KnownActionCategoryTypes.I);

            // Tache I provenant du parent, on applique le prétypage et la réduc s'il y en a : prétypage I
            var cloneIparentCatI = newScenario.Actions[1];
            AssertClonedAction(cloneIparentCatI, 27, true, 0, 27, KnownActionCategoryTypes.I);

            // Tache I provenant du parent, on applique le prétypage et la réduc s'il y en a : prétypage E
            var cloneIparentCatE = newScenario.Actions[2];
            AssertClonedAction(cloneIparentCatE, 27, true, 0, 27, KnownActionCategoryTypes.E);

            // Tache I provenant du parent, on applique le prétypage et la réduc s'il y en a : prétypage S => Suppression
            var cloneIparentCatS = newScenario.Actions[0];
            Assert.IsTrue(!newScenario.Actions.Any(a => a.Label == actionIparentCatS.Label));

            // Tâche E, on conserve E et on applique la réduc
            var cloneE = newScenario.Actions[3];
            AssertClonedAction(cloneE, 27, true, 0, 27, KnownActionCategoryTypes.E);

            // Tâche S, on supprime l'action
            var cloneS = newScenario.Actions[0];
            Assert.IsTrue(!newScenario.Actions.Any(a => a.Label == actionS.Label));
        }


        private void AssertClonedAction(KAction clone, long newDuration, bool hasReduced, double reductionRatio, long originalBuildDuration, string reducedActionTypeCode)
        {
            Assert.AreEqual(newDuration, clone.BuildDuration);
            Assert.AreEqual(hasReduced, clone.Reduced != null);
            if (hasReduced)
            {
                Assert.AreEqual(reductionRatio, clone.Reduced.ReductionRatio);
                Assert.AreEqual(originalBuildDuration, clone.Reduced.OriginalBuildDuration);
                Assert.AreEqual(reducedActionTypeCode, clone.Reduced.ActionTypeCode);
            }
        }

        private void AssertClonedActionNotReduced(KAction clone, long newDuration)
        {
            Assert.AreEqual(newDuration, clone.BuildDuration);
            Assert.IsNull(clone.Reduced);
        }

        [TestMethod()]
        public void CloneActionFromInitialToTargetTest()
        {
            var catI = new ActionCategoryStandard { ActionTypeCode = KnownActionCategoryTypes.I };
            var catE = new ActionCategoryStandard { ActionTypeCode = KnownActionCategoryTypes.E };
            var catS = new ActionCategoryStandard { ActionTypeCode = KnownActionCategoryTypes.S };

            var actionNonPretypee = new KAction()
            {
                BuildDuration = 30,
            };

            var actionPretypeeI = new KAction()
            {
                BuildDuration = 30,
                Category = catI
            };

            var actionPretypeeE = new KAction()
            {
                BuildDuration = 30,
                Category = catE
            };

            var actionPretypeeS = new KAction()
            {
                BuildDuration = 30,
                Category = catS
            };

            // En mode initial to target, on n'est pas sensé avoir de partie réduite à la base
            // Le prétype par la catégorie uniquement s'applique

            var clone = ScenarioCloneManager.CloneAction(actionNonPretypee, ScenarioCloneManager.ActionCloneBehavior.InitialToTarget);
            AssertClonedAction(clone, 30, true, 0, 30, KnownActionCategoryTypes.I);

            clone = ScenarioCloneManager.CloneAction(actionPretypeeI, ScenarioCloneManager.ActionCloneBehavior.InitialToTarget);
            AssertClonedAction(clone, 30, true, 0, 30, KnownActionCategoryTypes.I);

            clone = ScenarioCloneManager.CloneAction(actionPretypeeE, ScenarioCloneManager.ActionCloneBehavior.InitialToTarget);
            AssertClonedAction(clone, 30, true, 0, 30, KnownActionCategoryTypes.E);

            clone = ScenarioCloneManager.CloneAction(actionPretypeeS, ScenarioCloneManager.ActionCloneBehavior.InitialToTarget);
            AssertClonedAction(clone, 0, true, 1, 30, KnownActionCategoryTypes.S);
        }

        [TestMethod()]
        public void CloneActionFromTargetToTargetTest()
        {
            var scenario = new Scenario()
            {
            };

            var catI = new ActionCategoryStandard { ActionTypeCode = KnownActionCategoryTypes.I };
            var catE = new ActionCategoryStandard { ActionTypeCode = KnownActionCategoryTypes.E };
            var catS = new ActionCategoryStandard { ActionTypeCode = KnownActionCategoryTypes.S };

            var actionIGrandParent = new KAction
            {
                BuildDuration = 27,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.I,
                    ReductionRatio = .1,
                    OriginalBuildDuration = 30,
                },
                OriginalActionId = -1, // simule un original en dehors du scénario
                Category = catE,
            };

            // Tache I provenant du grand parent, on n'applique pas le prétype mais on applique la réduc
            var clone = ScenarioCloneManager.CloneAction(actionIGrandParent, ScenarioCloneManager.ActionCloneBehavior.TargetToTarget);
            AssertClonedAction(clone, 27, true, 0, 27, KnownActionCategoryTypes.I);



            var actionIparentCatI = new KAction
            {
                BuildDuration = 27,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.I,
                    ReductionRatio = .1,
                    OriginalBuildDuration = 30,
                },
                Category = catI,
            };

            // Tache I provenant du parent, on applique le prétypage et la réduc s'il y en a
            clone = ScenarioCloneManager.CloneAction(actionIparentCatI, ScenarioCloneManager.ActionCloneBehavior.TargetToTarget);
            AssertClonedAction(clone, 27, true, 0, 27, KnownActionCategoryTypes.I);



            var actionIparentCatE = new KAction
            {
                BuildDuration = 27,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.I,
                    ReductionRatio = .1,
                    OriginalBuildDuration = 30,
                },
                Category = catE,
            };

            // Tache I provenant du parent, on applique le prétypage et la réduc s'il y en a
            clone = ScenarioCloneManager.CloneAction(actionIparentCatE, ScenarioCloneManager.ActionCloneBehavior.TargetToTarget);
            AssertClonedAction(clone, 27, true, 0, 27, KnownActionCategoryTypes.E);





            var actionIparentCatS = new KAction
            {
                BuildDuration = 27,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.I,
                    ReductionRatio = .1,
                    OriginalBuildDuration = 30,
                },
                Category = catS,
            };

            // Tache I provenant du parent, on applique le prétypage et la réduc s'il y en a
            clone = ScenarioCloneManager.CloneAction(actionIparentCatS, ScenarioCloneManager.ActionCloneBehavior.TargetToTarget);
            AssertClonedAction(clone, 0, true, 1, 27, KnownActionCategoryTypes.S);




            var actionE = new KAction
            {
                BuildDuration = 27,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.E,
                    ReductionRatio = .1,
                    OriginalBuildDuration = 30,
                },
                Category = catI,
            };

            // Tâche E, on conserve E et on applique la réduc
            clone = ScenarioCloneManager.CloneAction(actionE, ScenarioCloneManager.ActionCloneBehavior.TargetToTarget);
            AssertClonedAction(clone, 27, true, 0, 27, KnownActionCategoryTypes.E);



            var actionS = new KAction
            {
                BuildDuration = 0,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.S,
                    ReductionRatio = 1,
                }
            };

            // Tâche S, on supprime l'action
            clone = ScenarioCloneManager.CloneAction(actionS, ScenarioCloneManager.ActionCloneBehavior.TargetToTarget);
            AssertClonedAction(clone, 0, true, 1, 0, KnownActionCategoryTypes.S);


            var scenarioSolutionNOK = new Scenario();
            scenarioSolutionNOK.Solutions.Add(new Solution()
            {
                SolutionDescription = "NOK",
                Approved = false,
            });
            var actionENOK = new KAction
            {
                BuildDuration = 30,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.E,
                    ReductionRatio = .1,
                    OriginalBuildDuration = 30,
                    Solution = "NOK",
                },
                Category = catS,
                Scenario = scenarioSolutionNOK,
            };

            // Tâche NOK, on ne prend pas en compte le prétypage ni l'action type du reduced, on la laisse I
            clone = ScenarioCloneManager.CloneAction(actionENOK, ScenarioCloneManager.ActionCloneBehavior.TargetToTarget);
            AssertClonedAction(clone, 30, true, 0, 30, KnownActionCategoryTypes.I);
        }

        [TestMethod()]
        public void CloneActionFromTargetToRealizedTest()
        {
            // Target to realized : pour I et E, on copie la partie Realized

            var action = new KAction
            {
                BuildDuration = 27,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.I,
                    ReductionRatio = .1,
                    OriginalBuildDuration = 30,
                },
            };
            var clone = ScenarioCloneManager.CloneAction(action, ScenarioCloneManager.ActionCloneBehavior.TargetToRealized);
            AssertClonedAction(clone, 27, true, 0d, 27, KnownActionCategoryTypes.I);


            action = new KAction
            {
                BuildDuration = 27,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.E,
                    ReductionRatio = .1,
                    OriginalBuildDuration = 30,
                },
            };
            clone = ScenarioCloneManager.CloneAction(action, ScenarioCloneManager.ActionCloneBehavior.TargetToRealized);
            AssertClonedAction(clone, 27, true, 0d, 27, KnownActionCategoryTypes.E);

            // Une tâche S doit être supprimée
            action = new KAction
            {
                BuildDuration = 0,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.S,
                    ReductionRatio = 1,
                    OriginalBuildDuration = 30,
                },
            };
            clone = ScenarioCloneManager.CloneAction(action, ScenarioCloneManager.ActionCloneBehavior.TargetToRealized);
            AssertClonedActionNotReduced(clone, 0);

        }

        /// <summary>
        ///A test for CloneAction
        ///</summary>
        [TestMethod()]
        public void CloneActionCascadeTest()
        {
            var catI = new ActionCategoryStandard { ActionTypeCode = KnownActionCategoryTypes.I };
            var catE = new ActionCategoryStandard { ActionTypeCode = KnownActionCategoryTypes.E };
            var catS = new ActionCategoryStandard { ActionTypeCode = KnownActionCategoryTypes.S };

            // En mode Cascade, on applique le type de Reduced si la tâche est réduite, sinon on applique le type de ActioNCategory, tout en conservant la réduc

            var action = new KAction
            {
                BuildDuration = 27,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.I,
                    ReductionRatio = .1,
                    OriginalBuildDuration = 30,
                },
                OriginalActionId = -1, // simule un original en dehors du scénario
                Category = catE,
            };
            var clone = ScenarioCloneManager.CloneAction(action, ScenarioCloneManager.ActionCloneBehavior.Cascade);
            AssertClonedAction(clone, 27, true, 0, 27, KnownActionCategoryTypes.I);

            action = new KAction
            {
                BuildDuration = 27,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.I,
                    ReductionRatio = .1,
                    OriginalBuildDuration = 30,
                },
                Category = catI,
            };
            clone = ScenarioCloneManager.CloneAction(action, ScenarioCloneManager.ActionCloneBehavior.Cascade);
            AssertClonedAction(clone, 27, true, 0, 27, KnownActionCategoryTypes.I);


            action = new KAction
            {
                BuildDuration = 27,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.I,
                    ReductionRatio = .1,
                    OriginalBuildDuration = 30,
                },
                Category = catE,
            };
            clone = ScenarioCloneManager.CloneAction(action, ScenarioCloneManager.ActionCloneBehavior.Cascade);
            AssertClonedAction(clone, 27, true, 0, 27, KnownActionCategoryTypes.I);



            action = new KAction
            {
                BuildDuration = 27,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.I,
                    ReductionRatio = .1,
                    OriginalBuildDuration = 30,
                },
                Category = catS,
            };
            clone = ScenarioCloneManager.CloneAction(action, ScenarioCloneManager.ActionCloneBehavior.Cascade);
            AssertClonedAction(clone, 27, true, 0, 27, KnownActionCategoryTypes.I);



            action = new KAction
            {
                BuildDuration = 27,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.E,
                    ReductionRatio = .1,
                    OriginalBuildDuration = 30,
                },
                Category = catE,
            };
            clone = ScenarioCloneManager.CloneAction(action, ScenarioCloneManager.ActionCloneBehavior.Cascade);
            AssertClonedAction(clone, 27, true, 0, 27, KnownActionCategoryTypes.E);



            action = new KAction
            {
                BuildDuration = 0,
                Reduced = new KActionReduced
                {
                    ActionTypeCode = KnownActionCategoryTypes.S,
                    ReductionRatio = 1,
                    OriginalBuildDuration = 30,
                },
                Category = catS,
            };
            clone = ScenarioCloneManager.CloneAction(action, ScenarioCloneManager.ActionCloneBehavior.Cascade);
            AssertClonedAction(clone, 0, true, 1, 0, KnownActionCategoryTypes.S);


        }

        [TestMethod()]
        public void BulkScenarioCloneTests()
        {
            SampleData.ClearDatabaseThenImportDefaultProject();
            var service = new PrepareService();

            int[] projectIds;
            using (var context = KProcess.Ksmed.Data.ContextFactory.GetNewContext())
            {
                projectIds = context.Projects.Select(p => p.ProjectId).ToArray();
            }

            foreach (var pid in projectIds)
            {
                var mre = new System.Threading.ManualResetEvent(false);

                Exception e = null;
                ScenariosData data = null;

                service.GetScenarios(pid, d =>
                {
                    data = d;
                    mre.Set();
                }, ex =>
                {
                    e = ex;
                    mre.Set();
                });

                mre.WaitOne();
                AssertExt.IsExceptionNull(e);
                Assert.IsNotNull(data);

                foreach (var scenario in data.Scenarios.Where(s => s.NatureCode != KnownScenarioNatures.Realized))
                {
                    if (scenario.NatureCode == KnownScenarioNatures.Target && scenario.Actions.Any(a => !a.IsReduced))
                    {
                        // Il s'agit d'un vieux projet. Tous les actions d'un scénario cible doivent aujourd'hui avoir une partie réduite
                        continue;
                    }

                    mre.Reset();

                    Scenario newScenario = null;

                    service.CreateScenario(pid, scenario, true, s =>
                    {
                        newScenario = s;
                        mre.Set();
                    }, ex =>
                    {
                        e = ex;
                        mre.Set();
                    });

                    mre.WaitOne();
                    AssertExt.IsExceptionNull(e);
                    Assert.IsNotNull(newScenario);

                    // Vérification de l'intégrité du scénario
                    ActionsTimingsMoveManagement.DebugCheckAllWBS(EnumerableExt.Concat(newScenario));

                    // Vérifier qu'il n'y ai pas de tâche avec un temps process nul
                    if (scenario.NatureCode != KnownScenarioNatures.Initial && newScenario.Actions.Any(a => a.BuildDuration <= 0))
                        Assert.Fail("Une action a un temps invalide");

                }
            }
        }

    }
}
