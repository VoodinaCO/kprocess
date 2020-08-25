using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Business.Tests;
using Kent.Boogaart.KBCsv;
using KProcess.Ksmed.Business.Dtos;
using KProcess.Ksmed.Business.Impl;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Core.ExcelExport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KProcess.Ksmed.Business.Tests;

namespace KProcess.Ksmed.Presentation.Tests
{


    /// <summary>
    ///This is a test class for ExcelBuilderTest and is intended
    ///to contain all ExcelBuilderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ExcelBuilderTest
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

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            ServicesHelper.RegisterMockServices();
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

        [TestMethod()]
        public void CellReferenceTest()
        {
            Assert.AreEqual("A1", new CellReference().Reference);
            Assert.AreEqual("A1", new CellReference(1, 1).Reference);
            Assert.AreEqual("Z10", new CellReference(26, 10).Reference);
            Assert.AreEqual("AA10", new CellReference(27, 10).Reference);
            Assert.AreEqual("AB10", new CellReference(28, 10).Reference);
            Assert.AreEqual("ZZ10", new CellReference(702, 10).Reference);
            Assert.AreEqual("AAA10", new CellReference(703, 10).Reference);
            Assert.AreEqual("AAB10", new CellReference(704, 10).Reference);

            Assert.AreEqual(new CellReference().Reference, new CellReference("A1").Reference);

            Assert.AreEqual((uint)26, new CellReference("Z10").ColumnIndex);
            Assert.AreEqual((uint)10, new CellReference("Z10").RowIndex);

            Assert.AreEqual((uint)28, new CellReference("AB10").ColumnIndex);
            Assert.AreEqual((uint)10, new CellReference("AB10").RowIndex);

            Assert.AreEqual((uint)702, new CellReference("ZZ10").ColumnIndex);
            Assert.AreEqual((uint)10, new CellReference("ZZ10").RowIndex);

            Assert.AreEqual((uint)703, new CellReference("AAA10").ColumnIndex);
            Assert.AreEqual((uint)10, new CellReference("AAA10").RowIndex);

            Assert.AreEqual((uint)704, new CellReference("AAB10").ColumnIndex);
            Assert.AreEqual((uint)10, new CellReference("AAB10").RowIndex);
        }


        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@"Resources\Excel base.xlsm", "Resources")]
        public void CreateTest()
        {
            string outFile = Path.Combine(TestContext.DeploymentDirectory, ExcelExporter.GetFileNameWithExtension("test"));

            CellContent[][] data = new CellContent[1][];

            ExcelExporter file = null;
            try
            {
                file = ExcelExporter.Create(outFile);
            }
            catch(ExcelExporter.FileAlreadyInUseExeption)
            {
                return; // Une notification a déjà été levée dans ce cas
            }

            var sheet = file.CreateSheet("Test");
            var cellRef = new CellReference();

            // String

            file.SetCellValue(sheet, cellRef, "string");
            cellRef.MoveRight();

            file.SetCellValue(sheet, cellRef, "");
            cellRef.MoveRight();

            file.SetCellValue(sheet, cellRef, new CellContent(null, CellDataType.String));
            cellRef.MoveRight();

            cellRef.NewLine();

            // Number

            file.SetCellValue(sheet, cellRef, 4);
            cellRef.MoveRight();

            file.SetCellValue(sheet, cellRef, 1.02);
            cellRef.MoveRight();

            file.SetCellValue(sheet, cellRef, 1.54E-12);
            cellRef.MoveRight();

            file.SetCellValue(sheet, cellRef, -12544.561681);
            cellRef.MoveRight();

            cellRef.NewLine();

            // Percentage

            file.SetCellValue(sheet, cellRef, CellContent.Percentage(.1));
            cellRef.MoveRight();

            file.SetCellValue(sheet, cellRef, CellContent.Percentage(.4656848468));
            cellRef.MoveRight();

            file.SetCellValue(sheet, cellRef, CellContent.Percentage(3));
            cellRef.MoveRight();

            file.SetCellValue(sheet, cellRef, CellContent.Percentage(-2));
            cellRef.MoveRight();

            cellRef.NewLine();

            // Timespan

            file.SetCellValue(sheet, cellRef, TimeSpan.FromMinutes(34.652155648));
            cellRef.MoveRight();

            file.SetCellValue(sheet, cellRef, TimeSpan.FromDays(42.656));
            cellRef.MoveRight();

            file.SetCellValue(sheet, cellRef, TimeSpan.FromHours(-1.6548));
            cellRef.MoveRight();

            file.SetCellValue(sheet, cellRef, CellContent.TimeSpan(TimeSpan.FromHours(5.6586).Ticks));
            cellRef.MoveRight();

            cellRef.NewLine();

            // Date

            file.SetCellValue(sheet, cellRef, new DateTime(1970, 1, 1));
            cellRef.MoveRight();

            file.SetCellValue(sheet, cellRef, new DateTime(2013, 4, 5, 6, 52, 34, 658));
            cellRef.MoveRight();

            file.SetCellValue(sheet, cellRef, new DateTime(1900, 10, 10));
            cellRef.MoveRight();

            cellRef.NewLine();

            // Hyperlink

            file.SetCellValue(sheet, cellRef, new CellContent("hyperlink", CellDataType.Hyperlink));
            cellRef.MoveRight();

            file.SetCellValue(sheet, cellRef, new CellContent("", CellDataType.Hyperlink));
            cellRef.MoveRight();

            file.SetCellValue(sheet, cellRef, new CellContent(null, CellDataType.Hyperlink));
            cellRef.MoveRight();

            cellRef.NewLine();

            file.SaveAndClose();

            Assert.IsTrue(File.Exists(outFile));
            //System.Diagnostics.Process.Start(outFile);

            TestContext.AddResultFile(outFile);

        }

        [TestMethod()]
        public void ExportProjectExcelTest()
        {
            SampleData.ClearDatabaseThenImportDefaultProject();
            string outFile = Path.Combine(TestContext.DeploymentDirectory, ExcelExporter.GetFileNameWithExtension("project export"));

            var service = new AnalyzeService();

            var mre = new System.Threading.ManualResetEvent(false);

            RestitutionData data = null;
            Exception e = null;
            service.GetFullProjectDetails(SampleData.GetProjectId(), d =>
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
            Assert.IsNotNull(data);

            new KProcess.Ksmed.Presentation.ViewModels.Restitution.ExportProjectToExcel(data, new ExportResult
            {
                Accepts = true,
                OpenWhenCreated = false,
                Filename = outFile,
            }).Export();

            TestContext.AddResultFile(outFile);
        }

    }
}
