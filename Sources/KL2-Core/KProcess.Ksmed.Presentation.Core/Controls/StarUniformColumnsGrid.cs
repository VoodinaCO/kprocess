using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    /// <summary>
    /// Représente un <see cref="Panel"/> dans lequel les éléments prennent toute la largeur disponible sauf s'il est possible de les faire
    /// rentrer sur plusieurs colonnes.
    /// </summary>
    public class StarUniformColumnsGrid : Panel
    {

        /// <summary>
        /// Gets or sets the ItemMinWidth.
        /// </summary>
        public double ItemMinWidth
        {
            get { return (double)GetValue(ItemMinWidthProperty); }
            set { SetValue(ItemMinWidthProperty, value); }
        }
        /// <summary>
        /// Identifies the <see cref="ItemMinWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemMinWidthProperty =
            DependencyProperty.Register("ItemMinWidth", typeof(double), typeof(StarUniformColumnsGrid), new UIPropertyMetadata(0.0));

        private int _rows;
        private int _columns;

        /// <summary>
        /// Mesure la taille du panel et de ses enfants.
        /// </summary>
        /// <param name="availableSize">La taille disponible</param>
        /// <returns>La taille voulue pour ce panel.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (ItemMinWidth == default(double))
                throw new InvalidOperationException("ItemMinWidth must be set.");

            Size itemAvailableSize;
            if (!double.IsPositiveInfinity(availableSize.Width))
            {
                _columns = (int)Math.Floor(availableSize.Width / ItemMinWidth);
                if (_columns == 0)
                    _columns = 1;

                double childrenCount = base.InternalChildren.Count < 0 ? 0 : base.VisualChildrenCount;

                _rows = (int)Math.Ceiling(childrenCount / _columns);

                var columnWidth = availableSize.Width / _columns;

                itemAvailableSize = new Size(columnWidth, availableSize.Height);
            }
            else
            {
                itemAvailableSize = availableSize;
                _columns = base.InternalChildren.Count;
                _rows = 1;
            }

            double width = 0.0;
            double height = 0.0;

            int rowIndex = 0;

            var children = base.InternalChildren.OfType<UIElement>().ToArray();
            foreach (UIElement element in children)
            {
                element.Measure(itemAvailableSize);

                if (width < element.DesiredSize.Width)
                    width = element.DesiredSize.Width;

                if (rowIndex % _columns == 0)
                    height += element.DesiredSize.Height;
                rowIndex++;
            }

            return new Size(width * _columns, height);
        }

        /// <summary>
        /// Positionne les enfants du panel.
        /// </summary>
        /// <param name="finalSize">La taille finalement disponible.</param>
        /// <returns>
        /// La taille utilisée.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var itemFinalRect = new Rect(0.0, 0.0, finalSize.Width / _columns, finalSize.Height / _rows);

            double width = itemFinalRect.Width;
            double availableWidth = Math.Max(finalSize.Width - 1.0, 0.0);

            foreach (UIElement element in base.InternalChildren)
            {
                itemFinalRect.Height = element.DesiredSize.Height;
                element.Arrange(itemFinalRect);
                if (element.Visibility != Visibility.Collapsed)
                {
                    itemFinalRect.X += width;
                    if (itemFinalRect.X >= availableWidth)
                    {
                        itemFinalRect.Y += itemFinalRect.Height;
                        itemFinalRect.X = 0.0;
                    }
                }
            }

            return finalSize;
        }

    }
}
