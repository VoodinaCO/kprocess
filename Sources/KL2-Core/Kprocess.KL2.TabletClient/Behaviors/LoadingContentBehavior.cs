using System.Windows;
using System.Windows.Interactivity;

namespace Kprocess.KL2.TabletClient.Behaviors
{
    public class LoadingContentBehavior : Behavior<MainWindow>
    {
        public static DependencyProperty ContentProperty =
            DependencyProperty.RegisterAttached(nameof(Content), typeof(UIElement),
            typeof(LoadingContentBehavior), new FrameworkPropertyMetadata(null));

        public UIElement Content
        {
            get => (UIElement)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
    }
}
