using Kprocess.KL2.TabletClient.Common;
using Syncfusion.UI.Xaml.Grid;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;

namespace Kprocess.KL2.TabletClient.Behaviors
{
    public class CustomColumnSizerBehavior : Behavior<SfDataGrid>
    {
        public static DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(nameof(IsEnabled), typeof(bool),
            typeof(CustomColumnSizerBehavior), new FrameworkPropertyMetadata(false, OnIsEnabledChanged));

        public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            SetIsEnabled(d, (bool)e.NewValue);

        public static void SetIsEnabled(DependencyObject d, bool value)
        {
            if (d is UIElement uie)
            {
                var behColl = Interaction.GetBehaviors(uie);
                if (behColl != null)
                {
                    var existingBehavior = behColl.FirstOrDefault(b => b.GetType() == typeof(CustomColumnSizerBehavior)) as CustomColumnSizerBehavior;

                    if (value == false && existingBehavior != null)
                        behColl.Remove(existingBehavior);
                    else if (value == true && existingBehavior == null)
                        behColl.Add(new CustomColumnSizerBehavior());
                }
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject?.GridColumnSizer != null)
                AssociatedObject.GridColumnSizer = new CustomColumnSizer(AssociatedObject);
        }
    }
}
