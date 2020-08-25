using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KProcess.Presentation.Windows.Helpers
{
    /// <summary>
    /// Permet de rajouter un MaxLength sur un ComboBox
    /// </summary>
    public class EditableComboBox
    {
        public static int GetMaxLength(DependencyObject obj)
        {
            return (int)obj.GetValue(MaxLengthProperty);
        }

        public static void SetMaxLength(DependencyObject obj, int value)
        {
            obj.SetValue(MaxLengthProperty, value);
        }

        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.RegisterAttached("MaxLength", typeof(int), typeof(EditableComboBox), new UIPropertyMetadata(OnMaxLenghtChanged));

        private static void OnMaxLenghtChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var comboBox = obj as ComboBox;
            if (comboBox == null) return;

            comboBox.Loaded +=
                (s, e) =>
                {
                    var textBox = (TextBox)comboBox.FindChildren(_ => _.GetType() == typeof(TextBox) && ((TextBox)_).Name.Equals("PART_EditableTextBox")).DefaultIfEmpty(null).FirstOrDefault();
                    if (textBox == null) return;

                    textBox.SetValue(TextBox.MaxLengthProperty, args.NewValue);
                };
        }
    }
}
