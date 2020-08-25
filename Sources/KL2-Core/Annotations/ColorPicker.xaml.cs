using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AnnotationsLib
{
    /// <summary>
    /// Logique d'interaction pour ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : ComboBox
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ColorPicker), new FrameworkPropertyMetadata(null, OnCommandPropertyChanged));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                OnSelectedItemPropertyChanged(d, e);
        }

        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker control = (ColorPicker)d;
            if (control.Command?.CanExecute(control.SelectedItem) == true)
                control.Command?.Execute(control.SelectedItem);
        }

        static ColorPicker()
        {
            SelectedItemProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(OnSelectedItemPropertyChanged));
        }

        public ColorPicker()
        {
            InitializeComponent();
        }
    }
}
