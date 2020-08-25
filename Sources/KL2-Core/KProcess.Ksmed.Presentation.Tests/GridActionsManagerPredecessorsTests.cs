using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DlhSoft.Windows.Controls;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Business.Tests;

namespace KProcess.Ksmed.Presentation.Tests
{
    /// <summary>
    /// Permet de réaliser des tests sur la gestion des temps avec des prédecesseurs/successeurs.
    /// </summary>
    [TestClass]
    public class GridActionsManagerPredecessorsTests
    {
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            ServicesHelper.RegisterMockServices();
        }

        private List<KAction> _actions = new List<KAction>();
        private BulkObservableCollection<DataTreeGridItem> _collection;
        private int _assertIndex = 0;

        /// <summary>
        /// Crée une action.
        /// </summary>
        /// <param name="duration">La durée de l'action.</param>
        /// <param name="predecessorIndexes">Les index des prédécesseurs.</param>
        private void CreateAction(long duration, params int[] predecessorIndexes)
        {
            var wbs = (_actions.Count + 1).ToString();
            var action = new KAction() { Label = wbs, WBS = wbs, BuildDuration = duration };
            foreach (var index in predecessorIndexes)
                action.Predecessors.Add(_actions[index]);

            _actions.Add(action);
        }

        /// <summary>
        /// Crée l'ActionManager.
        /// </summary>
        private void CreateManager()
        {
            _collection = new BulkObservableCollection<DataTreeGridItem>();

            var manager = new GridActionsManager(_collection, null, null);
            manager.ChangeView(GanttGridView.WBS, null);

            manager.RegisterInitialActions(_actions);
            manager.FixPredecessorsSuccessorsTimings();
        }

        /// <summary>
        /// Vérifie qu'une action démarre bien à l'endroit voulu.
        /// </summary>
        /// <param name="start">Le départ.</param>
        private void AssertAction(long start)
        {
            Assert.AreEqual(start, _actions[_assertIndex].BuildStart);
            _assertIndex++;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _actions.Clear();
            _assertIndex = 0;
        }

        [TestMethod]
        public void TestCheckTimings1()
        {
            CreateAction(2);
            CreateAction(3, 0);
            CreateAction(4, 0);

            CreateManager();

            AssertAction(0);
            AssertAction(2);
            AssertAction(2);
        }

        [TestMethod]
        public void TestCheckTimings2()
        {
            CreateAction(2);
            CreateAction(3, 0);
            CreateAction(4, 0, 1);

            CreateManager();

            AssertAction(0);
            AssertAction(2);
            AssertAction(5);
        }


        [TestMethod]
        public void TestCheckTimings3()
        {
            CreateAction(1);
            CreateAction(2);
            CreateAction(3, 0, 1);
            CreateAction(4, 2);
            CreateAction(5, 2);
            CreateAction(6, 0, 1, 2, 3, 4);

            CreateManager();

            AssertAction(0);
            AssertAction(0);
            AssertAction(2);
            AssertAction(5);
            AssertAction(5);
            AssertAction(10);
        }

    }
}
