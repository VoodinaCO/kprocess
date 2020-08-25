using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Business.Tests;
using DlhSoft.Windows.Controls;
using KProcess.Ksmed.Business;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Business.Dtos.Export;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KProcess.Ksmed.Presentation.Tests
{
    /// <summary>
    /// Permet de faire des tests divers sur l'action manager.
    /// </summary>
    [TestClass]
    public class GridActionsManagerMiscTests
    {

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            ServicesHelper.RegisterMockServices();
        }

        private void AssertWBS(KAction action, string wbs)
        {
            Assert.AreEqual(wbs, action.WBS);
        }

        [TestMethod]
        public void TestFixWBS()
        {

            var g1 = new KAction() { Label = "G1", WBS = "1" };
            var t1 = new KAction() { Label = "T1", WBS = "1.2" };
            var t2 = new KAction() { Label = "T2", WBS = "1.3" };
            var t21 = new KAction() { Label = "T21", WBS = "1.3.0" };
            var t22 = new KAction() { Label = "T22", WBS = "1.3.2" };

            var actions = new List<KAction>() { g1, t1, t2, t21, t22 };

            var collection = new BulkObservableCollection<DataTreeGridItem>();

            var manager = new GridActionsManager(collection, null, null);
            manager.ChangeView(GanttGridView.WBS, null);

            manager.RegisterInitialActions(actions);

            manager.FixAllWBS();

            AssertWBS(g1, "1");
            AssertWBS(t1, "1.1");
            AssertWBS(t2, "1.2");
            AssertWBS(t21, "1.2.1");
            AssertWBS(t22, "1.2.2");

        }

        // Lié au bug 749
        // Déplacer 1.2 à droite dans :

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
        public void TestMoveRight()
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

            var collection = new BulkObservableCollection<DataTreeGridItem>();

            var manager = new GridActionsManager(collection, null, null);
            manager.ChangeView(GanttGridView.WBS, null);

            manager.RegisterInitialActions(actions);

            manager.MoveRight((ActionGridItem)collection[3]);

            AssertWBS(g1, "1");
            AssertWBS(t1, "1.1");
            AssertWBS(t11, "1.1.1");

            AssertWBS(t2, "1.1.2");
            AssertWBS(t3, "1.2");
            AssertWBS(t4, "1.3");
            AssertWBS(xcvb, "2");
            AssertWBS(xbc, "3");
            AssertWBS(vcxbv, "3.1");

        }


        /// <summary>
        /// Test du chemin critique
        /// </summary>
        [TestMethod]
        public void TestGetCriticalPath1()
        {
            var g1 = new KAction() { Label = "G1", WBS = "1" };
            var t1 = new KAction() { Label = "T1", WBS = "1.1", BuildStart = 0, BuildDuration = 2 };
            var t2 = new KAction() { Label = "T2", WBS = "1.2" };
            var t21 = new KAction() { Label = "T21", WBS = "1.2.1", BuildStart = 0, BuildDuration = 2 };
            var t22 = new KAction() { Label = "T22", WBS = "1.2.2", BuildStart = 0, BuildDuration = 2 };

            var actions = new List<KAction>() { g1, t1, t2, t21, t22 };

            t22.Predecessors.Add(t1);
            t21.Predecessors.Add(t22);

            var collection = new BulkObservableCollection<DataTreeGridItem>();

            var manager = new GridActionsManager(collection, null, null);
            manager.ChangeView(GanttGridView.WBS, null);

            manager.RegisterInitialActions(actions);
            manager.FixPredecessorsSuccessorsTimings();

            var actual = manager.UpdateCriticalPath().ToList();
            //var actual = manager.GetCriticalPathv2().ToList();
            var expected = new List<ActionPath>() 
            {
                actual.First(c => c.Action == t1),
                actual.First(c => c.Action == t22),
                actual.First(c => c.Action == t21)
            };

            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test du chemin critique, avec des successeurs multiples
        /// </summary>
        [TestMethod]
        public void TestGetCriticalPath2()
        {
            // Successeurs multiples
            var t1 = new KAction() { Label = "T1", WBS = "1", BuildStart = 0, BuildDuration = 2 };
            var t2 = new KAction() { Label = "T2", WBS = "2", BuildStart = 0, BuildDuration = 3 };
            var t3 = new KAction() { Label = "T3", WBS = "3", BuildStart = 0, BuildDuration = 4 };
            var t4 = new KAction() { Label = "T4", WBS = "4", BuildStart = 0, BuildDuration = 2 };
            var t5 = new KAction() { Label = "T5", WBS = "5", BuildStart = 0, BuildDuration = 2 };

            var actions = new List<KAction>() { t1, t2, t3, t4, t5 };

            t2.Predecessors.Add(t1);
            t3.Predecessors.Add(t1);
            t4.Predecessors.Add(t2);
            t5.Predecessors.Add(t3);

            var collection = new BulkObservableCollection<DataTreeGridItem>();

            var manager = new GridActionsManager(collection, null, null);
            manager.ChangeView(GanttGridView.WBS, null);

            manager.RegisterInitialActions(actions);
            manager.FixPredecessorsSuccessorsTimings();

            var actual = manager.UpdateCriticalPath().ToList();
            var expected = new List<ActionPath>() 
            { 
                actual.First(c => c.Action == t1),
                actual.First(c => c.Action == t3),
                actual.First(c => c.Action == t5)
            };

            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test du chemin critique, avec des prédecesseurs multiples
        /// </summary>
        [TestMethod]
        public void TestGetCriticalPath3()
        {
            // Prédecesseurs multiples
            var t1 = new KAction() { Label = "T1", WBS = "1", BuildStart = 0, BuildDuration = 2 };
            var t2 = new KAction() { Label = "T2", WBS = "2", BuildStart = 0, BuildDuration = 3 };
            var t3 = new KAction() { Label = "T3", WBS = "3", BuildStart = 0, BuildDuration = 4 };
            var t4 = new KAction() { Label = "T4", WBS = "4", BuildStart = 0, BuildDuration = 2 };
            var t5 = new KAction() { Label = "T5", WBS = "5", BuildStart = 0, BuildDuration = 2 };

            var actions = new List<KAction>() { t1, t2, t3, t4, t5 };

            t3.Predecessors.Add(t1);
            t3.Predecessors.Add(t2);
            t4.Predecessors.Add(t3);
            t5.Predecessors.Add(t3);

            var collection = new BulkObservableCollection<DataTreeGridItem>();

            var manager = new GridActionsManager(collection, null, null);
            manager.ChangeView(GanttGridView.WBS, null);

            manager.RegisterInitialActions(actions);
            manager.FixPredecessorsSuccessorsTimings();

            var actual = manager.UpdateCriticalPath().ToList();
            var expected = new List<ActionPath>() 
            { 
                actual.First(c => c.Action == t2),
                actual.First(c => c.Action == t3),
                actual.First(c => c.Action == t4)
            };

            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test du chemin critique, pour correction du bug 1545, avec des tâches qui devraient reproduire le dysfonctionnement
        /// </summary>
        [TestMethod]
        public void TestGetCriticalPath_Bug1545_Equivalent()
        {

            var t1 = new KAction() { Label = "T1", WBS = "1", BuildStart = 0, BuildDuration = 1 };
            var t2 = new KAction() { Label = "T2", WBS = "2", BuildStart = 0, BuildDuration = 0 };
            var t3 = new KAction() { Label = "T3", WBS = "3", BuildStart = 2, BuildDuration = 1 };

            var actions = new List<KAction>() { t1, t2, t3 };

            t2.Predecessors.Add(t1);

            var collection = new BulkObservableCollection<DataTreeGridItem>();

            var manager = new GridActionsManager(collection, null, null);
            manager.ChangeView(GanttGridView.WBS, null);

            manager.RegisterInitialActions(actions);
            manager.FixPredecessorsSuccessorsTimings();

            var actual = manager.UpdateCriticalPath().ToList();
            var expected = new List<ActionPath>() 
            { 
                actual.First(c => c.Action == t1),
                actual.First(c => c.Action == t2),
                actual.First(c => c.Action == t3)
            };

            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Teste si une prédécesseur peut être ajouté.
        /// </summary>
        [TestMethod]
        public void TestCanAddPredecessor()
        {
            var t1 = new KAction() { Label = "T1", WBS = "1" };
            var g1 = new KAction() { Label = "G1", WBS = "2" };
            var t2 = new KAction() { Label = "T2", WBS = "2.1" };
            var t3 = new KAction() { Label = "T3", WBS = "2.2" };
            var t4 = new KAction() { Label = "T4", WBS = "3" };
            var t5 = new KAction() { Label = "T5", WBS = "4" };

            var actions = new List<KAction>() { t1, g1, t2, t3, t4, t5 };

            t3.Predecessors.Add(t1);
            t3.Predecessors.Add(t2);
            t4.Predecessors.Add(t3);
            t5.Predecessors.Add(t3);

            var collection = new BulkObservableCollection<DataTreeGridItem>();

            var manager = new GridActionsManager(collection, null, null);
            manager.ChangeView(GanttGridView.WBS, null);

            manager.RegisterInitialActions(actions);

            Assert.IsFalse(ActionsTimingsMoveManagement.CheckCanAddPredecessor(manager.GetActionsSortedByWBS(), t1, t1));
            Assert.IsFalse(ActionsTimingsMoveManagement.CheckCanAddPredecessor(manager.GetActionsSortedByWBS(), t1, t3));
            Assert.IsFalse(ActionsTimingsMoveManagement.CheckCanAddPredecessor(manager.GetActionsSortedByWBS(), g1, t3));

            Assert.IsTrue(ActionsTimingsMoveManagement.CheckCanAddPredecessor(manager.GetActionsSortedByWBS(), t4, t1));
            Assert.IsTrue(ActionsTimingsMoveManagement.CheckCanAddPredecessor(manager.GetActionsSortedByWBS(), t5, t1));
            Assert.IsTrue(ActionsTimingsMoveManagement.CheckCanAddPredecessor(manager.GetActionsSortedByWBS(), t2, t1));

        }

        /// <summary>
        /// Charge un KSP se trouvant dans le dossier Resources et étant Embedded Resource.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        internal static ProjectExport LoadKsp(string fileName)
        {
            var file = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("KProcess.Ksmed.Presentation.Tests.Resources." + fileName);

            ProjectExport project;

            using (var rdr = XmlDictionaryReader.CreateTextReader(file, XmlDictionaryReaderQuotas.Max))
            {
                var ser = new NetDataContractSerializer()
                {
                    Binder = new AnyVersionSerializationBinder(),
                };
                project = (ProjectExport)ser.ReadObject(rdr);
            }

            file.Close();

            return project;
        }


        /// <summary>
        /// Représente un lieur de sérialisation qui ne prend pas en compte la version de l'assembly pour le chargement des types.
        /// </summary>
        private class AnyVersionSerializationBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                var an = new System.Reflection.AssemblyName(assemblyName);
                an.Version = null;
                return System.Reflection.Assembly.Load(an.ToString()).GetType(typeName);
            }
        }
    }
}
