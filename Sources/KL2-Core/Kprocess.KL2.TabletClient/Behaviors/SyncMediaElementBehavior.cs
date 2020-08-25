using Kprocess.KL2.TabletClient.ViewModel;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;
using Unosquare.FFME;

namespace Kprocess.KL2.TabletClient.Behaviors
{
    public class SyncMediaElementBehavior : Behavior<MediaElement>
    {
        public static DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(nameof(IsEnabled), typeof(bool),
            typeof(SyncMediaElementBehavior), new FrameworkPropertyMetadata(false, OnIsEnabledChanged));

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
                    var existingBehavior = behColl.FirstOrDefault(b => b.GetType() == typeof(SyncMediaElementBehavior)) as SyncMediaElementBehavior;

                    if ((bool)e.NewValue == false && existingBehavior != null)
                        behColl.Remove(existingBehavior);
                    else if ((bool)e.NewValue == true && existingBehavior == null)
                        behColl.Add(new SyncMediaElementBehavior());
                }
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
                AssociatedObject.DataContextChanged += AssociatedObject_DataContextChanged;
        }

        void AssociatedObject_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is IMediaElementViewModel dataContext)
                dataContext.MediaElement = AssociatedObject;
        }
    }
}
