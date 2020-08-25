using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace KProcess.KL2.SetupUI.Views
{
    public class InstallScreen : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
