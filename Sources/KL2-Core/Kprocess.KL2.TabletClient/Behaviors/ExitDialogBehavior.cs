using MahApps.Metro.Controls.Dialogs;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;

namespace Kprocess.KL2.TabletClient.Behaviors
{
    public class ExitDialogBehavior : Behavior<UIElement>
    {
        public static DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(nameof(IsEnabled), typeof(bool),
            typeof(ExitDialogBehavior), new FrameworkPropertyMetadata(false, OnIsEnabledChanged));

        public static DependencyProperty DialogProperty =
            DependencyProperty.RegisterAttached(nameof(Dialog), typeof(CustomDialog),
            typeof(ExitDialogBehavior), new FrameworkPropertyMetadata(null));

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
                    var existingBehavior = behColl.FirstOrDefault(b => b.GetType() == typeof(ExitDialogBehavior)) as ExitDialogBehavior;

                    if ((bool)e.NewValue == false && existingBehavior != null)
                        behColl.Remove(existingBehavior);
                    else if ((bool)e.NewValue == true && existingBehavior == null)
                        behColl.Add(new ExitDialogBehavior());
                }
            }
        }

        public CustomDialog Dialog
        {
            get => (CustomDialog)GetValue(DialogProperty);
            set => SetValue(DialogProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
                AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
        }

        async void AssociatedObject_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            await Locator.Navigation.PopModal();
        }
    }
}
