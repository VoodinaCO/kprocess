using System.Windows;
using System.Windows.Markup;

namespace AnnotationsLib
{
    [ContentProperty("Items")]
    public class AdornmentCollection : DependencyCollection<DependencyObject>
    {
        // TODO - research a better way to handle this
        public DependencyCollection<DependencyObject> Items
        {
            get { return this; }
        }
    }
}
