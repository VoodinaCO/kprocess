using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DlhSoft.Windows.Controls;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;

namespace KProcess.Ksmed.Presentation.Tests
{
    [TestClass]
    public class GridActionsManagerWBSViewTest
    {
        private KAction _g1;
        private KAction _t1;
        private KAction _t2;
        private KAction _t21;
        private KAction _t22;
        private KAction _t3;
        private KAction _t31;
        private KAction _t4;

        private KAction[] _actions;
        private BulkObservableCollection<DataTreeGridItem> _collection;
        private DataTreeGridItem _currentItem;
        private int _currentAssertIndex = 0;

        private GridActionsManager _manager;

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
        }

        [TestInitialize]
        public void Initialize()
        {
            ServicesHelper.RegisterMockServices();

            //Arbre initial :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4

            _g1 = new KAction() { Label = "G1", WBS = "1" };
            _t1 = new KAction() { Label = "T1", WBS = "1.1" };
            _t2 = new KAction() { Label = "T2", WBS = "1.2" };
            _t21 = new KAction() { Label = "T21", WBS = "1.2.1" };
            _t22 = new KAction() { Label = "T22", WBS = "1.2.2" };
            _t3 = new KAction() { Label = "T3", WBS = "1.3" };
            _t31 = new KAction() { Label = "T31", WBS = "1.3.1" };
            _t4 = new KAction() { Label = "T4", WBS = "1.4" };

            _actions = new KAction[] { _g1, _t1, _t2, _t21, _t22, _t3, _t31, _t4 };

            _collection = new BulkObservableCollection<DataTreeGridItem>();

            _manager = new GridActionsManager(_collection, v => _currentItem = v, null);
            _manager.ChangeView(GanttGridView.WBS, null);
        }

        private void InitItemsCollectionAssertions()
        {
            _currentAssertIndex = 0;
        }

        private void AssertAction(KAction action, int indentation, string wbs)
        {
            var item = _collection[_currentAssertIndex] as ActionGridItem;
            Assert.IsNotNull(item);

            Assert.AreEqual(action, item.Action);
            Assert.AreEqual(indentation, item.Indentation);
            Assert.AreEqual(wbs, item.Action.WBS);

            _currentAssertIndex++;
        }

        private void EndItemsCollectionAssertions()
        {
            _currentAssertIndex = 0;
        }

        private string DumpAllWBS()
        {
            var sb = new StringBuilder();
            foreach (var action in _actions.OrderBy(a => a.WBS))
            {
                var indentation = WBSHelper.IndentationFromWBS(action.WBS);
                for (int i = 0; i < indentation; i++)
                    sb.Append("  ");
                sb.Append(action.Label);
                sb.Append(" ");
                sb.Append(action.WBS);
                sb.AppendLine();
            }

            Console.WriteLine(sb.ToString());
            return sb.ToString();
        }

        private string DumpCollection()
        {
            var sb = new StringBuilder();
            foreach (var item in _collection)
            {
                var indentation = item.Indentation;
                for (int i = 0; i < indentation; i++)
                    sb.Append("  ");
                sb.Append(item.Content);
                if (item is ActionGridItem)
                {
                    sb.Append(" ");
                    sb.Append(((ActionGridItem)item).Action.WBS);
                }
                sb.AppendLine();
            }

            Console.WriteLine(sb.ToString());
            return sb.ToString();
        }

        /// <summary>
        ///A test for RegisterInitialActions
        ///</summary>
        [TestMethod()]
        public void RegisterInitialActionsTest()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre attendu :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4

