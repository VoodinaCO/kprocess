using KProcess.Ksmed.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace KProcess.Ksmed.Presentation.Core.Controls
{
    public class ColorPicker : Control
    {
        #region Members

        private Popup _colorPickerCanvasPopup;
        private ListBox _availableColors;
        private ListBox _standardColors;

        #endregion //Members

        #region Properties

        #region AvailableColors

        public static readonly DependencyProperty AvailableColorsProperty = DependencyProperty.Register("AvailableColors", typeof(ObservableCollection<Color>), typeof(ColorPicker), new UIPropertyMetadata(CreateAvailableColors()));
        public ObservableCollection<Color> AvailableColors
        {
            get { return (ObservableCollection<Color>)GetValue(AvailableColorsProperty); }
            set { SetValue(AvailableColorsProperty, value); }
        }

        #endregion //AvailableColors

        #region ButtonStyle

        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register("ButtonStyle", typeof(Style), typeof(ColorPicker));
        public Style ButtonStyle
        {
            get { return (Style)GetValue(ButtonStyleProperty); }
            set { SetValue(ButtonStyleProperty, value); }
        }

        #endregion //ButtonStyle

        #region IsOpen

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(ColorPicker), new UIPropertyMetadata(false));
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        #endregion //IsOpen

        #region SelectedColor

        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker), new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnSelectedColorPropertyChanged)));
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        private static void OnSelectedColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker colorPicker = (ColorPicker)d;
            if (colorPicker != null)
                colorPicker.OnSelectedColorChanged((Color)e.OldValue, (Color)e.NewValue);
        }

        private void OnSelectedColorChanged(Color oldValue, Color newValue)
        {
            RoutedPropertyChangedEventArgs<Color> args = new RoutedPropertyChangedEventArgs<Color>(oldValue, newValue);
            args.RoutedEvent = ColorPicker.SelectedColorChangedEvent;
            RaiseEvent(args);

            RefreshListboxSelection();
        }

        #endregion //SelectedColor

        #region ShowAvailableColors

        public static readonly DependencyProperty ShowAvailableColorsProperty = DependencyProperty.Register("ShowAvailableColors", typeof(bool), typeof(ColorPicker), new UIPropertyMetadata(true));
        public bool ShowAvailableColors
        {
            get { return (bool)GetValue(ShowAvailableColorsProperty); }
            set { SetValue(ShowAvailableColorsProperty, value); }
        }

        #endregion //ShowAvailableColors

        #region ShowStandardColors

        public static readonly DependencyProperty ShowStandardColorsProperty = DependencyProperty.Register("ShowStandardColors", typeof(bool), typeof(ColorPicker), new UIPropertyMetadata(true));
        public bool ShowStandardColors
        {
            get { return (bool)GetValue(ShowStandardColorsProperty); }
            set { SetValue(ShowStandardColorsProperty, value); }
        }

        #endregion //DisplayStandardColors

        #region StandardColors

        public static readonly DependencyProperty StandardColorsProperty = DependencyProperty.Register("StandardColors", typeof(ObservableCollection<Color>), typeof(ColorPicker), new UIPropertyMetadata(CreateStandardColors()));
        public ObservableCollection<Color> StandardColors
        {
            get { return (ObservableCollection<Color>)GetValue(StandardColorsProperty); }
            set { SetValue(StandardColorsProperty, value); }
        }

        #endregion //StandardColors

        #endregion //Properties

        #region Constructors

        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        public ColorPicker()
        {
            Keyboard.AddKeyDownHandler(this, OnKeyDown);
            Mouse.AddPreviewMouseDownOutsideCapturedElementHandler(this, OnMouseDownOutsideCapturedElement);
        }

        #endregion //Constructors

        #region Base Class Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _colorPickerCanvasPopup = (Popup)GetTemplateChild("PART_ColorPickerPalettePopup");
            //_colorPickerCanvasPopup.Opened += ColorPickerCanvasPopup_Opened;

            _availableColors = (ListBox)GetTemplateChild("PART_AvailableColors");
            _availableColors.SelectionChanged += Color_SelectionChanged;

            _standardColors = (ListBox)GetTemplateChild("PART_StandardColors");
            _standardColors.SelectionChanged += Color_SelectionChanged;

            RefreshListboxSelection();
        }

        #endregion //Base Class Overrides

        #region Event Handlers

        void ColorPickerCanvasPopup_Opened(object sender, EventArgs e)
        {

        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                case Key.Tab:
                    {
                        CloseColorPicker();
                        break;
                    }
            }
        }

        private void OnMouseDownOutsideCapturedElement(object sender, MouseButtonEventArgs e)
        {
            CloseColorPicker();
        }

        private void Color_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;

            if (e.AddedItems.Count > 0)
            {
                var colorItem = (Color)e.AddedItems[0];
                SelectedColor = colorItem;
                CloseColorPicker();
                //lb.SelectedIndex = -1; //for now I don't care about keeping track of the selected color
            }

            RefreshListboxSelection();
        }

        private void RefreshListboxSelection()
        {
            if(_availableColors != null && _standardColors != null && this.AvailableColors != null && this.StandardColors != null)
            {
                if (this.AvailableColors.Contains(this.SelectedColor))
                {
                    _availableColors.SelectedItem = this.SelectedColor;
                    _standardColors.SelectedIndex = -1;
                }

                else if (this.StandardColors.Contains(this.SelectedColor))
                {
                    _standardColors.SelectedItem = this.SelectedColor;
                    _availableColors.SelectedIndex = -1;
                }
            }
        }

        #endregion //Event Handlers

        #region Events

        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent("SelectedColorChanged", RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Color>), typeof(ColorPicker));
        public event RoutedPropertyChangedEventHandler<Color> SelectedColorChanged
        {
            add { AddHandler(SelectedColorChangedEvent, value); }
            remove { RemoveHandler(SelectedColorChangedEvent, value); }
        }

        #endregion //Events

        #region Methods

        private void CloseColorPicker()
        {
            if (IsOpen)
                IsOpen = false;
            ReleaseMouseCapture();
        }

        private static ObservableCollection<Color> CreateStandardColors()
        {
            return ColorsHelper.StandardColorsExcludedGreenYellowOrangeRed
                .Union(ColorsHelper.StandardColors)
                .Except(CreateAvailableColors()).ToObservableCollection();
        }

        private static ObservableCollection<Color> CreateAvailableColors()
        {
            var colors = new ObservableCollection<Color>();

            colors.AddRange(ColorsHelper.StandardOfficeColors);

            int colorsCount = colors.Count;

            var darkLightColors = new Color[colorsCount][];
            for (int i = 0; i < colorsCount; i++)
                darkLightColors[i] = ColorsHelper.GetDarkLight(colors[i]);

            for (int line = 0; line < ColorsHelper.DarkLightColorsCount; line++)
            {
                for (int column = 0; column < colorsCount; column++)
                {
                    colors.Add(darkLightColors[column][line]);
                }
            }
            return colors;
        }

        #endregion //Methods
    }
}
