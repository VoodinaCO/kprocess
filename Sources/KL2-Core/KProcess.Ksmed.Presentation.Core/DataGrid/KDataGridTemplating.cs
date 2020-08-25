using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace KProcess.Ksmed.Presentation.Core
{
    public class KDataGridTemplating : DependencyObject
    {
        public static BindingBase GetTextBinding(DependencyObject obj)
        {
            return (BindingBase)obj.GetValue(TextBindingProperty);
        }

        public static void SetTextBinding(DependencyObject obj, BindingBase value)
        {
            obj.SetValue(TextBindingProperty, value);
        }

        public static readonly DependencyProperty TextBindingProperty = DependencyProperty.RegisterAttached("TextBinding", typeof(BindingBase), typeof(KDataGridTemplating), new PropertyMetadata(TextBindingPopertyChanged));

        private static void TextBindingPopertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is BindingBase)
            {
                if (d is TextBlock)
                {
                    BindingOperations.SetBinding(d, TextBlock.TextProperty, (BindingBase)e.NewValue);
                }
                else if (d is TextBox)
                {
                    BindingOperations.SetBinding(d, TextBox.TextProperty, (BindingBase)e.NewValue);
                }
            }
        }
    }
}
