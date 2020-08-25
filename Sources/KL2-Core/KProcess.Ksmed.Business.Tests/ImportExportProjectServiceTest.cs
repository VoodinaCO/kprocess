using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Business.Tests;
using KProcess.Ksmed.Business.ActionsManagement;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Business.Impl;
using KProcess.Ksmed.Data;
using KProcess.Ksmed.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KProcess.Ksmed.Business.Referentials;
using KProcess.Ksmed.Tests.Helper;
using KProcess.KL2.APIClient;

namespace KProcess.Ksmed.Business.Tests
{
    [TestClass()]
    public class ImportExportProjectServiceTest
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

        /// <summary>
        /// test l'export et l'import du projet.
        /// </summary>
        [TestMethod()]
        public void ExportImportProjectMergeServiceTest()
        {
            SampleData.ClearDatabaseThenImportDefaultProject();

            ExportProject(SampleData.GetProjectId(), true);
        }

        /// <summary>
        /// test l'export et l'import du projet.
        /// </summary>
        [TestMethod()]
        public void ExportImportProjectNoMergeServiceTest()
        {
            SampleData.ClearDatabaseThenImportDefaultProject();

            ExportProject(SampleData.GetProjectId(), false);
        }

        /// <summary>
        /// Exporte le projet spécifié.
        /// </summary>
        /// <param name="projectId">L'identifiant du projet.</param>
        private void ExportProject(int projectId, bool merge)
        {
            var service = new ImportExportService();

            string fileName = string.Format("out{0}.xml", fileNumber++);

            KProcess.Ksmed.Business.Dtos.Export.ProjectExport oldProjectExport;
            using (var context = KProcess.Ksmed.Data.ContextFactory.GetNewContext())
            {
                oldProjectExport = service.CreateProjectExport(context, projectId);
            }

            var mre = new System.Threading.ManualResetEvent(false);
            Stream stream = null;
            Exception e = null;
            service.ExportProject(projectId, d =>
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
            int newProjectId = -1;

            service.PredictMergedReferentialsProject(File.ReadAllBytes(fileName), pi =>
            {
                service.ImportProject(pi, merge, TestContext.DeploymentDirectory, p =>
                {
                    newProjectId = p.ProjectId;
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

            // Réexporter le projet
            KProcess.Ksmed.Business.Dtos.Export.ProjectExport newProjectExport;
            using (var context = KProcess.Ksmed.Data.ContextFactory.GetNewContext())
            {
                newProjectExport = service.CreateProjectExport(context, newProjectId);
            }

            // Comparer les valeurs

            // Projet
            AssertProject(oldProjectExport.Project, newProjectExport.Project);

            // Vidéos
            var oldVideos = oldProjectExport.Project.Videos.OrderBy(v => v.Name).ToArray();
            var newVideos = newProjectExport.Project.Videos.OrderBy(v => v.Name).ToArray();
            for (int i = 0; i < oldVideos.Length; i++)
                AssertVideo(oldVideos[i], newVideos[i]);

            // Scénarios
            var oldScenarios = oldProjectExport.Project.Scenarios.OrderBy(s => s.Label).ToArray();
            var newScenarios = newProjectExport.Project.Scenarios.OrderBy(s => s.Label).ToArray();

            for (int i = 0; i < oldProjectExport.Project.Scenarios.Count; i++)
            {
                var oldScenario = oldScenarios[i];
                var newScenario = newScenarios[i];

                AssertScenario(oldScenario, newScenario);

                var oldActions = oldScenario.Actions.OrderBy(a => WBSHelper.GetParts(a.WBS), new WBSHelper.WBSComparer()).ToArray();
                var newActions = newScenario.Actions.OrderBy(a => WBSHelper.GetParts(a.WBS), new WBSHelper.WBSComparer()).ToArray();

                // Actions
                for (int j = 0; j < oldScenario.Actions.Count; j++)
                {
                    var oldAction = oldActions[j];
                    var newAction = newActions[j];

                    AssertAction(oldAction, newAction);

                    // Actions réduites
                    AssertActionReduced(oldAction.Reduced, newAction.Reduced);
                }

            }
        }

        private void AssertProject(Project p1, Project p2)
        {
            Assert.IsTrue(new EntityComparer<Project>(TestContext)
                .MustBeDifferent(
                    "ProjectId",
                    "CreationDate", "LastModificationDate")
                .Ignore(
                    "CreatedByUserId", "ModifiedByUserId")
                .Compare(p1, p2));

            var p1RerentialsProject = ReferentialsHelper.GetAllReferentialsProject(p1).ToArray();
            var p1RerentialsStandard = ReferentialsHelper.GetAllReferentialsStandardUsed(p1).ToArray();

            var p2RerentialsProject = ReferentialsHelper.GetAllReferentialsProject(p2).ToArray();
            var p2RerentialsStandard = ReferentialsHelper.GetAllReferentialsStandardUsed(p2).ToArray();

            Assert.IsTrue(p1RerentialsProject.Length <= p2RerentialsProject.Length);
            Assert.AreEqual(p1RerentialsProject.Length + p1RerentialsStandard.Length, p2RerentialsProject.Length);
            Assert.AreEqual(0, p2RerentialsStandard.Length);
        }

        private void AssertVideo(Video v1, Video v2)
        {
            Assert.IsTrue(new EntityComparer<Video>(TestContext)
                .MustBeDifferent(
                    "VideoId",
                    "ProjectId", "Project",
                    "CreationDate", "LastModificationDate")
                .MustBeDifferentOrNull(
                    "DefaultResourceId", "DefaultResource")
                .Ignore(
                    "FilePath")
                .Ignore(
                    "CreatedByUserId", "ModifiedByUserId")
                .Compare(v1, v2));

            Assert.IsTrue(v2.FilePath.StartsWith(TestContext.DeploymentDirectory));

            Assert.AreEqual(Path.GetFileName(v1.FilePath), Path.GetFileName(v2.FilePath));
        }

        private void AssertScenario(Scenario s1, Scenario s2)
        {
            Assert.IsTrue(new EntityComparer<Scenario>(TestContext)
                .MustBeDifferent(
                    "ScenarioId",
                    "ProjectId", "Project",
                    "CreationDate", "LastModificationDate")
                .MustBeDifferentOrNull(
                    "OriginalScenarioId")
                .Ignore(
                    "CreatedByUserId", "ModifiedByUserId")
                .Compare(s1, s2));
        }

        private void AssertAction(KAction aa, KAction ab)
        {
            Assert.IsTrue(new EntityComparer<KAction>(TestContext)
                .MustBeDifferent(
                    "ActionId",
                    "ScenarioId", "Scenario",
                    "ResourceId", "Resource",
                    "CreationDate", "LastModificationDate")
                .MustBeDifferentOrNull(
                    "OriginalActionId",
                    "VideoId", "Video",
                    "CategoryId", "Category",
                    "Reduced"
                    )
                .Collection("Thumbnail", CollectionComparison.Content)
                .Ignore(
                    "CreatedByUserId", "ModifiedByUserId")
                .Compare(aa, ab));

            // Vérifier les référentiels
            AssertCategory(aa.Category, ab.Category);
            AssertResource(aa.Resource, ab.Resource);
            AssertMultiReferentials(aa.Ref1, ab.Ref1, "RefId");
            AssertMultiReferentials(aa.Ref2, ab.Ref2, "RefId");
            AssertMultiReferentials(aa.Ref3, ab.Ref3, "RefId");
            AssertMultiReferentials(aa.Ref4, ab.Ref4, "RefId");
            AssertMultiReferentials(aa.Ref5, ab.Ref5, "RefId");
            AssertMultiReferentials(aa.Ref6, ab.Ref6, "RefId");
            AssertMultiReferentials(aa.Ref7, ab.Ref7, "RefId");
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
                    idPropertyName,
                    "CreationDate", "LastModificationDate")
                .Ignore(
                    "ProjectId", "Project"
                )
                .Ignore(
                    "CreatedByUserId", "ModifiedByUserId")
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

        /// <summary>
        /// Teste l'export d'un projet contenant une vidéo POV dont la ressource par défaut est une ressource standard qui n'est pas utilisée par une action.
        /// </summary>
        [TestMethod]
        public void ExportVideoDefaultResource_Bug1547Test()
        {
            var resourceStandard = new EquipmentStandard() { Label = "Res" };
            var project = new Project();
            var video = new Video()
            {
                Name = "Video",
                DefaultResource = resourceStandard,
            };

            project.Videos.Add(video);
            Assert.IsTrue(ReferentialsHelper.GetAllReferentialsStandardUsed(project).Contains(resourceStandard));
        }

        [TestMethod]
        public void PredictMergedReferentials_Tests()
        {
            SampleData.ClearDatabaseThenImportDefaultProject();

            // Exporter le projet
            var service = new ImportExportService();

            var mre = new System.Threading.ManualResetEvent(false);
            Stream stream = null;
            Exception e = null;
            service.ExportProject(SampleData.GetProjectId(), d =>
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

            stream.Position = 0;

            byte[] importData;
            using (var memoryStream = new MemoryStream()) { stream.CopyTo(memoryStream, StreamExtensions.BufferSize); importData = memoryStream.ToArray(); }

            // PredictMergedReferentials
            mre.Reset();
            KProcess.Ksmed.Business.Dtos.Export.ProjectImport import = null;
            service.PredictMergedReferentialsProject(importData, pi =>
            {
                import = pi;
                mre.Set();
            }, ex =>
            {
                e = ex;
                mre.Set();
            });

            mre.WaitOne();
            AssertExt.IsExceptionNull(e);
            Assert.IsNotNull(import);

            
        }

    }
}
