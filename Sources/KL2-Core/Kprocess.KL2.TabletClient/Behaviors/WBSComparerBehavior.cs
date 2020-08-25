using KProcess.Ksmed.Models;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;

namespace Kprocess.KL2.TabletClient.Behaviors
{
    public class WBSComparerBehavior : Behavior<SfDataGrid>
    {
        public static DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(nameof(IsEnabled), typeof(bool),
            typeof(WBSComparerBehavior), new FrameworkPropertyMetadata(false, OnIsEnabledChanged));

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
                    var existingBehavior = behColl.FirstOrDefault(b => b.GetType() == typeof(WBSComparerBehavior)) as WBSComparerBehavior;

                    if ((bool)e.NewValue == false && existingBehavior != null)
                        behColl.Remove(existingBehavior);
                    else if ((bool)e.NewValue == true && existingBehavior == null)
                        behColl.Add(new WBSComparerBehavior());
                }
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            if (AssociatedObject?.SortComparers != null)
                AssociatedObject.SortComparers.Add(new SortComparer { Comparer = new WBSComparer(), PropertyName = nameof(PublishedAction.WBS) });
        }
    }
}
