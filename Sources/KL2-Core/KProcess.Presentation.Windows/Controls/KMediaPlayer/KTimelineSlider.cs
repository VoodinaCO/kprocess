using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

#pragma warning disable 1591

namespace KProcess.Presentation.Windows.Controls
{
    /// <summary>
    /// Slider de marqueur du début/fin.
    /// </summary>
    public class KTimelineSlider : Slider
    {
        public KTimelineSlider()
        {
            this.Loaded += KTimelineSlider_Loaded;
            this.Unloaded += KTimelineSlider_Unloaded;
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si c'est le marqueur du haut (de fin).
        /// </summary>
        public bool IsUpper
        {
            get { return (bool)GetValue(IsUpperProperty); }
            set { SetValue(IsUpperProperty, value); }
        }
        /// <summary>
        /// Identifie la propriété de dépendance <see cref="IsUpper"/>.
        /// </summary>
        public static readonly DependencyProperty IsUpperProperty =
            DependencyProperty.Register("IsUpper", typeof(bool), typeof(KTimelineSlider),
            new UIPropertyMetadata(false));

        private KMediaPlayer _parentPlayer;

        void KTimelineSlider_Loaded(object sender, RoutedEventArgs e)
        {
            if (_parentPlayer == null)
                _parentPlayer = this.TryFindParent<KMediaPlayer>();

            if (this.IsUpper)
                this.SetBinding(Slider.ValueProperty, new Binding(_parentPlayer.EndBindingPath) { Mode = BindingMode.TwoWay });
            else
                this.SetBinding(Slider.ValueProperty, new Binding(_parentPlayer.StartBindingPath) { Mode = BindingMode.TwoWay });


            this.PreviewMouseLeftButtonDown -= KTimelineSlider_PreviewMouseLeftButtonDown;
            this.ValueChanged -= KTimelineSlider_ValueChanged;

            this.PreviewMouseLeftButtonDown += KTimelineSlider_PreviewMouseLeftButtonDown;
            this.ValueChanged += KTimelineSlider_ValueChanged;
        }

        void KTimelineSlider_Unloaded(object sender, RoutedEventArgs e)
        {
            BindingOperations.ClearBinding(this, Slider.ValueProperty);
            this.PreviewMouseLeftButtonDown -= KTimelineSlider_PreviewMouseLeftButtonDown;
            this.ValueChanged -= KTimelineSlider_ValueChanged;
        }

        void KTimelineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _parentPlayer.OnMarkerSliderValueChanged(this, e.NewValue);
        }

        void KTimelineSlider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.IsUpper)
                _parentPlayer.OnUpperMarkerSliderMouseLeftButtonDown(this.DataContext);
            else
                _parentPlayer.OnLowerMarkerSliderMouseLeftButtonDown(this.DataContext);
        }
    }
}

#pragma warning restore 1591