            InitItemsCollectionAssertions();
            AssertAction(_g1, 0, "1");
            AssertAction(_t1, 1, "1.1");
            AssertAction(_t2, 1, "1.2");
            AssertAction(_t21, 2, "1.2.1");
            AssertAction(_t22, 2, "1.2.2");
            AssertAction(_t3, 1, "1.3");
            AssertAction(_t31, 2, "1.3.1");
            AssertAction(_t4, 1, "1.4");
            EndItemsCollectionAssertions();
        }

        /// <summary>
        /// Monter T21
        ///</summary>
        [TestMethod()]
        public void MoveUpTestT21()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4

            _manager.MoveUp((ActionGridItem)_collection[3]);

            //Arbre attendu :
            //  G1 1
            //      T1 1.1
            //          T21 1.1.1
            //      T2 1.2
            //          T22 1.2.1
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4

            InitItemsCollectionAssertions();
            AssertAction(_g1, 0, "1");
            AssertAction(_t1, 1, "1.1");
            AssertAction(_t21, 2, "1.1.1");
            AssertAction(_t2, 1, "1.2");
            AssertAction(_t22, 2, "1.2.1");
            AssertAction(_t3, 1, "1.3");
            AssertAction(_t31, 2, "1.3.1");
            AssertAction(_t4, 1, "1.4");
            EndItemsCollectionAssertions();
        }

        /// <summary>
        /// Descendre T1 trois fois
        ///</summary>
        [TestMethod()]
        public void MoveDownTestT1()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4

            _manager.MoveDown((ActionGridItem)_collection[1]);
            _manager.MoveDown((ActionGridItem)_collection[2]);
            _manager.MoveDown((ActionGridItem)_collection[3]);

            //Arbre attendu :
            //  G1 1
            //      T2 1.1
            //          T21 1.1.1
            //          T22 1.1.2
            //      T1 1.2
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4

            InitItemsCollectionAssertions();
            AssertAction(_g1, 0, "1");
            AssertAction(_t2, 1, "1.1");
            AssertAction(_t21, 2, "1.1.1");
            AssertAction(_t22, 2, "1.1.2");
            AssertAction(_t1, 1, "1.2");
            AssertAction(_t3, 1, "1.3");
            AssertAction(_t31, 2, "1.3.1");
            AssertAction(_t4, 1, "1.4");
            EndItemsCollectionAssertions();
        }

        /// <summary>
        /// Monter T3
        ///</summary>
        [TestMethod()]
        public void MoveUpTestT3()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4

            _manager.MoveUp((ActionGridItem)_collection[5]);

            //Arbre attendu :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //      T3 1.3
            //          T22 1.3.1
            //          T31 1.3.2
            //      T4 1.4

            InitItemsCollectionAssertions();
            AssertAction(_g1, 0, "1");
            AssertAction(_t1, 1, "1.1");
            AssertAction(_t2, 1, "1.2");
            AssertAction(_t21, 2, "1.2.1");
            AssertAction(_t3, 1, "1.3");
            AssertAction(_t22, 2, "1.3.1");
            AssertAction(_t31, 2, "1.3.2");
            AssertAction(_t4, 1, "1.4");
            EndItemsCollectionAssertions();
        }

        /// <summary>
        /// Indenter T3
        ///</summary>
        [TestMethod()]
        public void MoveRightTestT3()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4

            _manager.MoveRight((ActionGridItem)_collection[5]);

            //Arbre attendu :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //          T3 1.2.3
            //          T31 1.2.4
            //      T4 1.3

            InitItemsCollectionAssertions();
            AssertAction(_g1, 0, "1");
            AssertAction(_t1, 1, "1.1");
            AssertAction(_t2, 1, "1.2");
            AssertAction(_t21, 2, "1.2.1");
            AssertAction(_t22, 2, "1.2.2");
            AssertAction(_t3, 2, "1.2.3");
            AssertAction(_t31, 2, "1.2.4");
            AssertAction(_t4, 1, "1.3");
            EndItemsCollectionAssertions();
        }

        /// <summary>
        /// Supprimer T1.3
        ///</summary>
        [TestMethod()]
        public void DeleteTestT13()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4

            _manager.DeleteAction(((ActionGridItem)_collection[5]).Action);

            //Arbre attendu :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //          T31 1.2.3
            //      T4 1.3

            InitItemsCollectionAssertions();
            AssertAction(_g1, 0, "1");
            AssertAction(_t1, 1, "1.1");
            AssertAction(_t2, 1, "1.2");
            AssertAction(_t21, 2, "1.2.1");
            AssertAction(_t22, 2, "1.2.2");
            AssertAction(_t31, 2, "1.3");
            AssertAction(_t4, 1, "1.4");
            EndItemsCollectionAssertions();
        }

        /// <summary>
        /// Supprimer T21
        ///</summary>
        [TestMethod()]
        public void DeleteTestT21()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4

            _manager.DeleteAction(((ActionGridItem)_collection[3]).Action);

            //Arbre attendu :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T22 1.2.1
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4

            InitItemsCollectionAssertions();
            AssertAction(_g1, 0, "1");
            AssertAction(_t1, 1, "1.1");
            AssertAction(_t2, 1, "1.2");
            AssertAction(_t22, 2, "1.2.1");
            AssertAction(_t3, 1, "1.3");
            AssertAction(_t31, 2, "1.3.1");
            AssertAction(_t4, 1, "1.4");
            EndItemsCollectionAssertions();
        }

        /// <summary>
        /// Supprimer T22
        ///</summary>
        [TestMethod()]
        public void DeleteTestT22()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4

            _manager.DeleteAction(((ActionGridItem)_collection[4]).Action);

            //Arbre attendu :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4

            InitItemsCollectionAssertions();
            AssertAction(_g1, 0, "1");
            AssertAction(_t1, 1, "1.1");
            AssertAction(_t2, 1, "1.2");
            AssertAction(_t21, 2, "1.2.1");
            AssertAction(_t3, 1, "1.3");
            AssertAction(_t31, 2, "1.3.1");
            AssertAction(_t4, 1, "1.4");
            EndItemsCollectionAssertions();
        }

        /// <summary>
        /// Supprimer T1
        ///</summary>
        [TestMethod()]
        public void DeleteTestT1()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4

            _manager.DeleteAction(((ActionGridItem)_collection[1]).Action);

            //Arbre attendu :
            //  G1 1
            //      T2 1.1
            //          T21 1.1.1
            //          T22 1.1.2
            //      T3 1.2
            //          T31 1.2.1
            //      T4 1.3

            InitItemsCollectionAssertions();
            AssertAction(_g1, 0, "1");
            AssertAction(_t2, 1, "1.1");
            AssertAction(_t21, 2, "1.1.1");
            AssertAction(_t22, 2, "1.1.2");
            AssertAction(_t3, 1, "1.2");
            AssertAction(_t31, 2, "1.2.1");
            AssertAction(_t4, 1, "1.3");
            EndItemsCollectionAssertions();
        }

        /// <summary>
        /// Supprimer T2
        ///</summary>
        [TestMethod()]
        public void DeleteTestT2()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4

            _manager.DeleteAction(((ActionGridItem)_collection[2]).Action);

            //Arbre attendu :
            //  G1 1
            //      T1 1.1
            //      T21 1.2
            //          T22 1.2.1
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4

            InitItemsCollectionAssertions();
            AssertAction(_g1, 0, "1");
            AssertAction(_t1, 1, "1.1");
            AssertAction(_t21, 2, "1.2");
            AssertAction(_t22, 2, "1.2.1");
            AssertAction(_t3, 1, "1.3");
            AssertAction(_t31, 2, "1.3.1");
            AssertAction(_t4, 1, "1.4");
            EndItemsCollectionAssertions();
        }

        /// <summary>
        /// Supprimer G1
        ///</summary>
        [TestMethod()]
        public void DeleteTestG1()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4

            _manager.DeleteAction(((ActionGridItem)_collection[0]).Action);

            //Arbre attendu :
            //  T1 1
            //      T2 1.1
            //          T21 1.1.1
            //          T22 1.1.2
            //      T3 1.2
            //          T31 1.2.1
            //      T4 1.3

            InitItemsCollectionAssertions();
            AssertAction(_t1, 1, "1");
            AssertAction(_t2, 1, "1.1");
            AssertAction(_t21, 2, "1.1.1");
            AssertAction(_t22, 2, "1.1.2");
            AssertAction(_t3, 1, "1.2");
            AssertAction(_t31, 2, "1.2.1");
            AssertAction(_t4, 1, "1.3");
            EndItemsCollectionAssertions();
        }

        /// <summary>
        /// Teste la validité du scénario apres une suppression d'action.
        /// </summary>
        [TestMethod]
        public void TestDelete()
        {
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

            var t1 = new KAction
            {
                Label = "T1",
                WBS = "1",
                Start = 0,
                Finish = 1,
                BuildStart = 0,
                BuildFinish = 2,
            };

            var t2 = new KAction
            {
                Label = "T2",
                WBS = "2",
                Start = 0,
                Finish = 1,
                BuildStart = 0,
                BuildFinish = 2,
                Predecessors = new TrackableCollection<KAction>() { t1 },
            };

            var t4 = new KAction
            {
                Label = "T4",
                WBS = "4",
                Start = 0,
                Finish = 1,
                BuildStart = 0,
                BuildFinish = 1,
                Predecessors = new TrackableCollection<KAction>() { t1 },
            };

            var t3 = new KAction
            {
                Label = "T3",
                WBS = "3",
                Start = 0,
                Finish = 1,
                BuildStart = 0,
                BuildFinish = 2,
                Predecessors = new TrackableCollection<KAction>() { t2, t4 },
            };


            var actions = new List<KAction>() { t1, t2, t3, t4 };

            var collection = new BulkObservableCollection<DataTreeGridItem>();

            var manager = new GridActionsManager(collection, null, null);
            manager.ChangeView(GanttGridView.WBS, null);

            manager.RegisterInitialActions(actions);
            manager.FixPredecessorsSuccessorsTimings();

            Assert.AreEqual(4, t3.BuildStart);
            Assert.AreEqual(6, t3.BuildFinish);

            manager.DeleteAction(t2);

            Assert.AreEqual(3, t3.BuildStart);
            Assert.AreEqual(5, t3.BuildFinish);

        }

        /// <summary>
        /// Ajouter une nouvelle action en ayant sélectionné G1
        ///</summary>
        [TestMethod()]
        public void AddTestG1()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4

            var newAction = new KAction() { Label = "New" };
            var newItem = _manager.AddAction(newAction, ((ActionGridItem)_collection[0]));

            //Arbre attendu :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4
            //      New 1.5

            _actions = _manager.GetActionsSortedByWBS();

            InitItemsCollectionAssertions();
            AssertAction(_g1, 0, "1");
            AssertAction(_t1, 1, "1.1");
            AssertAction(_t2, 1, "1.2");
            AssertAction(_t21, 2, "1.2.1");
            AssertAction(_t22, 2, "1.2.2");
            AssertAction(_t3, 1, "1.3");
            AssertAction(_t31, 2, "1.3.1");
            AssertAction(_t4, 1, "1.4");
            AssertAction(newAction, 0, "2");
            EndItemsCollectionAssertions();
        }

        /// <summary>
        /// Vérifier que toutes les actions impossibles revoient bien FAlse.
        ///</summary>
        [TestMethod()]
        public void GroupTestImpossible()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4

            // Grouper G1
            Assert.IsTrue(_manager.CanGroup(new ActionGridItem[] 
            { 
                (ActionGridItem)_collection[0] 
            }));

            // Grouper G1 + T1
            Assert.IsTrue(_manager.CanGroup(new ActionGridItem[] 
            { 
                (ActionGridItem)_collection[0],
                (ActionGridItem)_collection[1],
            }));

            // Grouper T1
            Assert.IsTrue(_manager.CanGroup(new ActionGridItem[] 
            { 
                (ActionGridItem)_collection[1],
            }));

            // Grouper T1 + T3
            Assert.IsFalse(_manager.CanGroup(new ActionGridItem[] 
            { 
                (ActionGridItem)_collection[1],
                (ActionGridItem)_collection[5],
            }));

            // Grouper T22 + T3 + T31
            Assert.IsTrue(_manager.CanGroup(new ActionGridItem[] 
            { 
                (ActionGridItem)_collection[4],
                (ActionGridItem)_collection[5],
                (ActionGridItem)_collection[6],
            }));

            // Grouper T22 + T3 + T4
            Assert.IsTrue(_manager.CanGroup(new ActionGridItem[] 
            { 
                (ActionGridItem)_collection[4],
                (ActionGridItem)_collection[5],
                (ActionGridItem)_collection[7],
            }));
        }

        /// <summary>
        /// Grouper T1 et T2
        ///</summary>
        [TestMethod()]
        public void GroupTestT1T2()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4

            var groupItem = _manager.Group(new ActionGridItem[] 
            { 
                (ActionGridItem)_collection[1],
                (ActionGridItem)_collection[2] 
            });

            //Arbre attendu :
            //  G1 1
            //      GROUP 1.1
            //          T1 1.1.1
            //          T2 1.1.2
            //              T21 1.1.2.1
            //              T22 1.1.2.2
            //      T3 1.2
            //          T31 1.2.1
            //      T4 1.3

            _actions = _manager.GetActionsSortedByWBS();

            InitItemsCollectionAssertions();
            AssertAction(_g1, 0, "1");
            AssertAction(groupItem.Action, 1, "1.1");
            AssertAction(_t1, 2, "1.1.1");
            AssertAction(_t2, 2, "1.1.2");
            AssertAction(_t21, 3, "1.1.2.1");
            AssertAction(_t22, 3, "1.1.2.2");
            AssertAction(_t3, 1, "1.2");
            AssertAction(_t31, 2, "1.2.1");
            AssertAction(_t4, 1, "1.3");
            EndItemsCollectionAssertions();
        }

        /// <summary>
        /// Grouper T1 et T2 et T22
        ///</summary>
        [TestMethod()]
        public void GroupTestT1T2T22()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3
            //          T31 1.3.1
            //      T4 1.4

            var groupItem = _manager.Group(new ActionGridItem[] 
            { 
                (ActionGridItem)_collection[1],
                (ActionGridItem)_collection[2],
                (ActionGridItem)_collection[4],
            });

            //Arbre attendu :
            //  G1 1
            //      GROUP 1.1
            //          T1 1.1.1
            //          T2 1.1.2
            //              T21 1.1.2.1
            //              T22 1.1.2.2
            //      T3 1.2
            //          T31 1.2.1
            //      T4 1.3

            _actions = _manager.GetActionsSortedByWBS();

            InitItemsCollectionAssertions();
            AssertAction(_g1, 0, "1");
            AssertAction(groupItem.Action, 1, "1.1");
            AssertAction(_t1, 2, "1.1.1");
            AssertAction(_t2, 2, "1.1.2");
            AssertAction(_t21, 3, "1.1.2.1");
            AssertAction(_t22, 3, "1.1.2.2");
            AssertAction(_t3, 1, "1.2");
            AssertAction(_t31, 2, "1.2.1");
            AssertAction(_t4, 1, "1.3");
            EndItemsCollectionAssertions();
        }

        
    }
}
