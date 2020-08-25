using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using DlhSoft.Windows.Controls;
using KProcess.Ksmed.Business.Impl;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Business.Tests;
using KProcess.Ksmed.Business.Tests;

namespace KProcess.Ksmed.Presentation.Tests
{
    /// <summary>
    /// Tests sur CancelChanges des entités.
    /// </summary>
    [TestClass]
    public class EntitiesCancelChangesTests
    {

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            ServicesHelper.RegisterMockServices();
        }

        /// <summary>
        /// Teste l'annulation sur une grille de gantt
        /// </summary>
        [TestMethod]
        public void GanttGridTests()
        {
            KProcess.Ksmed.Business.Tests.SampleData.ClearDatabaseThenImportDefaultProject();

            // On récupère les données
            var service = new AnalyzeService();

            var mre = new ManualResetEvent(false);

            var collection = new BulkObservableCollection<DataTreeGridItem>();
            var actionsManager = new GridActionsManager(collection, null, null);
            actionsManager.ChangeView(Core.GanttGridView.WBS, null);

            var categories = new BulkObservableCollection<ActionCategory>();
            Video[] videos = null;
            Scenario[] scenarios = null;
            Scenario scenario = null;

            Exception ex = null;

            service.GetAcquireData(SampleData.GetProjectId(),
                data =>
                {
                    categories.AddRange(data.Categories);
                    videos = data.Videos;

                    scenarios = data.Scenarios;

                    scenario = scenarios.First();

                    foreach (var action in scenario.Actions)
                        action.StartTracking();

                    mre.Set();
                },
                e =>
                {
                    ex = e;
                    mre.Set();
                });

            mre.WaitOne();

            AssertExt.IsExceptionNull(ex);

            actionsManager.RegisterInitialActions(scenario.Actions);

            Assert.IsTrue(scenario.Actions.Count > 1);
            Assert.IsTrue(collection.Count > 1);
            Assert.IsTrue(videos.Length > 0);
            Assert.IsTrue(scenario.Actions[1].Video != null);

            // Capturons l'état de toutes les éléments
            var originalValues = GetCurrentValues(scenario.Actions, categories, videos);

            Modify(actionsManager, collection, scenario);

            var modifiedValues = GetCurrentValues(scenario.Actions, categories, videos);

            Assert.IsFalse(AreDumpsEqual(originalValues, modifiedValues));

            // On annule les changements
            actionsManager.UnregisterAllItems();
            ObjectWithChangeTrackerExtensions.CancelChanges(scenario.Actions, categories, videos);

            var revertedValues = GetCurrentValues(scenario.Actions, categories, videos);

            Assert.IsTrue(AreDumpsEqual(originalValues, revertedValues));




            // On recommence
            actionsManager.RegisterInitialActions(scenario.Actions);
            Modify(actionsManager, collection, scenario);

            modifiedValues = GetCurrentValues(scenario.Actions, categories, videos);

            Assert.IsFalse(AreDumpsEqual(originalValues, modifiedValues));

            // On annule les changements
            actionsManager.UnregisterAllItems();
            ObjectWithChangeTrackerExtensions.CancelChanges(scenario.Actions, categories, videos);

            revertedValues = GetCurrentValues(scenario.Actions, categories, videos);

            Assert.IsTrue(AreDumpsEqual(originalValues, revertedValues));
        }

        private void Modify(GridActionsManager actionsManager, BulkObservableCollection<DataTreeGridItem> collection, Scenario scenario)
        {
            // On va descendre la première tache
            actionsManager.MoveDown((ActionGridItem)collection[0]);

            // On change la vidéo sur la première tache
            scenario.Actions[0].Video = null;

            // On vide les prédecesseurs de T2
            scenario.Actions[1].Predecessors.Clear();

            // Ajouter un prédécesseur à T3
            scenario.Actions[2].Predecessors.Add(scenario.Actions[0]);
        }

        /// <summary>
        /// Obtient un dump des valeurs courantes pour les entités spécifiées.
        /// </summary>
        /// <param name="entitiesCollections">Les énumérations d'entités.</param>
        /// <returns>Un dump</returns>
        private Dictionary<IObjectWithChangeTracker, object[]> GetCurrentValues(params IEnumerable<IObjectWithChangeTracker>[] entitiesCollections)
        {
            var values = new Dictionary<IObjectWithChangeTracker, object[]>();
            foreach (var collection in entitiesCollections)
                foreach (var entity in collection)
                    values[entity] = entity.GetCurrentValues().Values.ToArray();
            return values;
        }

        private bool AreDumpsEqual(Dictionary<IObjectWithChangeTracker, object[]> dump1, Dictionary<IObjectWithChangeTracker, object[]> dump2)
        {
            if (dump1.Count != dump2.Count)
                return false;

            foreach (var kvp1 in dump1)
            {
                var entity = kvp1.Key;

                if (!dump2.ContainsKey(entity))
                    return false;

                var valuesEntity1 = kvp1.Value;
                var valuesEntity2 = dump2[entity];

                if (valuesEntity1.Length != valuesEntity2.Length)
                    return false;

                for (int i = 0; i < valuesEntity1.Length; i++)
                {
                    var value1 = valuesEntity1[i];
                    var value2 = valuesEntity2[i];

                    if (value1 is int[] && value2 is int[])
                    {
                        // Il s'agit d'une collection
                        // Les éléments ne sont pas nécessairement dans le même ordre

                        var array1 = (int[])value1;
                        var array2 = (int[])value2;

                        if (array1.Intersect(array2).Count() != array1.Length)
                        {
                            ShowValueDifferentError(entity, valuesEntity1, valuesEntity2, i);
                            return false;
                        }

                    }
                    else if (value1 is IComparable && value2 is IComparable)
                    {
                        var comparable1 = (IComparable)value1;
                        var comparable2 = (IComparable)value2;

                        if (comparable1.CompareTo(comparable2) != 0)
                        {
                            ShowValueDifferentError(entity, valuesEntity1, valuesEntity2, i);
                            return false;
                        }
                    }
                    else if (value1 != value2)
                    {
                        ShowValueDifferentError(entity, valuesEntity1, valuesEntity2, i);
                        return false;
                    }
                }

            }

            return true;
        }

        [Conditional("DEBUG")]
        private void ShowValueDifferentError(IObjectWithChangeTracker obj, object[] values1, object[] values2, int propertyIndex)
        {
            System.Diagnostics.Debug.WriteLine("Sur l'objet {0}, la valeur de la propriété {1} vaut '{2}' et '{3}'",
                obj, obj.GetCurrentValues().Keys.ElementAt(propertyIndex), values1[propertyIndex], values2[propertyIndex]);
        }
    }
}
