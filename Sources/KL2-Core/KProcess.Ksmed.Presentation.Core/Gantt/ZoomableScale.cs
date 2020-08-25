using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using DlhSoft.Windows.Controls;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    /// <summary>
    /// Représente une échelle qui gère le zoom
    /// </summary>
    public class ZoomableScale : Scale
    {

        /// <summary>
        /// Initialise la classe <see cref="ZoomableScale"/>.
        /// </summary>
        static ZoomableScale()
        {
            ScaleTypeProperty.OverrideMetadata(typeof(ZoomableScale), new UIPropertyMetadata(DlhSoft.Windows.Controls.ScaleType.Custom));
        }

        /// <summary>
        /// Obtient ou définit la largeur du chart auquel l'échelle appartient.
        /// </summary>
        public double ChartWidth
        {
            get { return (double)GetValue(ChartWidthProperty); }
            set { SetValue(ChartWidthProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="ChartWidth"/>.
        /// </summary>
        public static readonly DependencyProperty ChartWidthProperty =
            DependencyProperty.Register("ChartWidth", typeof(double), typeof(ZoomableScale), new UIPropertyMetadata(0d));

        /// <summary>
        /// Calcule les intervales en fonction du zoom, de la timeline affichée et de la largeur du chart.
        /// </summary>
        /// <param name="zoom">Le zoom appliqué.</param>
        public void CalculateInitialIntervals(double zoom)
        {
            var timeService = IoC.Resolve<IServiceBus>().Get<ITimeTicksFormatService>();

            var start = this.GanttChartView.TimelinePageStart;
            var finish = GanttDates.ToDateTime((this.GanttChartView.TimelinePageFinish - start).Ticks / 5);
            var overallFinish = this.GanttChartView.TimelinePageFinish;

            this.Intervals.Clear();

            var total = finish - start;

            var maxIntervalsCount = this.ChartWidth / 70;

            var timeSpanMinIncrement = TimeSpan.FromHours(total.TotalHours / maxIntervalsCount);

            long realIncrementTicks;

            if (timeSpanMinIncrement.TotalSeconds < 1)
            {
                realIncrementTicks = timeSpanMinIncrement.Ticks;
            }
            else if (timeSpanMinIncrement.TotalMinutes < 1)
            {
                realIncrementTicks = TimeSpan.FromSeconds(Math.Ceiling(timeSpanMinIncrement.TotalSeconds)).Ticks;
            }
            else if (timeSpanMinIncrement.TotalHours < 1)
            {
                realIncrementTicks = TimeSpan.FromMinutes(Math.Ceiling(timeSpanMinIncrement.TotalMinutes)).Ticks;
            }
            else
            {
                realIncrementTicks = TimeSpan.FromHours(Math.Ceiling(timeSpanMinIncrement.TotalHours)).Ticks;
            }

            var currentDate = start;
            while (currentDate < overallFinish)
            {
                var nextDate = currentDate.AddTicks(Convert.ToInt64(realIncrementTicks / zoom));
                var interval = new CustomScaleInterval(currentDate, nextDate);
                var timeSpan = currentDate - start;

                interval.HeaderContent = timeService.TicksToString(timeSpan.Ticks);
                this.Intervals.Add(interval);
                currentDate = nextDate;
            }

        }

        protected override Freezable CreateInstanceCore()
        {
            return new ZoomableScale();
        }

    }

    /// <summary>
    /// Représente une intervalle avec des informations complémentaires.
    /// </summary>
    public class CustomScaleInterval : ScaleInterval
    {
        public CustomScaleInterval(DateTime start, DateTime finish)
            : base(start, finish)
        {
        }

        /// <summary>
        /// Obtient ou définit le tooltip affiché dans l'entête.
        /// </summary>
        public object HeaderTooltip
        {
            get { return (object)GetValue(HeaderTooltipProperty); }
            set { SetValue(HeaderTooltipProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="HeaderTooltip"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderTooltipProperty =
            DependencyProperty.Register("HeaderTooltip", typeof(object), typeof(CustomScaleInterval), new UIPropertyMetadata(null));


    }
}
