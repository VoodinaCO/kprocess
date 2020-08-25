using System.Windows;
using System.Windows.Controls.Primitives;

namespace AnnotationsLib
{
    /// <summary>
    /// Logique d'interaction pour AnnotationsControlMenu.xaml
    /// </summary>
    public partial class AnnotationsControlMenu : UniformGrid
    {
        public static readonly DependencyProperty ItemSizeProperty = DependencyProperty.Register("ItemSize", typeof(double), typeof(AnnotationsControlMenu), new FrameworkPropertyMetadata(20d));

        public double ItemSize
        {
            get { return (double)GetValue(ItemSizeProperty); }
            set { SetValue(ItemSizeProperty, value); }
        }

        public AnnotationsControlMenu()
        {
            InitializeComponent();
        }
    }
}
