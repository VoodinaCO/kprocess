using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace AnnotationsLib.Annotations.Actions
{
    public abstract class AnnotationAction : DependencyObject, ICommand
    {
        public static DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(BitmapImage), typeof(AnnotationAction), new PropertyMetadata(null));

        public string Name { get; private set; }
        public AnnotationBase Owner { get; private set; }

        public BitmapImage Image
        {
            get { return (BitmapImage)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, null);
        }

        public virtual bool CanExecute(object parameter) => false;

        public virtual void Execute(object parameter)
        { }

        public override string ToString() => Name;

        protected AnnotationAction(string name, AnnotationBase owner, string uri = null)
        {
            Name = name;
            Owner = owner;
            if (!string.IsNullOrEmpty(uri))
                Image = new BitmapImage(new Uri(uri));
        }
    }
}
