using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestConnectionTool
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += async (o, e) =>
            {
                MainViewModel.mainWindow = this;

                if (!MainViewModel.Instance.SQLInstancesScanned)
                    await MainViewModel.Instance.ScanSQL_Instances();
            };

            MainViewModel.Instance.dummyAddDataBase.PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.Instance.dummyAddDataBase.ServerName))
                {
                    MainViewModel.Instance.dummyAddDataBase.KL2_DataBaseVersion = null;
                    ((DelegateCommand<object>)MainViewModel.Instance.TestDataBaseConnectionCommand).RaiseCanExecuteChanged();
                }
                else if (e.PropertyName == nameof(MainViewModel.Instance.dummyAddDataBase.InstanceName))
                {
                    MainViewModel.Instance.dummyAddDataBase.KL2_DataBaseVersion = null;
                }
            };
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await MainViewModel.Instance.ScanSQL_Instances(true);
        }

        private void DatabasePasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            MainViewModel.Instance.NewInstance_Password = DatabasePasswordBox.SecurePassword;
        }
    }
}
