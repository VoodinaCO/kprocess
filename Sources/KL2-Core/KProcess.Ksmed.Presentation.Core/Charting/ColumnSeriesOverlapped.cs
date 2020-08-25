using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;

namespace KProcess.Ksmed.Presentation.Core.Charting
{
    /// <summary>
    /// Représente une série de colonnes qui va se superpoer avec les autres.
    /// </summary>
    public class ColumnSeriesOverlapped : ColumnSeries
    {

        /// <summary>
        /// Obtient ou définit le ratio de la largeur de la colonne.
        /// </summary>
        public double WidthRatio
        {
            get { return (double)GetValue(WidthRatioProperty); }
            set { SetValue(WidthRatioProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="WidthRatio"/>.
        /// </summary>
        public static readonly DependencyProperty WidthRatioProperty =
            DependencyProperty.Register("WidthRatio", typeof(double), typeof(ColumnSeriesOverlapped), new UIPropertyMetadata(1.0));

        /// <summary>
        /// Updates each point.
        /// </summary>
        /// <param name="dataPoint">The data point to update.</param>
        protected override void UpdateDataPoint(DataPoint dataPoint)
        {
            if (SeriesHost == null)
                return;

            object category = dataPoint.ActualIndependentValue ?? (this.ActiveDataPoints.IndexOf(dataPoint) + 1);
            Range<UnitValue> coordinateRange = GetCategoryRange(category);

            if (!coordinateRange.HasData)
            {
                return;
            }
            else if (coordinateRange.Maximum.Unit != Unit.Pixels || coordinateRange.Minimum.Unit != Unit.Pixels)
            {
                throw new InvalidOperationException("DataPointSeriesWithAxes_ThisSeriesDoesNotSupportRadialAxes");
            }

            double minimum = (double)coordinateRange.Minimum.Value;
            double maximum = (double)coordinateRange.Maximum.Value;

            double plotAreaHeight = ActualDependentRangeAxis.GetPlotAreaCoordinate(ActualDependentRangeAxis.Range.Maximum).Value;
            IEnumerable<ColumnSeries> columnSeries = SeriesHost.Series.OfType<ColumnSeries>().Where(series => series.ActualIndependentAxis == ActualIndependentAxis);
            //int numberOfSeries = columnSeries.Count();
            int numberOfSeries = 1;
            double coordinateRangeWidth = (maximum - minimum);
            double segmentWidth = coordinateRangeWidth * 0.8;
            //double columnWidth = segmentWidth / numberOfSeries;
            double columnWidth = segmentWidth / numberOfSeries * this.WidthRatio;
            //int seriesIndex = columnSeries.IndexOf(this);
            int seriesIndex = 0;
            int realSeriesIndex = columnSeries.IndexOf(this);

            double dataPointY = ActualDependentRangeAxis.GetPlotAreaCoordinate(ToDouble(dataPoint.ActualDependentValue)).Value;
            double zeroPointY = ActualDependentRangeAxis.GetPlotAreaCoordinate(ActualDependentRangeAxis.Origin).Value;

            double offset = seriesIndex * Math.Round(columnWidth) + coordinateRangeWidth * 0.1;
            double dataPointX = minimum + offset;

            if (GetIsDataPointGrouped(category))
            {
                // Multiple DataPoints share this category; offset and overlap them appropriately
                IGrouping<object, DataPoint> categoryGrouping = GetDataPointGroup(category);
                int index = categoryGrouping.IndexOf(dataPoint);
                dataPointX += (index * (columnWidth * 0.2)) / (categoryGrouping.Count() - 1);
                columnWidth *= 0.8;
                Canvas.SetZIndex(dataPoint, -index);
            }

            if (CanGraph(dataPointY) && CanGraph(dataPointX) && CanGraph(zeroPointY))
            {
                dataPoint.Visibility = Visibility.Visible;

                double left = Math.Round(dataPointX);
                double width = Math.Round(columnWidth);

                double top = Math.Round(plotAreaHeight - Math.Max(dataPointY, zeroPointY) + 0.5);
                double bottom = Math.Round(plotAreaHeight - Math.Min(dataPointY, zeroPointY) + 0.5);
                double height = bottom - top + 1;

                Canvas.SetZIndex(dataPoint, realSeriesIndex);
                Canvas.SetLeft(dataPoint, left);
                Canvas.SetTop(dataPoint, top);
                dataPoint.Width = width;
                dataPoint.Height = height;
            }
            else
            {
                dataPoint.Visibility = Visibility.Collapsed;
            }
        }

        private static bool CanGraph(double value)
        {
            return !double.IsNaN(value) && !double.IsNegativeInfinity(value) && !double.IsPositiveInfinity(value) && !double.IsInfinity(value);
        }

        private static double ToDouble(object value)
        {
            return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }


    }
}
