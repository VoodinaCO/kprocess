using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using Syncfusion.Windows.Tools.Controls;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;

namespace KProcess.Ksmed.Presentation.Shell.Views
{
    public class SyncWizardBehavior : Behavior<WizardControl>
    {
        public static DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(nameof(IsEnabled), typeof(bool),
            typeof(SyncWizardBehavior), new FrameworkPropertyMetadata(false, OnIsEnabledChanged));

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
                    var existingBehavior = behColl.FirstOrDefault(b => b.GetType() == typeof(SyncWizardBehavior)) as SyncWizardBehavior;

                    if ((bool)e.NewValue == false && existingBehavior != null)
                        behColl.Remove(existingBehavior);
                    else if ((bool)e.NewValue == true && existingBehavior == null)
                        behColl.Add(new SyncWizardBehavior());
                }
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject != null)
                AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            if (AssociatedObject?.DataContext is IWizardViewModel context)
                context.Wizard = AssociatedObject;
        }
    }
}
