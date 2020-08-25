using System.Windows.Media;

namespace AnnotationsLib.Annotations.Actions
{
    public class ChangeBrushAction : AnnotationAction
    {
        private const string imageUri = "pack://application:,,,/AnnotationsLib;component/Resources/Color.png";

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var brush = parameter as Brush;
            if (Owner != null && brush != null)
                Owner.Brush = brush;
        }

        public ChangeBrushAction(AnnotationBase owner) : base(LocalizationExt.CurrentCulture.GetLocalizedValue("ChangeBrushActionLabel"), owner, imageUri)
        { }
    }
}
