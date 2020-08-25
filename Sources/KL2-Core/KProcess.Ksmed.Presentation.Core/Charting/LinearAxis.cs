using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.DataVisualization;
using System.Windows.Controls.DataVisualization.Charting;

namespace KProcess.Ksmed.Presentation.Core.Charting
{
    public class KLinearAxis : System.Windows.Controls.DataVisualization.Charting.LinearAxis
    {
        /// <summary>
        /// Obtient ou définit la valeur minimale du maximum de l'axe.
        /// </summary>
        public double? MinimumMaximum
        {
            get { return (double?)GetValue(MinimumMaximumProperty); }
            set { SetValue(MinimumMaximumProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="MinimumMaximum"/>.
        /// </summary>
        public static readonly DependencyProperty MinimumMaximumProperty =
            DependencyProperty.Register("MinimumMaximum", typeof(double?), typeof(KLinearAxis), new UIPropertyMetadata(null));

        protected override Range<IComparable> OverrideDataRange(Range<IComparable> range)
        {

            range = base.OverrideDataRange(range);

            if (this.MinimumMaximum.HasValue && Compare(range.Maximum, this.MinimumMaximum.Value) < 0)
                return new Range<IComparable>(range.Minimum, this.MinimumMaximum.Value);
            else
                return range;
        }


        /// <summary>
        /// Compares two IComparables returning -1 if the left is null and 1 if
        /// the right is null.
        /// </summary>
        /// <param name="left">The left comparable.</param>
        /// <param name="right">The right comparable.</param>
        /// <returns>A value indicating which is larger.</returns>
        public static int Compare(IComparable left, IComparable right)
        {
            if (left == null && right == null)
            {
                return 0;
            }
            else if (left == null && right != null)
            {
                return -1;
            }
            else if (left != null && right == null)
            {
                return 1;
            }
            else
            {
                return left.CompareTo(right);
            }
        }

    }
}
