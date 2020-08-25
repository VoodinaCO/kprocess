using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Business.Tests;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Business.Impl;
using KProcess.Ksmed.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KProcess.Ksmed.Business.Referentials;
using KProcess.Ksmed.Tests.Helper;
using KProcess.Ksmed.Business.Impl.ImportExport;

namespace KProcess.Ksmed.Business.Tests
{
    [TestClass()]
    public class ExportServiceVideoDecompositionTest
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

        static int fileNumber = 0;

        [TestMethod()]
        public void ExportImportVideoDecompositionMergeTest()
        {
            SampleData.ClearDatabaseThenImportDefaultProject();

            using (var ctx = KProcess.Ksmed.Data.ContextFactory.GetNewContext())
            {
                var projectId = ctx.Projects.First().ProjectId;
                ExportVideoDecomposition(
                    projectId,
                    ctx.Scenarios.Where(sc => sc.ProjectId == projectId)
                        .OrderByDescending(sc => sc.ScenarioId)
                        .First()
                        .ScenarioId,
                    ctx.Videos.First().VideoId,
                    projectId,
                    true);
            }
        }

        [TestMethod()]
        public void ExportImportVideoDecompositionNoMergeTest()
        {
            SampleData.ClearDatabaseThenImportDefaultProject();

            using (var ctx = KProcess.Ksmed.Data.ContextFactory.GetNewContext())
            {
                var projectId = ctx.Projects.First().ProjectId;
                ExportVideoDecomposition(
                    projectId,
                    ctx.Scenarios.Where(sc => sc.ProjectId == projectId)
                        .OrderByDescending(sc => sc.ScenarioId)
                        .First()
                        .ScenarioId,
                    ctx.Videos.First().VideoId,
                    projectId,
                    false);
            }
        }

        /// <summary>
        /// Exporte le projet spécifié.
        /// </summary>
        private void ExportVideoDecomposition(int projectId, int scenarioId, int videoId, int targetProjectId, bool merge)
        {
            var service = new ImportExportService();

            string fileName = string.Format("out{0}.xml", fileNumber++);

            KProcess.Ksmed.Business.Dtos.Export.VideoDecompositionExport oldVideoExport;
            using (var context = KProcess.Ksmed.Data.ContextFactory.GetNewContext())
            {
                oldVideoExport = new VideoDecompositionExporter(context, projectId, scenarioId, videoId).CreateExport();
            }

            var mre = new System.Threading.ManualResetEvent(false);
            Stream stream = null;
            Exception e = null;
            service.ExportVideoDecomposition(projectId, scenarioId, videoId, d =>
            {
                stream = d;
                mre.Set();
            }, ex =>
            {
                e = ex;
                mre.Set();
            });

            mre.WaitOne();
            AssertExt.IsExceptionNull(e);
            Assert.IsNotNull(stream);

            using (var reader = new StreamReader(stream))
            {
                string xml = reader.ReadToEnd();
                File.WriteAllText(fileName, xml);

                Assert.IsNotNull(xml);
            }
            stream.Close();

            Initialization.SetCurrentUser("paula");

            // Ouvrir le fichier
            mre.Reset();
            service.PredictMergedReferentialsVideoDecomposition(targetProjectId, File.OpenRead(fileName), vdi =>
            {
                service.ImportVideoDecomposition(vdi, merge, TestContext.DeploymentDirectory, targetProjectId, success =>
                {
                    Assert.IsTrue(success);
                    mre.Set();
                }, ex =>
                {
                    e = ex;
                    mre.Set();
                });
            }, ex =>
            {
                e = ex;
                mre.Set();
            });

            mre.WaitOne();
            AssertExt.IsExceptionNull(e);

            // Récupérer le numéro de scénario initial du projet
            Scenario targetScenario;
            int newVideoId;
            using (var context = KProcess.Ksmed.Data.ContextFactory.GetNewContext())
            {
                targetScenario = context.Scenarios.Single(s => s.ProjectId == targetProjectId && s.StateCode == KnownScenarioStates.Draft && s.NatureCode == KnownScenarioNatures.Initial);
                var oldVideoName = context.Videos.Single(v => v.VideoId == videoId).Name;
                newVideoId = context.Videos.Where(v => v.ProjectId == targetProjectId && v.Name == oldVideoName)
                    .AsEnumerable().Last().VideoId;
            }

            //Réexporter la décompo
            KProcess.Ksmed.Business.Dtos.Export.VideoDecompositionExport newVideoExport;
            using (var context = KProcess.Ksmed.Data.ContextFactory.GetNewContext())
            {
                newVideoExport = new VideoDecompositionExporter(context, targetScenario.ProjectId, targetScenario.ScenarioId, newVideoId).CreateExport();
            }

            // Comparer les valeurs

            // Vidéo
            var oldVideo = oldVideoExport.Video;
            var newVideo = newVideoExport.Video;
            AssertVideo(oldVideo, newVideo);

            // Référentiels
            var p1RerentialsProject = ReferentialsHelper.GetAllReferentialsProject(oldVideoExport.Actions).ToArray();
            var p1RerentialsStandard = ReferentialsHelper.GetAllReferentialsStandardUsed(oldVideoExport.Actions).ToArray();

            var p2RerentialsProject = ReferentialsHelper.GetAllReferentialsProject(newVideoExport.Actions).ToArray();
            var p2RerentialsStandard = ReferentialsHelper.GetAllReferentialsStandardUsed(newVideoExport.Actions).ToArray();

            Assert.IsTrue(p1RerentialsProject.Length <= p2RerentialsProject.Length);
            Assert.AreEqual(p1RerentialsProject.Length + p1RerentialsStandard.Length, p2RerentialsProject.Length);
            Assert.AreEqual(0, p2RerentialsStandard.Length);

            // Vérifier que toutes les actions de l'ancien export soient également maintenant dans le projet de destination
            var oldActions = oldVideoExport.Actions.OrderBy(a => WBSHelper.GetParts(a.WBS), new WBSHelper.WBSComparer()).ToArray();
            var newActions = newVideoExport.Actions.OrderBy(a => WBSHelper.GetParts(a.WBS), new WBSHelper.WBSComparer()).ToArray();

            Assert.AreEqual(oldActions.Length, newActions.Length);

            // Actions
            for (int j = 0; j < oldActions.Length; j++)
            {
                var oldAction = oldActions[j];
                var newAction = newActions[j];

                AssertAction(oldAction, newAction);

                // Actions réduites
                AssertActionReduced(oldAction.Reduced, newAction.Reduced);
            }

            ServicesDiagnosticsDebug.CheckReferentialsState();

        }

