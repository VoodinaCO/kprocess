using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Business.Tests;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Business.Impl;
using KProcess.Ksmed.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KProcess.Ksmed.Business.Tests
{
    [TestClass()]
    public class AnalyzeServiceTest
    {

        [TestMethod()]
        public void ConvertActionToReducedActionTest()
        {
            SampleData.ClearDatabaseThenImportDefaultProject();
            AnalyzeService service = new AnalyzeService();

            var mre = new System.Threading.ManualResetEvent(false);
            BuildData data = null;
            Exception e = null;
            service.GetBuildData(SampleData.GetProjectId(), d =>
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
        public void GetActionsDurationsTest()
        {
            SampleData.ClearDatabaseThenImportDefaultProject();
            using (var ctx = KProcess.Ksmed.Data.ContextFactory.GetNewContext())
            {
                var lastScenario = ctx.Scenarios
                    .Include("Actions")
                    .OrderByDescending(s => s.ScenarioId)
                    .First();

                var res = SharedScenarioActionsOperations.GetActionsBuildDurations(
                    KProcess.Ksmed.Data.ContextFactory.GetNewContext(),
                    new int[] { lastScenario.Actions[0].ActionId, lastScenario.Actions[1].ActionId });

                Assert.IsTrue(res.Any());
            }

        }
    }
}
