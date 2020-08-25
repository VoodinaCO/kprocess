using System.Windows;
using System.Windows.Controls;

namespace Kprocess.PackIconKprocess
{
    /// <summary>
    /// Logique d'interaction pour PackIconCountriesFlags.xaml
    /// </summary>
    public partial class PackIconCountriesFlags : UserControl
    {
        public static readonly DependencyProperty CountryProperty = DependencyProperty.Register(nameof(Country), typeof(PackIconCountriesFlagsKind), typeof(PackIconCountriesFlags), new PropertyMetadata(PackIconCountriesFlagsKind.France));

        public PackIconCountriesFlagsKind Country
        {
            get => (PackIconCountriesFlagsKind)GetValue(CountryProperty);
            set => SetValue(CountryProperty, value);
        }

        public PackIconCountriesFlags()
        {
            InitializeComponent();
        }
    }
}
