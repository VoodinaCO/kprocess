using GalaSoft.MvvmLight.Threading;
using System.Windows;

namespace Kprocess.KL2.FileTransferTest
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }
    }
}
