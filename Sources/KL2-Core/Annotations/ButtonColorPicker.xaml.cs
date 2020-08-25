using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AnnotationsLib
{
    /// <summary>
    /// Logique d'interaction pour ButtonColorPicker.xaml
    /// </summary>
    public partial class ButtonColorPicker : ComboBox
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ButtonColorPicker), new FrameworkPropertyMetadata(null));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ButtonColorPicker control = (ButtonColorPicker)d;
            if (control.Command?.CanExecute(control.SelectedItem) == true)
                control.Command?.Execute(control.SelectedItem);
        }

        static ButtonColorPicker()
        {
            SelectedItemProperty.OverrideMetadata(typeof(ButtonColorPicker), new FrameworkPropertyMetadata(OnSelectedItemPropertyChanged));
        }

        public ButtonColorPicker()
        {
            InitializeComponent();
        }
    }
}
