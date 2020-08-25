using System.Linq;
using System.Windows;
using System.Windows.Interactivity;
using Unosquare.FFME;

namespace Kprocess.KL2.TabletClient.Behaviors
{
    public class DXVA2_Behavior : Behavior<MediaElement>
    {
        static readonly string DXVA2_Device = "dxva2";

        public static DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(nameof(IsEnabled), typeof(bool),
            typeof(DXVA2_Behavior), new FrameworkPropertyMetadata(false, OnIsEnabledChanged));

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
                    var existingBehavior = behColl.FirstOrDefault(b => b.GetType() == typeof(DXVA2_Behavior)) as DXVA2_Behavior;

                    if ((bool)e.NewValue == false && existingBehavior != null)
                        behColl.Remove(existingBehavior);
                    else if ((bool)e.NewValue == true && existingBehavior == null)
                        behColl.Add(new DXVA2_Behavior());
                }
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            /*if (AssociatedObject != null)
                AssociatedObject.MediaOpening += AssociatedObject_MediaOpening;*/
        }

        void AssociatedObject_MediaOpening(object sender, Unosquare.FFME.Common.MediaOpeningEventArgs e)
        {
            var dxva2 = e.Options.VideoStream.HardwareDevices.SingleOrDefault(_ => _.DeviceTypeName == DXVA2_Device);
            if (dxva2 != null)
                e.Options.VideoHardwareDevice = dxva2;
        }
    }
}
