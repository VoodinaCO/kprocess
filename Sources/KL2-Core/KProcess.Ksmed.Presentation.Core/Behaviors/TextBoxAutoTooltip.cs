using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace KProcess.Ksmed.Presentation.Core.Behaviors
{
    /// <summary>
    /// Permet d'activer le Tooltip lorsque le contenu de la cellule n'est pas entièrement affiché.
    /// </summary>
    public static class TextBoxAutoTooltip
    {

        public static bool GetAttach(DependencyObject obj)
        {
            return (bool)obj.GetValue(AttachProperty);
        }

        public static void SetAttach(DependencyObject obj, bool value)
        {
            obj.SetValue(AttachProperty, value);
        }

        /// <summary>
        /// Identifie la clé de la propriété de dépendance <see cref="Attach"/>.
        /// </summary>
        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(TextBoxAutoTooltip),
            new PropertyMetadata(false, OnAttachPropertyChanged));

        private static void OnAttachPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                d.SetValue(TextBlockToolTipPropertyKey, TextBlockTooltipAttachment.Attach((TextBlock)d, null));
            }
            else
            {
                var attached = d.GetValue(TextBlockToolTipPropertyKey.DependencyProperty) as TextBlockTooltipAttachment;
                if (attached != null)
                    attached.Dispose();
            }
        }

        /// <summary>
        /// Identifie la clé de la propriété de dépendance <see cref="TextBlockToolTip"/>.
        /// </summary>
        private static readonly DependencyPropertyKey TextBlockToolTipPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("TextBlockToolTip", typeof(TextBlockTooltipAttachment), typeof(TextBoxAutoTooltip), new UIPropertyMetadata(null));


        public static void Attach(TextBlock tb, FrameworkElement parent)
        {
            TextBlockTooltipAttachment.Attach(tb, parent);
        }


        /// <summary>
        /// Wrapper interne
        /// </summary>
        private class TextBlockTooltipAttachment : IDisposable
        {
            private TextBlock _tb;
            private FrameworkElement _parent;
            private bool _isTooltipActivated;
            private bool _hasBeenInitialisedOnce;

            private TextBlockTooltipAttachment(TextBlock tb, FrameworkElement parent)
            {
                Debug.Assert(tb.TextTrimming == TextTrimming.None, "TextTrimming non supporté car les calculs des tailles changent");

                _tb = tb;
                _parent = parent;

                _tb.Loaded += _tb_Loaded;
                _tb.Unloaded += _tb_Unloaded;

                if (tb.IsLoaded)
                    Update();
            }

            public static TextBlockTooltipAttachment Attach(TextBlock tb, FrameworkElement parent)
            {
                return new TextBlockTooltipAttachment(tb, parent);
            }

            private void _tb_Loaded(object sender, RoutedEventArgs e)
            {
                if (_parent == null)
                {
                    _parent = VisualTreeHelperExtensions.TryFindParent<FrameworkElement>(_tb);

                    if (_parent == null)
                        throw new InvalidOperationException("Parent introuvable");
                }

                _parent.SizeChanged += cell_SizeChanged;
                Update();
            }
            private void _tb_Unloaded(object sender, RoutedEventArgs e)
            {
                _parent.SizeChanged -= cell_SizeChanged;
            }

            private void cell_SizeChanged(object sender, SizeChangedEventArgs e)
            {
                Update();
            }

            private void Update()
            {
                var text = _tb.Text;

                var topLeftTranslation = _tb.TranslatePoint(new Point(0, 0), _parent);
                var leftMargin = topLeftTranslation.X;
                var topMargin = topLeftTranslation.Y;

                var isTrimmed = _tb.ActualWidth + leftMargin + _tb.Margin.Right > _parent.ActualWidth ||
                    _tb.ActualHeight + topMargin + _tb.Margin.Bottom > _parent.ActualHeight;

                if (isTrimmed != _isTooltipActivated)
                {
                    if (isTrimmed)
                    {
                        if (!_hasBeenInitialisedOnce)
                        {
                            var binding = new Binding
                            {
                                RelativeSource = new RelativeSource(RelativeSourceMode.Self),
                                Path = new PropertyPath("Text"),
                            };
                            BindingOperations.SetBinding(_tb, FrameworkElement.ToolTipProperty, binding);

                            ToolTipService.SetInitialShowDelay(_tb, 0);
                            ToolTipService.SetPlacement(_tb, PlacementMode.Relative);
                            ToolTipService.SetHorizontalOffset(_tb, -6);
                            ToolTipService.SetVerticalOffset(_tb, -4);

                            _hasBeenInitialisedOnce = true;
                        }

                        ToolTipService.SetIsEnabled(_tb, true);
                    }
                    else
                    {
                        ToolTipService.SetIsEnabled(_tb, false);
                    }

                    _isTooltipActivated = isTrimmed;
                }
            }

            #region IDisposable Members

            public void Dispose()
            {
                _tb.Loaded -= _tb_Loaded;
                _tb.Unloaded -= _tb_Unloaded;
                _parent.SizeChanged -= cell_SizeChanged;
            }

            #endregion
        }

    }
}
