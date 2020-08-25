using KProcess.Ksmed.Business.Impl;
using KProcess.Ksmed.Business.Tests.Helper;
using KProcess.Ksmed.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Business.Tests
{
    [TestClass]
    public class SharedScenarioActionsOperationsTests
    {

        private const string Add_Action_Original_ProjectName = "Test_Add_Action_Original";

        /// <summary>
        /// Teste l'ajout d'une nouvelle action.
        /// Vérifie que l'action des scénarios dérivés aura bien l'action du scénario parent en "Original".
        /// </summary>
        [TestMethod]
        public void Add_Action_Original()
        {
            var prepareService = new PrepareService();
            var analyzeService = new AnalyzeService();

            // Création du projet
            var project = new Project
            {
                Label = Add_Action_Original_ProjectName,
                ObjectiveCode = "PROD01",
                Workshop = "Test_Add_Action_Original",
            };
            prepareService.SaveProject(project);

            // Création du scénario initial
            var initialScenario = prepareService.CreateInitialScenario(project.ProjectId);
            initialScenario.StateCode = KnownScenarioStates.Validated;
            prepareService.SaveScenario(project.ProjectId, initialScenario);

            // Création du scénario cible 1
            var targetScenario1 = prepareService.CreateScenario(project.ProjectId, initialScenario);
            targetScenario1.Label = "Target 1";
            prepareService.SaveScenario(project.ProjectId, targetScenario1);

            // Création du scénario cible 2 - hérite du scénario cible 1
            var targetScenario2 = prepareService.CreateScenario(project.ProjectId, targetScenario1);
            targetScenario2.Label = "Target 2";
            prepareService.SaveScenario(project.ProjectId, targetScenario2);

            // Passage du scénario initial en non figé
            targetScenario1.StateCode = KnownScenarioStates.Draft;
            prepareService.SaveScenario(project.ProjectId, targetScenario1);

            // Requêtage des données rafraichies
            var scenarios = analyzeService.GetAcquireData(project.ProjectId);
            initialScenario = scenarios.Scenarios[0];
            targetScenario1 = scenarios.Scenarios[1];
            targetScenario2 = scenarios.Scenarios[2];

            // Ajout de la tâche T1 dans le scénario Initial
            var t1Initial = new KAction
            {
                WBS = "1",
                Start = 0,
                Finish = 1,
                BuildStart = 0,
                BuildFinish = 1,
            };


            initialScenario.Actions.Add(t1Initial);
            analyzeService.SaveAcquireData(new Scenario[] { initialScenario, targetScenario1, targetScenario2 }, initialScenario);

            // Requêtage des données rafraichies
            scenarios = analyzeService.GetAcquireData(project.ProjectId);
            initialScenario = scenarios.Scenarios[0];
            targetScenario1 = scenarios.Scenarios[1];
            targetScenario2 = scenarios.Scenarios[2];

            // Vérification de la correspondance entre les actions/originaux
            Assert.AreEqual(initialScenario.Actions[0], targetScenario1.Actions[0].Original);
            Assert.AreEqual(targetScenario1.Actions[0], targetScenario2.Actions[0].Original);

        }

        [TestCleanup]
        public void Cleanup()
        {
            // Suppression des projets créés.

            var prepapreService = new PrepareService();

            var projects = prepapreService.GetProjects().Projects.Where(p => p.Label == Add_Action_Original_ProjectName);

            foreach (var project in projects)
            {
                project.MarkAsDeleted();
                prepapreService.SaveProject(project);
            }

        }
    }
}
