namespace AnnotationsLib.Annotations.Actions
{

    public class ThicknessAction
    {
        public const double MaxThickness = 18.0;
    }

    public class IncreaseThicknessAction : AnnotationAction
    {
        private const string imageUri = "pack://application:,,,/AnnotationsLib;component/Resources/Plus.png";

        public override bool CanExecute(object parameter)
        {
            var editableAnnotation = Owner as IThicknessEditableAnnotation;
            return Owner.IsInEditMode && editableAnnotation != null
                && 1.0 <= editableAnnotation.Thickness
                && editableAnnotation.Thickness < ThicknessAction.MaxThickness;
        }

        public override void Execute(object parameter)
        {
            var editableAnnotation = Owner as IThicknessEditableAnnotation;
            if (editableAnnotation == null) return;
            else editableAnnotation.Thickness++;
        }

        public IncreaseThicknessAction(AnnotationBase owner) : base(LocalizationExt.CurrentCulture.GetLocalizedValue("IncreaseThicknessActionLabel"), owner, imageUri)
        {
        }
    }

    public class DecreaseThicknessAction : AnnotationAction
    {
        private const string imageUri = "pack://application:,,,/AnnotationsLib;component/Resources/Minus.png";

        public override bool CanExecute(object parameter)
        {
            var editableAnnotation = Owner as IThicknessEditableAnnotation;
            return Owner.IsInEditMode && editableAnnotation != null
                && 1.0 < editableAnnotation.Thickness
                && editableAnnotation.Thickness <= ThicknessAction.MaxThickness;
        }

        public override void Execute(object parameter)
        {
            var editableAnnotation = Owner as IThicknessEditableAnnotation;
            if (editableAnnotation == null) return;
            else editableAnnotation.Thickness--;
        }

        public DecreaseThicknessAction(AnnotationBase owner) : base(LocalizationExt.CurrentCulture.GetLocalizedValue("DecreaseThicknessActionLabel"), owner, imageUri)
        {
        }
    }

    public interface IThicknessEditableAnnotation
    {
        double Thickness { get; set; }
    }
}
