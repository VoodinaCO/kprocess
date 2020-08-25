using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DlhSoft.Windows.Controls;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Business.Tests;

namespace KProcess.Ksmed.Presentation.Tests
{


    /// <summary>
    ///This is a test class for ActionsManagerTest and is intended
    ///to contain all ActionsManagerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GridActionsManagerReferentialViewTest
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

        [TestInitialize()]
        public void MyTestInitialize()
        {
            ServicesHelper.RegisterMockServices();

            _res1 = new EquipmentStandard() { Label = "Res1" };
            _res2 = new EquipmentStandard() { Label = "Res2" };

            _g1 = new KAction() { Label = "G1", WBS = "1", Resource = _res1 };
            _t1 = new KAction() { Label = "T1", WBS = "1.1", Resource = _res1 };
            _t2 = new KAction() { Label = "T2", WBS = "1.2", Resource = _res2 };
            _t21 = new KAction() { Label = "T21", WBS = "1.2.1", Resource = _res1 };
            _t22 = new KAction() { Label = "T22", WBS = "1.2.2", Resource = _res2 };
            _t3 = new KAction() { Label = "T3", WBS = "1.3", Resource = _res2 };

            _actions = new KAction[] { _g1, _t1, _t2, _t21, _t22, _t3 };

            _collection = new BulkObservableCollection<DataTreeGridItem>();

            _manager = new GridActionsManager(_collection, v => _currentItem = v, null);
            _manager.ChangeView(GanttGridView.Resource, null);
        }

        private Resource _res1;
        private Resource _res2;
        private KAction _g1;
        private KAction _t1;
        private KAction _t2;
        private KAction _t21;
        private KAction _t22;
        private KAction _t3;

        private KAction[] _actions;
        private BulkObservableCollection<DataTreeGridItem> _collection;
        private DataTreeGridItem _currentItem;
        private int _currentAssertIndex = 0;

        private GridActionsManager _manager;

        private void InitItemsCollectionAssertions(IActionReferential res)
        {
            _currentAssertIndex = _collection.IndexOf(_collection.OfType<ReferentialGridItem>().First(i => i.Referential == res));
        }

        private void AssertAction(KAction action, int indentation, string wbs)
        {
            var item = _collection[_currentAssertIndex] as ActionGridItem;
            Assert.IsNotNull(item);

            Assert.AreEqual(action, item.Action, "Action");
            Assert.AreEqual(indentation, item.Indentation, "Indentation");
            Assert.AreEqual(wbs, item.Action.WBS, "WBS");

            _currentAssertIndex++;
        }

        private void AssertReferential(IActionReferential refe, int indentation)
        {
            var item = _collection[_currentAssertIndex] as ReferentialGridItem;
            Assert.IsNotNull(item);

            Assert.AreEqual(refe, item.Referential);
            Assert.AreEqual(indentation, item.Indentation);

            _currentAssertIndex++;
        }

        private void EndItemsCollectionAssertions()
        {
            _currentAssertIndex = 0;
        }

        private void AssertWBS(KAction action, string wbs)
        {
            Assert.AreEqual(wbs, action.WBS);
        }

        private string DumpAllWBS(KAction[] actions = null)
        {
            if (actions == null)
                actions = _actions;
            var sb = new StringBuilder();
            foreach (var action in actions.OrderBy(a => a.WBS))
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
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3
            InitItemsCollectionAssertions(_res1);
            AssertReferential(_res1, 0);
            AssertAction(_g1, 1, "1");
            AssertAction(_t1, 2, "1.1");
            AssertAction(_t2, 2, "1.2");
            AssertAction(_t21, 3, "1.2.1");
            AssertReferential(_res2, 0);
            AssertAction(_g1, 1, "1");
            AssertAction(_t2, 2, "1.2");
            AssertAction(_t22, 3, "1.2.2");
            AssertAction(_t3, 2, "1.3");
            EndItemsCollectionAssertions();
        }

