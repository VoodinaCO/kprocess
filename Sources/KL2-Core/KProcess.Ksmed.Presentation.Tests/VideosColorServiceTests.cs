using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.Tests
{
    [TestClass]
    public class VideosColorServiceTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void NoCollisionTest()
        {
            var v1 = new Video { ProjectId = 1, VideoId = 1 };
            var v2 = new Video { ProjectId = 1, VideoId = 2 };

            var persistanceService = new ColorsPersistanceService
            {
                VideoColors = new Dictionary<Tuple<int, int>, string>
                {
                    { Tuple.Create(1,1), "#8000B0F0" }
                }
            };
            var colorsService = new VideoColorService(persistanceService);

            var b1 = colorsService.GetColor(v1);
            var b2 = colorsService.GetColor(v2);

            Assert.AreNotEqual(((SolidColorBrush)b1).Color, ((SolidColorBrush)b2).Color);
        }

        private class ColorsPersistanceService : IVideoColorPersistanceService
        {
            #region IVideoColorPersistanceService Members

            public IDictionary<Tuple<int, int>, string> VideoColors
            {
                get;
                set;
            }

            #endregion
        }

    }
}
