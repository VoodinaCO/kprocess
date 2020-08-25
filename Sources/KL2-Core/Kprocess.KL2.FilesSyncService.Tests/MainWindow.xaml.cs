using JKang.IpcServiceFramework;
using Kprocess.KL2.IpcContracts;
using System;
using System.Windows;

namespace Kprocess.KL2.FilesSyncService.Tests
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IpcServiceClient<IFilesSyncService> client;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += (sender, args) =>
            {
                try
                {
                    client = new IpcServiceClientBuilder<IFilesSyncService>()
                        .UseNamedPipe("KL2_FilesSyncService") // or .UseTcp(IPAddress.Loopback, 45684) to invoke using TCP
                        .Build();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
        }

        async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            bool result = false;
            try
            {
                result = await client.InvokeWithDelayAsync(x => x.Ping(), TimeSpan.FromSeconds(1));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't call service.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (result)
                MessageBox.Show("Pong");
        }
    }
}
