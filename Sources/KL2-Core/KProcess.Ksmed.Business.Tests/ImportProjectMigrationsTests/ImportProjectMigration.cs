using KProcess.KL2.APIClient;
using KProcess.Ksmed.Business.Dtos.Prepare;
using KProcess.Ksmed.Business.Impl;
using KProcess.Ksmed.Business.Impl.ProjectImportMigration;
using KProcess.Ksmed.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Business.Tests.ImportProjectMigrationsTests
{
    [TestClass]
    public class ImportProjectMigration
    {
        public TestContext TestContext { get; set; }

        // Scénario :
        // Le logiciel a 3 versions :
        // v2200 : état initial, valeur de l'export projet
        // v2500 : rien de nouveau
        // v2600 : Migration d'une valeur
        // v2700 : rien de nouveau. Version actuelle

        private class CustomMigration : IExportedProjectMigration
        {
            #region IExportedProjectMigration Members

            public Version NewVersion
            {
                get { return new Version(2, 6, 0, 0); }
            }

            public System.IO.Stream Migrate(System.IO.Stream stream, Version currentVersion)
            {
                // Désérialiser
                var po = SerializationOperations.DeserializeWithNamespacesChange<KProcess.Ksmed.Business.Dtos.Export.OlderVersions.v2200.ProjectExport>(stream, currentVersion, false);

                // Modifier la valeur
                po.Project.Label += " migré";

                // Resérialiser
                var newStream = SerializationOperations.Serialize(po);

                return newStream;
            }

            #endregion
        }

        private static IExportedProjectMigration[] _initialMigrations;

        [ClassInitialize]
        public static void Initialize(TestContext tc)
        {
            _initialMigrations = ProjectMigration._migrations;
        }

        [TestCleanup]
        public void Cleanup()
        {
            SerializationOperations.SpecificNamespacesSerializationBinder.AssemblyTypeResolverOverride = null;
            ProjectMigration._migrations = _initialMigrations;
        }

        [TestMethod]
        public void ImportProjectMigrationTest()
        {
            var importData = PrepareImport();
            var migration = new ProjectMigration(importData);
            var pe = migration.Migrate();

            Assert.IsTrue(pe.Project.Label.EndsWith(" migré"));
        }

        [TestMethod]
        public void ImportProjectServiceWithMigrationTest()
        {
            SampleData.ClearDatabaseThenImportDefaultProject();

            var importData = PrepareImport();

            var service = new ImportExportService();

            var mre = new System.Threading.ManualResetEvent(false);

            KProcess.Ksmed.Business.Dtos.Export.ProjectImport projectImport = null;
            Exception e = null;

            int newProjectId = -1;

            service.PredictMergedReferentialsProject(importData, pi =>
            {
                projectImport = pi;

                service.ImportProject(pi, false, TestContext.DeploymentDirectory, p =>
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
            Assert.IsNotNull(projectImport);

            Assert.IsTrue(projectImport.ExportedProject.Project.Label.EndsWith(" migré"));

            // Récupérer le projet depuis la base et tester
            var prepareService = new PrepareService();

            mre.Reset();
            ProjectsData data = null;
            prepareService.GetProjects(d =>
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

            var project = data.Projects.First(p => p.ProjectId == newProjectId);

            Assert.IsTrue(project.Label.EndsWith(" migré"));


        }

        private byte[] PrepareImport()
        {
            byte[] importData;
            using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("KProcess.Ksmed.Business.Tests.ImportProjectMigrationsTests.Projet2200.ksp"))
            {
                using (var memoryStream = new MemoryStream()) { stream.CopyTo(memoryStream, StreamExtensions.BufferSize); importData = memoryStream.ToArray(); }
            }

            SerializationOperations.SpecificNamespacesSerializationBinder.AssemblyTypeResolverOverride = typeName =>
            {
                if (typeName.Contains("OlderVersions"))
                    return typeof(ImportProjectMigration).Assembly.GetType(typeName);
                else if (typeName.Contains("Models"))
                    return typeof(KAction).Assembly.GetType(typeName);
                else if (typeName.Contains("Dtos"))
                    return typeof(KProcess.Ksmed.Business.Dtos.Export.ProjectExport).Assembly.GetType(typeName);
                else
                    return null;
            };

            ProjectMigration._migrations = new IExportedProjectMigration[] { new CustomMigration() };

            return importData;
        }


    }
}
