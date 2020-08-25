using System.Windows;
using System.Windows.Controls;

namespace AnnotationsLib.Annotations
{
    /// <summary>
    /// Logique d'interaction pour ActionsAnnotationAdorner.xaml
    /// </summary>
    public partial class ActionsAnnotationAdorner : ItemsControl
    {
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var test = (FrameworkElement)GetVisualChild(0);
            Width = test.DesiredSize.Width;
            Height = test.DesiredSize.Height;

            return base.ArrangeOverride(arrangeBounds);
        }

        public ActionsAnnotationAdorner()
        {
            InitializeComponent();
        }
    }
}
