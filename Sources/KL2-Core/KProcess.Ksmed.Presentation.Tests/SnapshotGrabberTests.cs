using KProcess.Presentation.Windows.Controls;
using KProcess.Presentation.Windows.Controls.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace KProcess.Ksmed.Presentation.Tests
{
    [TestClass]
    public class SnapshotGrabberTests
    {

        public TestContext TestContext { get; set; }

        /// <summary>
        /// Attention, requiert :
        /// - Lav Filters installé
        /// - WmfDemux.dll enregistré avec regsvr32
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"Resources\Hello_Ksmed_Codriver_Low.wmv")]
        public async Task Grab()
        {
            ConfigureContext();

            var tasks = new List<bool>();
            tasks.Add(await GrabSnapshot(0));
            tasks.Add(await GrabSnapshot(10));
            tasks.Add(await GrabSnapshot(15));
            tasks.Add(await GrabSnapshot(30));
            tasks.Add(await GrabSnapshot(80));
            tasks.Add(await GrabSnapshot(4));

            foreach (var t in tasks)
                Assert.IsTrue(t);

        }

        private async Task<bool> GrabSnapshot(double positionSeconds)
        {
            try
            {
                BitmapSource bitmapSource = null;

                bitmapSource = await SnapshotGrabber.GetSnapshotSynchronizedAsync("Hello_Ksmed_Codriver_Low.wmv", TimeSpan.FromSeconds(positionSeconds).Ticks);
                if (bitmapSource == null)
                    return false;

                var thumbnailPath = Path.Combine(TestContext.TestResultsDirectory, string.Format("thumb-{0}s.jpg", Math.Round(positionSeconds, 2)));
                using (var fs = File.Create(thumbnailPath))
                {
                    var encoder = new JpegBitmapEncoder();
                    encoder.QualityLevel = 90;

                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(fs);
                }
                TestContext.AddResultFile(thumbnailPath);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        private void ConfigureContext()
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            ServicesHelper.RegisterMockServices();

            // Configuration des filtres selon ce modèle :
            //<extension extension=".wmv">
            //  <splitter name="GDCL WMV" sourceType="External" externalCLSID="1932C124-77DA-4151-99AA-234FEA09F463" />
            //  <videoDecoder name="LAV Video Decoder" sourceType="External" externalCLSID="EE30215D-164F-4A92-A4EB-9D4C13390F9F" />
            //  <audioDecoder name="Auto Audio Decoder" sourceType="Auto" />
            //</extension>

            var mockDecoderInfoService = new MockDecoderInfoService
            {
                FiltersConfiguration = new FiltersConfiguration()
            };

            mockDecoderInfoService.FiltersConfiguration[".wmv"] = new ExtensionFiltersSource
            {
                Splitter = new FilterSource()
                {
                    Name = "GDCL WMV",
                    SourceType = FilterSourceTypeEnum.External,
                    ExternalCLSID = "1932C124-77DA-4151-99AA-234FEA09F463",
                },

                VideoDecoder = new FilterSource()
                {
                    Name = "LAV Video Decoder",
                    SourceType = FilterSourceTypeEnum.External,
                    ExternalCLSID = "EE30215D-164F-4A92-A4EB-9D4C13390F9F",
                },

                AudioDecoder = new FilterSource()
                {
                    Name = "Auto Audio Decoder",
                    SourceType = FilterSourceTypeEnum.Auto,
                },
            };

            IoC.Resolve<IServiceBus>().Register<IDecoderInfoService>(mockDecoderInfoService);


        }

        private class MockDecoderInfoService : IDecoderInfoService
        {
            #region IDecoderInfoService Members

            public FiltersConfiguration FiltersConfiguration
            {
                get;
                set;
            }

            public bool InitializeFiltersConfiguration()
            {
                return true;
            }

            #endregion
        }

    }
}
