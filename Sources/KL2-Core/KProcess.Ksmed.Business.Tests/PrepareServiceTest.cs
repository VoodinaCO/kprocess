using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Business.Tests;
using KProcess.Ksmed.Business.Dtos.Prepare;
using KProcess.Ksmed.Business.Impl;
using KProcess.Ksmed.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KProcess.Ksmed.Business.Tests
{
    [TestClass()]
    public class PrepareServiceTest
    {

        [TestMethod()]
        public void GetProjectsTest()
        {
            SampleData.ClearDatabaseThenImportDefaultProject();
            var service = new PrepareService();

            var mre = new System.Threading.ManualResetEvent(false);
            ProjectsData data = null;
            Exception e = null;
            service.GetProjects(d =>
            {
                data = d;
                mre.Set();
            }, ex =>
            {
                e = ex;
                mre.Set();
            });

            mre.WaitOne();

            // Pour l'instant vérifie qu'aucune exception n'a été levée
            AssertExt.IsExceptionNull(e);
        }

        [TestMethod()]
        public void GetScenariosTest()
        {
            SampleData.ClearDatabaseThenImportDefaultProject();

            using (var ctx = KProcess.Ksmed.Data.ContextFactory.GetNewContext())
            {
                foreach (var scenario in ctx.Scenarios)
                {
                    GetScenarios(scenario.ScenarioId);
                }
            }
        }

        private void GetScenarios(int scenarioId)
        {
            var service = new PrepareService();

            var mre = new System.Threading.ManualResetEvent(false);
            ScenariosData data = null;
            Exception e = null;
            service.GetScenarios(scenarioId, d =>
            {
                data = d;
                mre.Set();
            }, ex =>
            {
                e = ex;
                mre.Set();
            });

            mre.WaitOne();

            // Pour l'instant vérifie qu'aucune exception n'a été levée
            AssertExt.IsExceptionNull(e);
        }
    }
}
