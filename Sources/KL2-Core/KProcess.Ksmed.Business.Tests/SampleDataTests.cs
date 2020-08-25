using Business.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Business.Tests
{
    [TestClass]
    public class SampleDataTests
    {

        [TestMethod]
        public void ClearDatabase()
        {
            SampleData.ClearDatabase();
        }

        [TestMethod]
        public void ClearDatabaseThenImportDefaultProject()
        {
            SampleData.ClearDatabaseThenImportDefaultProject();

            using (var context = KProcess.Ksmed.Data.ContextFactory.GetNewContext())
            {
                Assert.AreEqual(1, context.Projects.Count());
            }
        }
    }
}