        [TestMethod()]
        public void RegisterInitialActionsWithNullReferentialTests()
        {
            // Arbre initial :
            //G1 1
            //  T1 1.1 REF1
            //  T2 1.2 
            //    T21 1.2.1 REF1
            //    T22 1.2.2 REF2
            //  T3 1.3 REF2
            //G2 2
            //  T4 2.1 REF1
            //  T5 2.2 REF2

            var ref1 = new Ref1Standard() { Label = "Ref1" };
            Ref1 ref2 = null;

            var g1 = new KAction() { Label = "G1", WBS = "1" };
            var t1 = new KAction() { Label = "T1", WBS = "1.1" };
            t1.Ref1.Add(new Ref1Action { Ref1 = ref1 });
            var t2 = new KAction() { Label = "T2", WBS = "1.2" };
            var t21 = new KAction() { Label = "T21", WBS = "1.2.1" };
            t21.Ref1.Add(new Ref1Action { Ref1 = ref1 });
            var t22 = new KAction() { Label = "T22", WBS = "1.2.2" };
            var t3 = new KAction() { Label = "T3", WBS = "1.3" };

            var g2 = new KAction() { Label = "G2", WBS = "2" };
            var t4 = new KAction() { Label = "T4", WBS = "2.1" };
            t4.Ref1.Add(new Ref1Action { Ref1 = ref1 });
            var t5 = new KAction() { Label = "T5", WBS = "2.2" };

            var actions = new KAction[] { g1, t1, t2, t21, t22, t3, g2, t4, t5 };

            var manager = new GridActionsManager(_collection, v => { }, null);
            manager.ChangeView(GanttGridView.Ref1, null);
            manager.RegisterInitialActions(actions);

            //Arbre attendu :
            //REF1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //    G2 2
            //        T4 2.1
            //REF NULL
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3
            //    G2 2
            //        T5 2.2

            InitItemsCollectionAssertions(ref1);
            AssertReferential(ref1, 0);
            AssertAction(g1, 1, "1");
            AssertAction(t1, 2, "1.1");
            AssertAction(t2, 2, "1.2");
            AssertAction(t21, 3, "1.2.1");
            AssertAction(g2, 1, "2");
            AssertAction(t4, 2, "2.1");
            AssertReferential(ref2, 0);
            AssertAction(g1, 1, "1");
            AssertAction(t2, 2, "1.2");
            AssertAction(t22, 3, "1.2.2");
            AssertAction(t3, 2, "1.3");
            AssertAction(g2, 1, "2");
            AssertAction(t5, 2, "2.2");
            EndItemsCollectionAssertions();
        }

        [TestMethod()]
        public void RegisterInitialActionsWithNullReferentialLevel2GroupsTests()
        {
            var ref1 = new Ref1Standard() { Label = "Ref1" };
            Ref1 ref2 = null;

            var g1 = new KAction() { Label = "G1", WBS = "1" };
            g1.Ref1.Add(new Ref1Action { Ref1 = ref1 });
            var t1 = new KAction() { Label = "T1", WBS = "1.1" };
            t1.Ref1.Add(new Ref1Action { Ref1 = ref1 });
            var t2 = new KAction() { Label = "T2", WBS = "1.2" };
            t2.Ref1.Add(new Ref1Action { Ref1 = ref1 });
            var t21 = new KAction() { Label = "T21", WBS = "1.2.1" };
            t21.Ref1.Add(new Ref1Action { Ref1 = ref1 });
            var t22 = new KAction() { Label = "T22", WBS = "1.2.2" };
            var t3 = new KAction() { Label = "T3", WBS = "1.3" };

            var actions = new KAction[] { g1, t1, t2, t21, t22, t3 };

            var manager = new GridActionsManager(_collection, v => { }, null);
            manager.ChangeView(GanttGridView.Ref1, null);
            manager.RegisterInitialActions(actions);

            //Arbre attendu :
            //REF1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //REF NULL
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            InitItemsCollectionAssertions(ref1);
            AssertReferential(ref1, 0);
            AssertAction(g1, 1, "1");
            AssertAction(t1, 2, "1.1");
            AssertAction(t2, 2, "1.2");
            AssertAction(t21, 3, "1.2.1");
            AssertReferential(ref2, 0);
            AssertAction(g1, 1, "1");
            AssertAction(t2, 2, "1.2");
            AssertAction(t22, 3, "1.2.2");
            AssertAction(t3, 2, "1.3");
            EndItemsCollectionAssertions();
        }

