using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.DataVisualization.Charting;

namespace KProcess.Ksmed.Presentation.Core.Charting
{
    /// <summary>
    /// 
    /// </summary>
    public class KStackedColumnSeries : System.Windows.Controls.DataVisualization.Charting.StackedColumnSeries
    {

        /// <summary>
        /// Obtient ou définit une valeur indiquant si des marges doivent être insérées.
        /// </summary>
        public bool UseMargins
        {
            get { return (bool)GetValue(UseMarginsProperty); }
            set { SetValue(UseMarginsProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="UseMargins"/>.
        /// </summary>
        public static readonly DependencyProperty UseMarginsProperty =
            DependencyProperty.Register("UseMargins", typeof(bool), typeof(StackedColumnSeries), new UIPropertyMetadata(false));

        /// <summary>
        /// Returns the value margins for the data points of the series.
        /// </summary>
        /// <param name="valueMarginConsumer">Consumer of the value margins.</param>
        /// <returns>
        /// Sequence of value margins.
        /// </returns>
        protected override IEnumerable<ValueMargin> IValueMarginProviderGetValueMargins(IValueMarginConsumer valueMarginConsumer)
        {
            if (UseMargins)
                return base.IValueMarginProviderGetValueMargins(valueMarginConsumer);
            else
                return Enumerable.Empty<ValueMargin>();
        }

    }
}
