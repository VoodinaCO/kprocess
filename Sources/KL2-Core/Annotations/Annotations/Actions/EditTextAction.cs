namespace AnnotationsLib.Annotations.Actions
{
    public class EditTextAction : AnnotationAction
    {
        private const string imageUri = "pack://application:,,,/AnnotationsLib;component/Resources/Text-Editor.png";

        public override bool CanExecute(object parameter)
        {
            var editableAnnotation = Owner as IContentEditableAnnotation;
            return Owner.IsInEditMode && editableAnnotation != null;
        }

        public override void Execute(object parameter)
        {
            var editableAnnotation = Owner as IContentEditableAnnotation;
            if (editableAnnotation == null) return;
            else editableAnnotation.ContentEditable = true;
        }

        public EditTextAction(AnnotationBase owner) : base(LocalizationExt.CurrentCulture.GetLocalizedValue("EditTextActionLabel"), owner, imageUri)
        {
        }
    }

    public interface IContentEditableAnnotation
    {
        bool ContentEditable { get; set; }
    }
}
