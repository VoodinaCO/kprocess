using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AnnotationsLib
{
    /// <summary>
    /// Logique d'interaction pour CanvasLayer.xaml
    /// </summary>
    public partial class CanvasLayer : ItemsControl
    {
        public AnnotationsControl AnnotationsControl { get; set; } = null;

        public ObservableCollection<T> GetItemsSource<T>()
        {
            return (ObservableCollection<T>)ItemsSource;
        }

        public CanvasLayer()
        {
            InitializeComponent();
        }

        public CanvasLayer(FrameworkElement adorneredElement) : this()
        {
            SetBinding(WidthProperty, new Binding(FrameworkElement.ActualWidthProperty.Name) { Source = adorneredElement });
            SetBinding(HeightProperty, new Binding(FrameworkElement.ActualHeightProperty.Name) { Source = adorneredElement });
            ItemsSource = new ObservableCollection<UIElement>();
        }

        public static CanvasLayer New<T>(FrameworkElement adorneredElement, string name = null)
        {
            return new CanvasLayer(adorneredElement) { ItemsSource = new ObservableCollection<T>(), Name = name };
        }
    }
}
