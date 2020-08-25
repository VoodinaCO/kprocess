using Kprocess.KL2.TabletClient.ViewModel;
using KProcess.Ksmed.Models;
using Syncfusion.UI.Xaml.Grid;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;

namespace Kprocess.KL2.TabletClient.Behaviors
{
    public class SyncSfDataGridBehavior : Behavior<SfDataGrid>
    {
        public static DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(nameof(IsEnabled), typeof(bool),
            typeof(SyncSfDataGridBehavior), new FrameworkPropertyMetadata(false, OnIsEnabledChanged));

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
                    var existingBehavior = behColl.FirstOrDefault(b => b.GetType() == typeof(SyncSfDataGridBehavior)) as SyncSfDataGridBehavior;

                    if ((bool)e.NewValue == false && existingBehavior != null)
                        behColl.Remove(existingBehavior);
                    else if ((bool)e.NewValue == true && existingBehavior == null)
                        behColl.Add(new SyncSfDataGridBehavior());
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
            if (AssociatedObject?.DataContext is ISfDataGridViewModel<PublishedAction> dataContextAction)
                dataContextAction.DataGrid = AssociatedObject;
            else if (AssociatedObject?.DataContext is ISfDataGridViewModel<AuditItem> dataContextAudit)
                dataContextAudit.DataGrid = AssociatedObject;
        }
    }
}
