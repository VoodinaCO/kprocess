using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Markup;

namespace KProcess.Ksmed.Presentation.Core
{
    public class KDataGridFeatures : DependencyObject
    {
        #region HighlightWhenChildOverridenKActionPropertyPathValue
        public static string GetHighlightWhenChildOverridenKActionPropertyPathValue(DependencyObject obj)
        {
            return (string)obj.GetValue(HighlightWhenChildOverridenKActionPropertyPathValueProperty);
        }

        public static void SetHighlightWhenChildOverridenKActionPropertyPathValue(DependencyObject obj, string value)
        {
            obj.SetValue(HighlightWhenChildOverridenKActionPropertyPathValueProperty, value);
        }

        public static readonly DependencyProperty HighlightWhenChildOverridenKActionPropertyPathValueProperty =
            DependencyProperty.RegisterAttached("HighlightWhenChildOverridenKActionPropertyPathValue", typeof(string), typeof(KDataGridFeatures), new PropertyMetadata(null));
        #endregion

        #region CellVisibleWhenViewWBSTextBlock
        public static bool GetCellVisibleWhenViewWBSTextBlock(DependencyObject obj)
        {
            // TODO: attached property
            return (bool)obj.GetValue(CellVisibleWhenViewWBSTextBlockProperty);
        }

        public static void SetCellVisibleWhenViewWBSTextBlock(DependencyObject obj, bool value)
        {
            obj.SetValue(CellVisibleWhenViewWBSTextBlockProperty, value);
        }

        public static readonly DependencyProperty CellVisibleWhenViewWBSTextBlockProperty =
            DependencyProperty.RegisterAttached("CellVisibleWhenViewWBSTextBlock", typeof(bool), typeof(KDataGridFeatures), new PropertyMetadata(false));
        #endregion

        #region Binding
        public static KDataGridBindingDescription GetBinding(DependencyObject obj)
        {
            return (KDataGridBindingDescription)obj.GetValue(BindingProperty);
        }

        public static void SetBinding(DependencyObject obj, KDataGridBindingDescription value)
        {
            obj.SetValue(BindingProperty, value);
        }

        public static readonly DependencyProperty BindingProperty = DependencyProperty.RegisterAttached("Binding", typeof(KDataGridBindingDescription), typeof(KDataGridFeatures), new PropertyMetadata());
        #endregion

        #region BindingDisplay
        //public static KDataGridBindingDescription GetKActionBindingDisplay(DependencyObject obj)
        //{
        //    return (KDataGridBindingDescription)obj.GetValue(KActionBindingDisplayProperty);
        //}

        //public static void SetKActionBindingDisplay(DependencyObject obj, KDataGridBindingDescription value)
        //{
        //    obj.SetValue(KActionBindingDisplayProperty, value);
        //}

        //public static readonly DependencyProperty KActionBindingDisplayProperty = DependencyProperty.RegisterAttached("KActionBindingDisplay", typeof(KDataGridBindingDescription), typeof(KDataGridFeatures), new PropertyMetadata());


        public static string GetHighlightWhenChildOverridenKActionPropertyPathDisplay(DependencyObject obj)
        {
            return (string)obj.GetValue(HighlightWhenChildOverridenKActionPropertyPathDisplayProperty);
        }

        public static void SetHighlightWhenChildOverridenKActionPropertyPathDisplay(DependencyObject obj, string value)
        {
            obj.SetValue(HighlightWhenChildOverridenKActionPropertyPathDisplayProperty, value);
        }

        public static readonly DependencyProperty HighlightWhenChildOverridenKActionPropertyPathDisplayProperty = DependencyProperty.RegisterAttached("HighlightWhenChildOverridenKActionPropertyPathDisplay", typeof(string), typeof(KDataGridFeatures), new PropertyMetadata());


        public static IValueConverter GetHighlightWhenChildOverridenKActionPropertyPathDisplayConverter(DependencyObject obj)
        {
            return (IValueConverter)obj.GetValue(HighlightWhenChildOverridenKActionPropertyPathDisplayConverterProperty);
        }

        public static void SetHighlightWhenChildOverridenKActionPropertyPathDisplayConverter(DependencyObject obj, IValueConverter value)
        {
            obj.SetValue(HighlightWhenChildOverridenKActionPropertyPathDisplayConverterProperty, value);
        }

        public static readonly DependencyProperty HighlightWhenChildOverridenKActionPropertyPathDisplayConverterProperty = DependencyProperty.RegisterAttached("HighlightWhenChildOverridenKActionPropertyPathDisplayConverter", typeof(IValueConverter), typeof(KDataGridFeatures), new PropertyMetadata());

        #endregion

        #region CellVisibleWhenNotGroup
        public static bool GetCellVisibleWhenNotGroup(DependencyObject obj)
        {
            return (bool)obj.GetValue(CellVisibleWhenNotGroupProperty);
        }

        public static void SetCellVisibleWhenNotGroup(DependencyObject obj, bool value)
        {
            obj.SetValue(CellVisibleWhenNotGroupProperty, value);
        }

        public static readonly DependencyProperty CellVisibleWhenNotGroupProperty = DependencyProperty.RegisterAttached("CellVisibleWhenNotGroup", typeof(bool), typeof(KDataGridFeatures), new PropertyMetadata(false));
        #endregion
    }

    public class KDataGridBindingDescription
    {
        public BindingBase Value { get; set; }
        
        public static implicit operator BindingBase(KDataGridBindingDescription description)
        {
            return description.Value;
        }
    }
}
