using System.ComponentModel;
using System.Windows.Media;

namespace AnnotationsLib
{
    public static class StandardBrushes
    {
        public static BindingList<Brush> StandardColors { get; private set; } = new BindingList<Brush>()
        {
            Brushes.Black, Brushes.Gray, Brushes.Silver, Brushes.White,
            Brushes.Red, Brushes.Maroon, Brushes.Lime, Brushes.Green,
            Brushes.Yellow, Brushes.Olive, Brushes.Blue, Brushes.Navy,
            Brushes.Fuchsia, Brushes.Purple, Brushes.Aqua, Brushes.Teal
        };
    }
}