        private void AssertVideo(Video v1, Video v2)
        {
            Assert.IsTrue(new EntityComparer<Video>(TestContext)
                .MustBeDifferent(
                    "VideoId",
                    "CreatedByUserId", "CreationDate", "ModifiedByUserId", "LastModificationDate")
                .Ignore(
                    "FilePath", "Project", "ProjectId", "DefaultResource", "DefaultResourceId")
                .Compare(v1, v2));

            Assert.IsTrue(v2.FilePath.StartsWith(TestContext.DeploymentDirectory));

            Assert.AreEqual(Path.GetFileName(v1.FilePath), Path.GetFileName(v2.FilePath));
        }

        private void AssertAction(KAction a1, KAction a2)
        {
            Assert.IsTrue(new EntityComparer<KAction>(TestContext)
                .MustBeDifferent(
                    "ActionId",
                    "ScenarioId",
                    "WBS",
                    "CreatedByUserId", "CreationDate", "ModifiedByUserId", "LastModificationDate")
                .MustBeDifferentOrNull(
                    "OriginalActionId",
                    "VideoId", "Video",
                    "Reduced"
                    )
                .Ignore(
                    "Start", "Finish",
                    "BuildStart", "BuildFinish",
                    "Scenario",
                    "Category", "CategoryId",
                    "Resource", "ResourceId")
                .Compare(a1, a2));

            // Si l'action est un groupe, ses timings Start et Finish ne sont pas à tester
        }

        private void AssertResource(Resource r1, Resource r2)
        {
            AssertReferential(r1, r2, "ResourceId");
        }

        private void AssertCategory(ActionCategory cat1, ActionCategory cat2)
        {
            AssertReferential(cat1, cat2, "ActionCategoryId");
        }

        private void AssertMultiReferentials(IEnumerable<IReferentialActionLink> refes1, IEnumerable<IReferentialActionLink> refes2, string idPropertyName)
        {
            var refes1List = refes1.ToArray();
            var refes2List = refes2.ToArray();
            Assert.AreEqual(refes1List.Length, refes2List.Length, "Pas le même nombre de référentiels");

            for (int i = 0; i < refes1List.Length; i++)
            {
                var r1 = refes1List[i];
                var r2 = refes2List[i];

                Assert.AreEqual(r1.Quantity, r2.Quantity);
                AssertReferential(r1.Referential, r2.Referential, idPropertyName);
            }
        }

        private void AssertReferential(IActionReferential refe1, IActionReferential refe2, string idPropertyName)
        {
            Assert.IsFalse(refe1 == null && refe2 != null ||
                refe1 != null && refe2 == null);

            if (refe1 == null && refe2 == null)
                return;

            Assert.IsTrue(refe2 is IActionReferentialProcess);

            Assert.IsTrue(new EntityComparer<IActionReferential>(TestContext)
                .MustBeDifferent(
                    "CreatedByUserId", "CreationDate", "ModifiedByUserId", "LastModificationDate")
                .Ignore(
                    "ProjectId", "Project", idPropertyName
                )
                .Compare(refe1, refe2));
        }

        private void AssertActionReduced(KActionReduced a1, KActionReduced a2)
        {
            Assert.IsFalse(a1 == null && a2 != null ||
                a1 != null && a2 == null);

            if (a1 == null && a2 == null)
                return;

            Assert.IsTrue(new EntityComparer<KActionReduced>(TestContext)
                .MustBeDifferent(
                    "ActionId", "Action")
                .Compare(a1, a2));
        }

    }
}
