using AnnotationsLib.Annotations.Actions;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace AnnotationsLib.Annotations
{
    /// <summary>
    /// Logique d'interaction pour TextAnnotation.xaml
    /// </summary>
    [TemplatePart(Name = "PART_EDITABLE_TEXT", Type = typeof(TextBox))]
    public partial class TextAnnotation : AnnotationBase, IContentEditableAnnotation
    {
        #region Private members
        
        private TextBox part_editable_text;

        #endregion

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TextAnnotation), new FrameworkPropertyMetadata(string.Empty));
        public static readonly DependencyProperty ContentEditableProperty = DependencyProperty.Register("ContentEditable", typeof(bool), typeof(TextAnnotation), new FrameworkPropertyMetadata(false));

       public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool ContentEditable
        {
            get { return (bool)GetValue(ContentEditableProperty); }
            set { SetValue(ContentEditableProperty, value); }
        }

        private static void OnIsInEditModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var annotation = (TextAnnotation)d;
            var command = annotation.Actions.OfType<EditTextAction>().DefaultIfEmpty(null).FirstOrDefault();
            annotation.ContentEditable = false;
            command?.RaiseCanExecuteChanged();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (ContentEditable)
            {
                part_editable_text = GetTemplateChild("PART_EDITABLE_TEXT") as TextBox;
                part_editable_text.Focus();
                part_editable_text.LostFocus += Part_editable_text_LostFocus;
            }
        }

        private void Part_editable_text_LostFocus(object sender, RoutedEventArgs e)
        {
            part_editable_text.LostFocus -= Part_editable_text_LostFocus;
            ContentEditable = false;
        }

        public override async Task<FrameworkElement> GetElementToFocusOnCreating()
        {
            var layer = AdornerLayer.GetAdornerLayer(this);
            var resizingAdorner = layer.GetAdorners(this as UIElement)?.OfType<Adornment.CanvasAdorner>()?.SelectMany(_ => _.Elements.OfType<ResizingAdorner>()).DefaultIfEmpty(null).FirstOrDefault();
            while (resizingAdorner == null)
            {
                await Task.Delay(10);
                resizingAdorner = layer.GetAdorners(this as UIElement)?.OfType<Adornment.CanvasAdorner>()?.SelectMany(_ => _.Elements.OfType<ResizingAdorner>()).DefaultIfEmpty(null).FirstOrDefault();
            }
            return resizingAdorner.PART_THUMB_SE;
        }

        static TextAnnotation()
        {
            IsInEditModeProperty.OverrideMetadata(typeof(TextAnnotation), new FrameworkPropertyMetadata(OnIsInEditModePropertyChanged));
        }

        public TextAnnotation()
        {
            InitializeComponent();
            Actions.Insert(0, new EditTextAction(this));
        }

        public TextAnnotation(double x, double y, double? width, double? height, string text, Brush brush):this()
        {
            X = x;
            Y = y;
            Width = width == null ? 0 : width.Value;
            Height = height == null ? 0 : height.Value;
            Text = text;
            Brush = brush;
        }

        public TextAnnotation(double x, double y, Brush brush) : this()
        {
            X = x;
            Y = y;
            Width = 0;
            Height = 0;
            Brush = brush;

            if (IsLoaded)
                ContentEditable = true;
            else
                Loaded += (sender, e) => ContentEditable = true;
        }
    }
}
