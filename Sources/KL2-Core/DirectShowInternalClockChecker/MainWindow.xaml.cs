using KProcess;
using KProcess.Globalization;
using KProcess.Ksmed.Globalization;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Shell;
using KProcess.Ksmed.Presentation.Shell.Configuration.DirectShow;
using KProcess.Ksmed.Presentation.Tests;
using KProcess.Ksmed.Presentation.ViewModels;
using KProcess.Presentation.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace DirectShowInternalClockChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDecoderInfoService
    {
        private VideoInternalClockMeasure _measure;
        public MainWindow()
        {
            InitializeComponent();

            var resources = new KProcess.Ksmed.Business.Impl.AppResourceService().GetResources(CultureInfo.CurrentUICulture.Name);
            LocalizationManager.ResourceProvider = new DatabaseResourceProvider("Main", resources);


            IoC.RegisterInstance<IServiceBus>(new KProcess.Presentation.Windows.ServiceBus());
            IoC.Resolve<IServiceBus>().Register<IDecoderInfoService>(this);

            IoC.Resolve<IServiceBus>().Get<KProcess.Presentation.Windows.Controls.IDecoderInfoService>().InitializeFiltersConfiguration();

            this.Videos.Text = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonVideos), "Sample Videos");

        }

        private List<ActionPath> _actions = new List<ActionPath>();

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            _measure = new VideoInternalClockMeasure(miniPlayer, s => this.log.Text += s + Environment.NewLine);

            // Lire le xml
            var root = XDocument.Load("TestsData.xml").Root;

            foreach (var tc in root.Elements("VideoSpeedRatioCorrection"))
            {
                var fileName = System.IO.Path.GetFileName(tc.Element("File").Value);

                var start = long.Parse(tc.Element("Start").Value, CultureInfo.InvariantCulture) * TimeSpan.TicksPerMillisecond;
                var end = long.Parse(tc.Element("End").Value, CultureInfo.InvariantCulture) * TimeSpan.TicksPerMillisecond;
                var videoStart = long.Parse(tc.Element("VideoStart").Value, CultureInfo.InvariantCulture) * TimeSpan.TicksPerMillisecond;
                var videoEnd = long.Parse(tc.Element("VideoEnd").Value, CultureInfo.InvariantCulture) * TimeSpan.TicksPerMillisecond;
                var expectedMaxLinearRegression = double.Parse(tc.Element("MaxLinearRegression").Value, CultureInfo.InvariantCulture);

                _actions.Add(new ActionPath(new KAction
                {
                    Video = new Video
                    {
                        FilePath = System.IO.Path.Combine(this.Videos.Text, fileName),
                    },
                    Start = videoStart,
                    Finish = videoEnd,
                })
                {
                    Start = start,
                    Finish = end,
                });
            }

            Run(0);

        }

        private void Run(int index)
        {
            var action = _actions[index];
            this.File.Text = action.Action.Video.FilePath;
            this.Start.Text = TimeSpan.FromTicks(action.Start).ToString();
            this.End.Text = TimeSpan.FromTicks(action.Finish).ToString();
            this.VideoStart.Text = TimeSpan.FromTicks(action.VideoStart).ToString();
            this.VideoEnd.Text = TimeSpan.FromTicks(action.VideoFinish).ToString();

            _measure.Run(action);

            log.Text += "--------------------------\r\n";

            miniPlayer.ItemsSource = null;


            System.Threading.Tasks.Task.Factory.StartNew(() => System.Threading.Thread.Sleep(1000))
            .ContinueWith(t =>
            {
                if (_actions.Count > index + 1)
                {
                    Dispatcher.BeginInvoke((Action<int>)Run, index + 1);
                }
            });
        }

        public FiltersConfiguration FiltersConfiguration { get; set; }

        public bool InitializeFiltersConfiguration()
        {
            var config = ConfigurationParser.Parse();
            this.FiltersConfiguration = config;
            return config != null;
        }
    }
}
