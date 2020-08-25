using Kprocess.KL2.TabletClient.Common;
using Syncfusion.UI.Xaml.Grid;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;

namespace Kprocess.KL2.TabletClient.Behaviors
{
    public class LinkedPublicationDataGridSerializationBehavior : Behavior<SfDataGrid>
    {
        public static DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(nameof(IsEnabled), typeof(bool),
            typeof(LinkedPublicationDataGridSerializationBehavior), new FrameworkPropertyMetadata(false, OnIsEnabledChanged));

        public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement uie)
            {
                var behColl = Interaction.GetBehaviors(uie);
                if (behColl != null)
                {
                    var existingBehavior = behColl.FirstOrDefault(b => b.GetType() == typeof(LinkedPublicationDataGridSerializationBehavior)) as LinkedPublicationDataGridSerializationBehavior;

                    if ((bool)e.NewValue == false && existingBehavior != null)
                        behColl.Remove(existingBehavior);
                    else if ((bool)e.NewValue == true && existingBehavior == null)
                        behColl.Add(new LinkedPublicationDataGridSerializationBehavior());
                }
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SerializationController = new LinkedPublicationDataGridSerialization(AssociatedObject);
        }
    }
}
