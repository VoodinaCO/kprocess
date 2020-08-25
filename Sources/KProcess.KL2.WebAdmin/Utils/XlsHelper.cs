using System;

namespace KProcess.KL2.WebAdmin.Utils
{
    public static class XlsHelper
    {
        public static double PixelsToHeightPoints(this double pixels) =>
            pixels * 3.0 / 4.0;

        public static double PixelsToHeightPoints(this int pixels) =>
            Convert.ToDouble(pixels).PixelsToHeightPoints();

        public static double PixelsToWidthPoints(this double pixels) =>
            pixels / 7.0;

        public static double PixelsToWidthPoints(this int pixels) =>
            Convert.ToDouble(pixels).PixelsToWidthPoints();

        public static double HeightPointsToPixelsDouble(this double points) =>
            points * 4.0 / 3.0;

        public static int HeightPointsToPixelsInt32(this double points) =>
            Convert.ToInt32(points.HeightPointsToPixelsDouble());

        public static double WidthPointsToPixelsDouble(this double points) =>
            points * 7.0;

        public static int WidthPointsToPixelsInt32(this double points) =>
            Convert.ToInt32(points.WidthPointsToPixelsDouble());
    }
}