        [TestMethod()]
        public void RegisterInitialActionsUndefinedOrderTests()
        {
            // Arbre initial :
            // T1 1.1 NULL
            // T3 1.3 REF1
            // T3 1.3 REF2

            Ref1 refNull = null;
            var ref1 = new Ref1Standard() { Label = "Ref1" };
            var ref2 = new Ref1Standard() { Label = "Ref2" };

            var t1 = new KAction() { Label = "T1", WBS = "1" };
            var t2 = new KAction() { Label = "T2", WBS = "2" };
            t2.Ref1.Add(new Ref1Action { Ref1 = ref1 });
            var t3 = new KAction() { Label = "T3", WBS = "3" };
            t3.Ref1.Add(new Ref1Action { Ref1 = ref2 });

            var actions = new KAction[] { t1, t2, t3 };

            var manager = new GridActionsManager(_collection, v => { }, null);
            manager.ChangeView(GanttGridView.Ref1, null);
            manager.RegisterInitialActions(actions);

            //Arbre attendu :
            //REF1
            //    T2 2
            //REF1
            //    T3 3
            //REFNULL
            //    T1 1

            InitItemsCollectionAssertions(ref1);
            AssertReferential(ref1, 0);
            AssertAction(t2, 1, "2");
            AssertReferential(ref2, 0);
            AssertAction(t3, 1, "3");
            AssertReferential(refNull, 0);
            AssertAction(t1, 1, "1");
            EndItemsCollectionAssertions();
        }

        /// <summary>
        /// Monter G1 dans Res1
        /// </summary>
        [TestMethod()]
        public void MoveUpTestRes1G1()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            // WBS Initial
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            // On monte G1
            _manager.MoveUp((ActionGridItem)_collection[1]);

            // Arbre Attendu
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1

            // WBS Attendu
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            InitItemsCollectionAssertions(_res1);
            AssertReferential(_res1, 0);
            AssertAction(_g1, 1, "1");
            AssertAction(_t1, 2, "1.1");
            AssertAction(_t2, 2, "1.2");
            AssertAction(_t21, 3, "1.2.1");
            EndItemsCollectionAssertions();

