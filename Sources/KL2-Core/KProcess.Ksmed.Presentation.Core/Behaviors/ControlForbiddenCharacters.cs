using KProcess.Globalization;
using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Permet le blocage du texte permettant de l'injection de code par Excel/>.
    /// </summary>
    public static class ControlForbiddenCharacters
    {
        public static ICollection<ForbiddenCharacter> ForbiddenCharacters = new Collection<ForbiddenCharacter>()
        {
            new ForbiddenCharacter("Equals", "="),
            new ForbiddenCharacter("Plus", "+"),
            new ForbiddenCharacter("Minus", "-"),
            new ForbiddenCharacter("At", "@")
        };

        /// <summary>
        /// Active le blocage.
        /// </summary>
        public static void ActivateInputElement()
        {
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewTextInputEvent, (TextCompositionEventHandler)OnPreviewTextInput);
            CommandManager.RegisterClassCommandBinding(typeof(TextBox), new CommandBinding(ApplicationCommands.Paste, PasteExecuted));
            // Gérer les suppressions de caractères (Suppr, Back, Cut)
            CommandManager.RegisterClassCommandBinding(typeof(TextBox), new CommandBinding(ApplicationCommands.Cut, CutExecuted));
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewKeyDownEvent, (KeyEventHandler)OnPreviewKeyDown);
        }

        static void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var tb = (TextBox)sender;

            var caretIndex = tb.CaretIndex;

            if (ForbiddenCharacters.Any(_ => e.Text?.StartsWith(_.Character) == true
                                             && caretIndex == 0
                                             && !_.GetAllow(tb)))
            {
                SetTooltip(tb);
                e.Handled = true;
            }
        }

        static void PasteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var tb = (TextBox)sender;

            var caretIndex = tb.CaretIndex;

            if (Clipboard.ContainsData(DataFormats.Text))
            {
                string pasteText = Clipboard.GetData(DataFormats.Text) as string;
                if (!ForbiddenCharacters.Any(_ => pasteText?.StartsWith(_.Character) == true
                                                  && caretIndex == 0
                                                  && !_.GetAllow(tb)))
                {
                    tb.Paste();
                }
                else
                {
                    SetTooltip(tb);
                }
            }
        }

        static void CutExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var tb = (TextBox)sender;

            var caretIndex = tb.CaretIndex;
            if (caretIndex != 0)
            {
                tb.Cut();
                return;
            }

            var computedText = tb.Text.Remove(0, tb.SelectionLength);
            if (!ForbiddenCharacters.Any(_ => computedText?.StartsWith(_.Character) == true
                                              && !_.GetAllow(tb)))
            {
                tb.Cut();
            }
            else
            {
                SetTooltip(tb);
            }
        }

        static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var tb = (TextBox)sender;

            var caretIndex = tb.CaretIndex;

            if (e.Key == Key.Delete && caretIndex == 0)
            {
                if (string.IsNullOrEmpty(tb.Text))
                    return;
                var computedText = tb.Text.Remove(0, Math.Max(tb.SelectionLength, 1));
                if (ForbiddenCharacters.Any(_ => computedText?.StartsWith(_.Character) == true
                                                 && !_.GetAllow(tb)))
                {
                    SetTooltip(tb);
                    e.Handled = true;
                }
            }

            if (e.Key == Key.Back)
            {
                if (string.IsNullOrEmpty(tb.Text))
                    return;
                if (caretIndex == 0)
                {
                    var computedText = tb.Text.Remove(0, tb.SelectionLength);
                    if (ForbiddenCharacters.Any(_ => computedText?.StartsWith(_.Character) == true
                                                     && !_.GetAllow(tb)))
                    {
                        SetTooltip(tb);
                        e.Handled = true;
                    }
                }
                else if (caretIndex == 1 && tb.SelectionLength == 0)
                {
                    var computedText = tb.Text.Remove(0, 1);
                    if (ForbiddenCharacters.Any(_ => computedText?.StartsWith(_.Character) == true
                                                     && !_.GetAllow(tb)))
                    {
                        SetTooltip(tb);
                        e.Handled = true;
                    }
                }
            }
        }

        static readonly Dictionary<UIElement, ToolTip> _toolTips = new Dictionary<UIElement, ToolTip>();

        static void SetTooltip(TextBox tb)
        {
            if (_toolTips.ContainsKey(tb))
                return;
            var tooltip = new ToolTip()
            {
                Content = LocalizationManager.GetStringFormat("Common_ForbiddenCharacters", string.Join("', '", ForbiddenCharacters.Where(_ => !_.GetAllow(tb)).Select(_ => _.Character))),
                Placement = PlacementMode.Bottom,
                PlacementTarget = tb
            };
            tooltip.IsOpen = true;
            tooltip.StaysOpen = false;
            tooltip.Closed += (sender, args) =>
            {
                UIElement key = ((ToolTip)sender).PlacementTarget;
                if (_toolTips.ContainsKey(key))
                    _toolTips.Remove(key);
            };
            _toolTips.Add(tb, tooltip);
        }

    }

    public class ForbiddenCharacter
    {
        public readonly string Name;

        public readonly string Character;

        public bool GetAllow(DependencyObject obj)
        {
            var behaviors = new List<AllowCharacter>();
            var currentObj = obj;
            while (currentObj != null)
            {
                behaviors.AddRange(Interaction.GetBehaviors(currentObj).OfType<AllowCharacter>());
                currentObj = currentObj.GetParentObject();
            }
            return behaviors.Any(_ => _.Character == Character);
        }

        public ForbiddenCharacter(string name, string character)
        {
            Name = name;
            Character = character;
        }
    }

    public class AllowCharacter : Behavior<UIElement>
    {
        public string Character
        {
            get => (string)GetValue(CharacterProperty);
            set => SetValue(CharacterProperty, value);
        }

        public static readonly DependencyProperty CharacterProperty = DependencyProperty.Register(nameof(Character), typeof(string), typeof(AllowCharacter), new PropertyMetadata());
    }
}
