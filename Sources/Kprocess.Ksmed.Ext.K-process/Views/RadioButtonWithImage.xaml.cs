using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KProcess.Ksmed.Ext.Kprocess.Views
{
    /// <summary>
    /// Logique d'interaction pour RadioButtonWithImage.xaml
    /// </summary>
    public partial class RadioButtonWithImage : RadioButton
    {
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(RadioButtonWithImage));
        public static readonly DependencyProperty CheckedBrushProperty = DependencyProperty.Register("CheckedBrush", typeof(Brush), typeof(RadioButtonWithImage));
        public static readonly DependencyProperty NormalBrushProperty = DependencyProperty.Register("NormalBrush", typeof(Brush), typeof(RadioButtonWithImage));

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public Brush CheckedBrush
        {
            get { return (Brush)GetValue(CheckedBrushProperty); }
            set { SetValue(CheckedBrushProperty, value); }
        }

        public Brush NormalBrush
        {
            get { return (Brush)GetValue(NormalBrushProperty); }
            set { SetValue(NormalBrushProperty, value); }
        }

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RadioButtonWithImage control = (RadioButtonWithImage)d;
            if (control.IsChecked == true)
            {
                if (control.Command?.CanExecute(control.CommandParameter) == true)
                    control.Command?.Execute(control.CommandParameter);
            }
        }

        static RadioButtonWithImage()
        {
            CommandProperty.OverrideMetadata(typeof(RadioButtonWithImage), new FrameworkPropertyMetadata(OnCommandPropertyChanged));
            CommandParameterProperty.OverrideMetadata(typeof(RadioButtonWithImage), new FrameworkPropertyMetadata(OnCommandPropertyChanged));
        }

        public RadioButtonWithImage()
        {
            InitializeComponent();
        }
    }
}