            AssertWBS(_g1, "1");
            AssertWBS(_t1, "1.1");
            AssertWBS(_t2, "1.2");
            AssertWBS(_t21, "1.2.1");
            AssertWBS(_t22, "1.2.2");
            AssertWBS(_t3, "1.3");
        }

        /// <summary>
        /// Monter T1 dans Res1
        /// </summary>
        [TestMethod()]
        public void MoveUpTestRes1T1()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            // WBS Initial
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            // On monte T1
            _manager.MoveUp((ActionGridItem)_collection[2]);

            // Arbre Attendu
            //RES1
            //    T1 1
            //    G1 2
            //        T2 2.1
            //            T21 2.1.1

            // WBS Attendu
            //  T1 1
            //  G1 2
            //      T2 2.1
            //          T21 2.1.1
            //          T22 2.1.2
            //      T3 2.2

            InitItemsCollectionAssertions(_res1);
            AssertReferential(_res1, 0);
            AssertAction(_t1, 1, "1");
            AssertAction(_g1, 1, "2");
            AssertAction(_t2, 2, "2.1");
            AssertAction(_t21, 3, "2.1.1");
            EndItemsCollectionAssertions();

            AssertWBS(_t1, "1");
            AssertWBS(_g1, "2");
            AssertWBS(_t2, "2.1");
            AssertWBS(_t21, "2.1.1");
            AssertWBS(_t22, "2.1.2");
            AssertWBS(_t3, "2.2");
        }

        /// <summary>
        /// Monter T2 dans Res1
        /// </summary>
        [TestMethod()]
        public void MoveUpTestRes1T2()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            // WBS Initial
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            // On monte T2
            _manager.MoveUp((ActionGridItem)_collection[3]);

            // Arbre Attendu
            //RES1
            //    G1 1
            //        T2 1.1
            //        T1 1.2
            //            T21 1.2.1

            // WBS Attendu
            //  G1 1
            //      T2 1.1
            //      T1 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            InitItemsCollectionAssertions(_res1);
            AssertReferential(_res1, 0);
            AssertAction(_g1, 1, "1");
            AssertAction(_t2, 2, "1.1");
            AssertAction(_t1, 2, "1.2");
            AssertAction(_t21, 3, "1.2.1");
            EndItemsCollectionAssertions();

            AssertWBS(_g1, "1");
            AssertWBS(_t2, "1.1");
            AssertWBS(_t1, "1.2");
            AssertWBS(_t21, "1.2.1");
            AssertWBS(_t22, "1.2.2");
            AssertWBS(_t3, "1.3");
        }

        /// <summary>
        /// Monter T21 dans Res1
        /// </summary>
        [TestMethod()]
        public void MoveUpTestRes1T21()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            // WBS Initial
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            // On monte T21
            _manager.MoveUp((ActionGridItem)_collection[4]);

            // Arbre Attendu
            //RES1
            //    G1 1
            //        T1 1.1
            //            T21 1.1.1
            //        T2 1.2

            // WBS Attendu
            //  G1 1
            //      T1 1.1
            //          T21 1.1.1
            //      T2 1.2
            //          T22 1.2.1
            //      T3 1.3

            InitItemsCollectionAssertions(_res1);
            AssertReferential(_res1, 0);
            AssertAction(_g1, 1, "1");
            AssertAction(_t1, 2, "1.1");
            AssertAction(_t21, 3, "1.1.1");
            AssertAction(_t2, 2, "1.2");
            EndItemsCollectionAssertions();

            AssertWBS(_g1, "1");
            AssertWBS(_t1, "1.1");
            AssertWBS(_t21, "1.1.1");
            AssertWBS(_t2, "1.2");
            AssertWBS(_t3, "1.3");
        }

        /// <summary>
        /// Monter T2 dans Res2
        /// </summary>
        [TestMethod()]
        public void MoveUpTestRes2T2()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            // WBS Initial
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            // On monte G1
            _manager.MoveUp((ActionGridItem)_collection[7]);

            // Arbre Attendu
            //RES2
            //    T2 1
            //    G1 2
            //        T1 2.1
            //            T22 2.1.2
            //        T3 2.2

            // WBS Attendu
            //  T2 1
            //  G1 2
            //      T1 2.1
            //          T21 2.1.1
            //          T22 2.1.2
            //      T3 2.2

            InitItemsCollectionAssertions(_res2);
            AssertReferential(_res2, 0);
            AssertAction(_t2, 1, "1");
            AssertAction(_g1, 1, "2");
            AssertAction(_t1, 2, "2.1");
            AssertAction(_t22, 3, "2.1.2");
            AssertAction(_t3, 2, "2.2");
            EndItemsCollectionAssertions();

            AssertWBS(_t2, "1");
            AssertWBS(_g1, "2");
            AssertWBS(_t1, "2.1");
            AssertWBS(_t21, "2.1.1");
            AssertWBS(_t22, "2.1.2");
            AssertWBS(_t3, "2.2");
        }

        /// <summary>
        /// Monter T22 dans Res2
        /// </summary>
        [TestMethod()]
        public void MoveUpTestRes2T22()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            // WBS Initial
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            // On monte G1
            _manager.MoveUp((ActionGridItem)_collection[8]);

            // Arbre Attendu
            //RES2
            //    G1 1
            //        T22 1.2
            //        T2 1.3
            //        T3 1.4

            // WBS Attendu
            //  G1 1
            //      T1 1.1
            //      T22 1.2
            //      T2 1.3
            //          T21 1.3.1
            //      T3 1.4

            InitItemsCollectionAssertions(_res2);
            AssertReferential(_res2, 0);
            AssertAction(_g1, 1, "1");
            AssertAction(_t22, 2, "1.2");
            AssertAction(_t2, 2, "1.3");
            AssertAction(_t3, 2, "1.4");
            EndItemsCollectionAssertions();

            AssertWBS(_g1, "1");
            AssertWBS(_t1, "1.1");
            AssertWBS(_t22, "1.2");
            AssertWBS(_t2, "1.3");
            AssertWBS(_t21, "1.3.1");
            AssertWBS(_t3, "1.4");
        }

        /// <summary>
        /// Monter T3 dans Res2
        /// </summary>
        [TestMethod()]
        public void MoveUpTestRes2T3()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            // WBS Initial
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            // On monte G1
            _manager.MoveUp((ActionGridItem)_collection[9]);

            // Arbre Attendu
            //RES2
            //    G1 1
            //        T2 1.2
            //        T3 1.3
            //            T22 1.3.1

            // WBS Attendu
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //      T3 1.3
            //          T22 1.3.1

            InitItemsCollectionAssertions(_res2);
            AssertReferential(_res2, 0);
            AssertAction(_g1, 1, "1");
            AssertAction(_t2, 2, "1.2");
            AssertAction(_t3, 2, "1.3");
            AssertAction(_t22, 3, "1.3.1");
            EndItemsCollectionAssertions();

            AssertWBS(_g1, "1");
            AssertWBS(_t1, "1.1");
            AssertWBS(_t2, "1.2");
            AssertWBS(_t21, "1.2.1");
            AssertWBS(_t3, "1.3");
            AssertWBS(_t22, "1.3.1");
        }

        /// <summary>
        /// Descendre G1 dans Res1
        /// </summary>
        [TestMethod()]
        public void MoveDownTestRes1G1()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            // WBS Initial
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            // On monte G1
            _manager.MoveDown((ActionGridItem)_collection[1]);

            // Arbre Attendu
            //RES1
            //    T1 1
            //    G1 2
            //        T2 2.1
            //            T21 2.1.1

            // WBS Attendu
            //  T1 1
            //  G1 2
            //      T2 2.1
            //          T21 2.1.1
            //          T22 2.1.2
            //      T3 2.2

            InitItemsCollectionAssertions(_res1);
            AssertReferential(_res1, 0);
            AssertAction(_t1, 1, "1");
            AssertAction(_g1, 1, "2");
            AssertAction(_t2, 2, "2.1");
            AssertAction(_t21, 3, "2.1.1");
            EndItemsCollectionAssertions();

            AssertWBS(_t1, "1");
            AssertWBS(_g1, "2");
            AssertWBS(_t2, "2.1");
            AssertWBS(_t21, "2.1.1");
            AssertWBS(_t22, "2.1.2");
            AssertWBS(_t3, "2.2");
        }

        /// <summary>
        /// Descendre T21 dans Res1
        /// </summary>
        [TestMethod()]
        public void MoveDownTestRes1T21()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            // WBS Initial
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            // On monte G1
            _manager.MoveDown((ActionGridItem)_collection[4]);

            // Arbre Attendu
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1

            // WBS Attendu
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            InitItemsCollectionAssertions(_res1);
            AssertReferential(_res1, 0);
            AssertAction(_g1, 1, "1");
            AssertAction(_t1, 2, "1.1");
            AssertAction(_t2, 2, "1.2");
            AssertAction(_t21, 3, "1.2.1");
            EndItemsCollectionAssertions();

            AssertWBS(_g1, "1");
            AssertWBS(_t1, "1.1");
            AssertWBS(_t2, "1.2");
            AssertWBS(_t21, "1.2.1");
            AssertWBS(_t22, "1.2.2");
            AssertWBS(_t3, "1.3");
        }

        /// <summary>
        /// Désindenter G1 dans Res1
        /// </summary>
        [TestMethod()]
        public void MoveLeftTestRes1G1()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            // WBS Initial
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            // On monte G1
            _manager.MoveLeft((ActionGridItem)_collection[1]);

            // Arbre Attendu
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1

            // WBS Attendu
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            InitItemsCollectionAssertions(_res1);
            AssertReferential(_res1, 0);
            AssertAction(_g1, 1, "1");
            AssertAction(_t1, 2, "1.1");
            AssertAction(_t2, 2, "1.2");
            AssertAction(_t21, 3, "1.2.1");
            EndItemsCollectionAssertions();

            AssertWBS(_g1, "1");
            AssertWBS(_t1, "1.1");
            AssertWBS(_t2, "1.2");
            AssertWBS(_t21, "1.2.1");
            AssertWBS(_t22, "1.2.2");
            AssertWBS(_t3, "1.3");
        }

        /// <summary>
        /// Désindenter T1 dans Res1
        /// </summary>
        [TestMethod()]
        public void MoveLeftTestRes1T1()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            // WBS Initial
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            // On monte G1
            _manager.MoveLeft((ActionGridItem)_collection[2]);

            // Arbre Attendu
            //RES1
            //    G1 1
            //    T1 2
            //        T2 2.1
            //            T21 2.1.1

            // WBS Attendu
            //  G1 1
            //  T1 2
            //      T2 2.1
            //          T21 2.1.1
            //          T22 2.1.2
            //      T3 2.2

            InitItemsCollectionAssertions(_res1);
            AssertReferential(_res1, 0);
            AssertAction(_g1, 1, "1");
            AssertAction(_t1, 1, "2");
            AssertAction(_t2, 2, "2.1");
            AssertAction(_t21, 3, "2.1.1");
            EndItemsCollectionAssertions();

            AssertWBS(_g1, "1");
            AssertWBS(_t1, "2");
            AssertWBS(_t2, "2.1");
            AssertWBS(_t21, "2.1.1");
            AssertWBS(_t22, "2.1.2");
            AssertWBS(_t3, "2.2");
        }

        /// <summary>
        /// Désindenter T2 dans Res1
        /// </summary>
        [TestMethod()]
        public void MoveLeftTestRes1T2()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            // WBS Initial
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            // On monte G1
            _manager.MoveLeft((ActionGridItem)_collection[3]);

            // Arbre Attendu
            //RES1
            //    G1 1
            //        T1 1.1
            //    T2 2
            //        T21 2.1

            // WBS Attendu
            //  G1 1
            //      T1 1.1
            //  T2 2
            //      T21 2.1
            //      T22 2.2
            //      T3 2.3

            InitItemsCollectionAssertions(_res1);
            AssertReferential(_res1, 0);
            AssertAction(_g1, 1, "1");
            AssertAction(_t1, 2, "1.1");
            AssertAction(_t2, 1, "2");
            AssertAction(_t21, 2, "2.1");
            EndItemsCollectionAssertions();

            AssertWBS(_g1, "1");
            AssertWBS(_t1, "1.1");
            AssertWBS(_t2, "2");
            AssertWBS(_t21, "2.1");
            AssertWBS(_t22, "2.2");
            AssertWBS(_t3, "2.3");
        }

        /// <summary>
        /// Désindenter T21 dans Res1
        /// </summary>
        [TestMethod()]
        public void MoveLeftTestRes1T21()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            // WBS Initial
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            // On monte G1
            _manager.MoveLeft((ActionGridItem)_collection[4]);

            // Arbre Attendu
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //        T21 1.3

            // WBS Attendu
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //      T21 1.3
            //          T22 1.3.1
            //      T3 1.4

            InitItemsCollectionAssertions(_res1);
            AssertReferential(_res1, 0);
            AssertAction(_g1, 1, "1");
            AssertAction(_t1, 2, "1.1");
            AssertAction(_t2, 2, "1.2");
            AssertAction(_t21, 2, "1.3");
            EndItemsCollectionAssertions();

            AssertWBS(_g1, "1");
            AssertWBS(_t1, "1.1");
            AssertWBS(_t2, "1.2");
            AssertWBS(_t21, "1.3");
            AssertWBS(_t22, "1.3.1");
            AssertWBS(_t3, "1.4");
        }

        /// <summary>
        /// Indenter G1 dans Res1
        /// </summary>
        [TestMethod()]
        public void MoveRightTestRes1G1()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            // WBS Initial
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            // On monte G1
            _manager.MoveRight((ActionGridItem)_collection[1]);

            // Arbre Attendu
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1

            // WBS Attendu
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            InitItemsCollectionAssertions(_res1);
            AssertReferential(_res1, 0);
            AssertAction(_g1, 1, "1");
            AssertAction(_t1, 2, "1.1");
            AssertAction(_t2, 2, "1.2");
            AssertAction(_t21, 3, "1.2.1");
            EndItemsCollectionAssertions();

            AssertWBS(_g1, "1");
            AssertWBS(_t1, "1.1");
            AssertWBS(_t2, "1.2");
            AssertWBS(_t21, "1.2.1");
            AssertWBS(_t22, "1.2.2");
            AssertWBS(_t3, "1.3");
        }

        /// <summary>
        /// Indenter G1 dans Res1
        /// </summary>
        [TestMethod()]
        public void MoveRightTestRes1T1()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            // WBS Initial
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            // On monte G1
            _manager.MoveRight((ActionGridItem)_collection[2]);

            // Arbre Attendu
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1

            // WBS Attendu
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            InitItemsCollectionAssertions(_res1);
            AssertReferential(_res1, 0);
            AssertAction(_g1, 1, "1");
            AssertAction(_t1, 2, "1.1");
            AssertAction(_t2, 2, "1.2");
            AssertAction(_t21, 3, "1.2.1");
            EndItemsCollectionAssertions();

            AssertWBS(_g1, "1");
            AssertWBS(_t1, "1.1");
            AssertWBS(_t2, "1.2");
            AssertWBS(_t21, "1.2.1");
            AssertWBS(_t22, "1.2.2");
            AssertWBS(_t3, "1.3");
        }

        /// <summary>
        /// Indenter T3 dans Res2
        /// </summary>
        [TestMethod()]
        public void MoveRightTestRes2T3()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            // WBS Initial
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            // On monte G1
            _manager.MoveRight((ActionGridItem)_collection[9]);

            // Arbre Attendu
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //            T3 1.2.3

            // WBS Attendu
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //          T3 1.2.3

            InitItemsCollectionAssertions(_res2);
            AssertReferential(_res2, 0);
            AssertAction(_g1, 1, "1");
            AssertAction(_t2, 2, "1.2");
            AssertAction(_t22, 3, "1.2.2");
            AssertAction(_t3, 3, "1.2.3");
            EndItemsCollectionAssertions();

            AssertWBS(_g1, "1");
            AssertWBS(_t1, "1.1");
            AssertWBS(_t2, "1.2");
            AssertWBS(_t21, "1.2.1");
            AssertWBS(_t22, "1.2.2");
            AssertWBS(_t3, "1.2.3");
        }


        /// <summary>
        /// Ajouter une nouvelle action en ne sélectionnant rien.
        /// </summary>
        [TestMethod()]
        public void AddTest()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            // WBS Initial
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            var a = new KAction() { Label = "New" };
            var newItem = _manager.AddAction(a);

            // Arbre Attendu
            //  Sans Ressource
            //      New

            // WBS Attendu
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3
            //  New 2

            var resSansRes = _collection.OfType<ReferentialGridItem>().First(i => i.Referential == null);

            InitItemsCollectionAssertions(resSansRes.Referential);
            AssertReferential(resSansRes.Referential, 0);
            AssertAction(newItem.Action, 1, "2");
            EndItemsCollectionAssertions();

            AssertWBS(_g1, "1");
            AssertWBS(_t1, "1.1");
            AssertWBS(_t2, "1.2");
            AssertWBS(_t21, "1.2.1");
            AssertWBS(_t22, "1.2.2");
            AssertWBS(_t3, "1.3");
            AssertWBS(newItem.Action, "2");
        }

        /// <summary>
        /// Ajouter une nouvelle action en sélectionnant RES1.
        /// </summary>
        [TestMethod()]
        public void AddTestRes1()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            // WBS Initial
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            var a = new KAction() { Label = "New" };
            var newItem = _manager.AddAction(a, _collection[0]);

            // Arbre Attendu
            //  RES1
            //      G1 1
            //          T1 1.1
            //          T2 1.2
            //              T21 1.2.1
            //      New 2

            // WBS Attendu
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3
            //  New 2

            InitItemsCollectionAssertions(_res1);
            AssertReferential(_res1, 0);
            AssertAction(_g1, 1, "1");
            AssertAction(_t1, 2, "1.1");
            AssertAction(_t2, 2, "1.2");
            AssertAction(_t21, 3, "1.2.1");
            AssertAction(newItem.Action, 1, "2");
            EndItemsCollectionAssertions();

            AssertWBS(_g1, "1");
            AssertWBS(_t1, "1.1");
            AssertWBS(_t2, "1.2");
            AssertWBS(_t21, "1.2.1");
            AssertWBS(_t22, "1.2.2");
            AssertWBS(_t3, "1.3");
            AssertWBS(newItem.Action, "2");
        }

        /// <summary>
        /// Ajouter une nouvelle action en sélectionnant G1 dans Res1.
        /// </summary>
        [TestMethod()]
        public void AddTestRes1G1()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            // WBS Initial
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            var a = new KAction() { Label = "New" };
            var newItem = _manager.AddAction(a, _collection[1]);

            // Arbre Attendu
            //  RES1
            //      G1 1
            //          T1 1.1
            //          T2 1.2
            //              T21 1.2.1
            //      New 2

            // WBS Attendu
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3
            //  New 2

            InitItemsCollectionAssertions(_res1);
            AssertReferential(_res1, 0);
            AssertAction(_g1, 1, "1");
            AssertAction(_t1, 2, "1.1");
            AssertAction(_t2, 2, "1.2");
            AssertAction(_t21, 3, "1.2.1");
            AssertAction(newItem.Action, 1, "2");
            EndItemsCollectionAssertions();

            AssertWBS(_g1, "1");
            AssertWBS(_t1, "1.1");
            AssertWBS(_t2, "1.2");
            AssertWBS(_t21, "1.2.1");
            AssertWBS(_t22, "1.2.2");
            AssertWBS(_t3, "1.3");
            AssertWBS(newItem.Action, "2");
        }

        /// <summary>
        /// Ajouter une nouvelle action en sélectionnant T1 dans Res1.
        /// </summary>
        [TestMethod()]
        public void AddTestRes1T1()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            // WBS Initial
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            var a = new KAction() { Label = "New" };
            var newItem = _manager.AddAction(a, _collection[2]);

            // Arbre Attendu
            //  RES1
            //      G1 1
            //          T1 1.1
            //          New 1.2
            //          T2 1.3
            //              T21 1.3.1

            // WBS Attendu
            //  G1 1
            //      T1 1.1
            //      New 1.2
            //      T2 1.3
            //          T21 1.3.1
            //          T22 1.3.2
            //      T3 1.4

            _actions = _manager.GetActionsSortedByWBS();

            InitItemsCollectionAssertions(_res1);
            AssertReferential(_res1, 0);
            AssertAction(_g1, 1, "1");
            AssertAction(_t1, 2, "1.1");
            AssertAction(newItem.Action, 2, "1.2");
            AssertAction(_t2, 2, "1.3");
            AssertAction(_t21, 3, "1.3.1");
            EndItemsCollectionAssertions();

            AssertWBS(_g1, "1");
            AssertWBS(_t1, "1.1");
            AssertWBS(newItem.Action, "1.2");
            AssertWBS(_t2, "1.3");
            AssertWBS(_t21, "1.3.1");
            AssertWBS(_t22, "1.3.2");
            AssertWBS(_t3, "1.4");
        }

        /// <summary>
        /// Ajouter une nouvelle action en sélectionnant T3 dans Res2.
        /// </summary>
        [TestMethod()]
        public void AddTestRes2T2()
        {
            _manager.RegisterInitialActions(_actions);

            //Arbre initial :
            //RES1
            //    G1 1
            //        T1 1.1
            //        T2 1.2
            //            T21 1.2.1
            //RES2
            //    G1 1
            //        T2 1.2
            //            T22 1.2.2
            //        T3 1.3

            // WBS Initial
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3

            var a = new KAction() { Label = "New" };
            var newItem = _manager.AddAction(a, _collection[9]);

            // Arbre Attendu
            //  RES2
            //      G1 1
            //          T2 1.2
            //              T22 1.2.2
            //          T3 1.3
            //          New 1.4

            // WBS Attendu
            //  G1 1
            //      T1 1.1
            //      T2 1.2
            //          T21 1.2.1
            //          T22 1.2.2
            //      T3 1.3
            //      New 1.4

            _actions = _manager.GetActionsSortedByWBS();

            InitItemsCollectionAssertions(_res2);
            AssertReferential(_res2, 0);
            AssertAction(_g1, 1, "1");
            AssertAction(_t2, 2, "1.2");
            AssertAction(_t22, 3, "1.2.2");
            AssertAction(_t3, 2, "1.3");
            AssertAction(newItem.Action, 2, "1.4");
            EndItemsCollectionAssertions();

            AssertWBS(_g1, "1");
            AssertWBS(_t1, "1.1");
            AssertWBS(_t2, "1.2");
            AssertWBS(_t21, "1.2.1");
            AssertWBS(_t22, "1.2.2");
            AssertWBS(_t3, "1.3");
            AssertWBS(newItem.Action, "1.4");
        }
    }
}
