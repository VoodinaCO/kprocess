namespace AnnotationsLib.Annotations.Actions
{

    public class ZoomAction
    {
        public const double MaxZoom = 4d;
    }

    public class IncreaseZoomAction : AnnotationAction
    {
        private const string imageUri = "pack://application:,,,/AnnotationsLib;component/Resources/Zoom-In.png";

        public override bool CanExecute(object parameter)
        {
            var editableAnnotation = Owner as IZoomEditableAnnotation;
            return Owner.IsInEditMode && editableAnnotation != null
                && 1.0 <= editableAnnotation.ZoomFactor
                && editableAnnotation.ZoomFactor < ZoomAction.MaxZoom;
        }

        public override void Execute(object parameter)
        {
            var editableAnnotation = Owner as IZoomEditableAnnotation;
            if (editableAnnotation == null) return;
            else editableAnnotation.ZoomFactor++;
        }

        public IncreaseZoomAction(AnnotationBase owner) : base(LocalizationExt.CurrentCulture.GetLocalizedValue("IncreaseZoomActionLabel"), owner, imageUri)
        {
        }
    }

    public class DecreaseZoomAction : AnnotationAction
    {
        private const string imageUri = "pack://application:,,,/AnnotationsLib;component/Resources/Zoom-Out.png";

        public override bool CanExecute(object parameter)
        {
            var editableAnnotation = Owner as IZoomEditableAnnotation;
            return Owner.IsInEditMode && editableAnnotation != null
                && 1.0 < editableAnnotation.ZoomFactor
                && editableAnnotation.ZoomFactor <= ThicknessAction.MaxThickness;
        }

        public override void Execute(object parameter)
        {
            var editableAnnotation = Owner as IZoomEditableAnnotation;
            if (editableAnnotation == null) return;
            else editableAnnotation.ZoomFactor--;
        }

        public DecreaseZoomAction(AnnotationBase owner) : base(LocalizationExt.CurrentCulture.GetLocalizedValue("DecreaseZoomActionLabel"), owner, imageUri)
        {
        }
    }

    public interface IZoomEditableAnnotation
    {
        double ZoomFactor { get; set; }
    }
}
