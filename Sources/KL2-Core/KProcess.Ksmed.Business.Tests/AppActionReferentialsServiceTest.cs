using System;
using System.Collections.Generic;
using System.Linq;
using KProcess.Ksmed.Business.Impl;
using KProcess.Ksmed.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KProcess.Ksmed.Business.Tests;
using KProcess.Ksmed.Business.Tests.Helper;
using KProcess.Ksmed.Business.Dtos.Prepare;
using KProcess.Ksmed.Business.Dtos;

namespace Business.Tests
{
    [TestClass()]
    public class AppActionReferentialsServiceTest
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

        /// <summary>
        ///A test for GetResources
        ///</summary>
        [TestMethod()]
        public void MergeCategoriesTest()
        {
            SampleData.ClearDatabaseThenImportDefaultProject();

            var service = new ReferentialsService();

            ActionCategory[] categories;
            using (var ctx = KProcess.Ksmed.Data.ContextFactory.GetNewContext())
            {
                categories = ctx.ActionCategories.ToArray();
            }

            service.MergeReferentials(new ActionCategoryProject { ActionCategoryId = categories[0].ActionCategoryId }, new ActionCategory[]
            {
                categories[1],
                categories[2],
                categories[3],
            });

        }

        /// <summary>
        ///A test for GetResources
        ///</summary>
        [TestMethod()]
        public void MergeMultipleReferentialsTest()
        {
            SampleData.ClearDatabase();
            // Importer le projet une deuxième fois
            SampleData.ImportProject(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("KProcess.Ksmed.Business.Tests.Resources.MergeMultipleReferentials.ksp"));
            SampleData.ImportProject(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("KProcess.Ksmed.Business.Tests.Resources.MergeMultipleReferentials.ksp"));

            // Mémoriser les Ref1 de chaque tâche de chaque projet

            var projects = GetAllProjectDetails().ToArray();
            var actionsRef1 = projects.SelectMany(p => p.Scenarios.SelectMany(s => s.Actions.SelectMany(a => a.Ref1))).ToDictionary(al => al.ActionId, al => al.Referential.Label);

            // Créer un référentiel standard pour chaque ref1
            var masterSlaves = new Dictionary<Ref1Standard, Ref1Project[]>();

            foreach (var ref1 in projects[0].Project.Ref1)
            {
                var standard = new Ref1Standard
                {
                    Label = ref1.Label,
                };

                masterSlaves[standard] = new Ref1Project[]
                {
                    ref1,
                    projects[1].Project.Ref1.First(r => r.Label == ref1.Label),
                };

                new ReferentialsService().SaveReferentials<Ref1>(new Ref1Standard[]
                {
                   standard,
                });
            }

            // Merger chaque ref standard avec les deux ref projets
            var service = new ReferentialsService();
            foreach (var standard in masterSlaves.Keys)
            {
                // Copier pour éviter les références inutiles
                var std = new Ref1Standard
                {
                    RefId = standard.RefId,
                };
                standard.MarkAsUnchanged();

                var slaves = masterSlaves[standard].Select(s => new Ref1Project { RefId = s.RefId }).ToArray();
                foreach (var slave in slaves)
                    slave.MarkAsUnchanged();

                service.MergeReferentials(std, slaves);
            }

            // Vérifier que les référentiels des tâches correspondent
            projects = GetAllProjectDetails().ToArray();
            var actionsRef1Final = projects.SelectMany(p => p.Scenarios.SelectMany(s => s.Actions.SelectMany(a => a.Ref1))).ToDictionary(al => al.ActionId, al => al.Referential.Label);

            CollectionAssert.AreEquivalent(actionsRef1, actionsRef1Final);
        }

        private IEnumerable<RestitutionData> GetAllProjectDetails()
        {
            var projects = new PrepareService().GetProjects();

            var analyzeService = new AnalyzeService();

            foreach (var project in projects.Projects)
            {
                yield return analyzeService.GetFullProjectDetails(project.ProjectId);
            }

        }
    }
}
