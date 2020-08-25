namespace AnnotationsLib.Annotations.Actions
{
    public class DeleteAction : AnnotationAction
    {
        private const string imageUri = "pack://application:,,,/AnnotationsLib;component/Resources/DeleteAction.png";

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var control = (AnnotationsControl)Owner.DataContext;
            control.RemoveAnnotation(Owner);
        }

        public DeleteAction(AnnotationBase owner) : base(LocalizationExt.CurrentCulture.GetLocalizedValue("DeleteActionLabel"), owner, imageUri)
        { }
    }
}
