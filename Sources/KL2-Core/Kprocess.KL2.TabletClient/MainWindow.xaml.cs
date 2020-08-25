using Kprocess.KL2.TabletClient.Behaviors;
using Kprocess.KL2.TabletClient.ViewModel;
using MahApps.Metro.Controls;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Kprocess.KL2.TabletClient
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        MainViewModel vm = Locator.GetInstance<MainViewModel>();

        public Panel RootContainer { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = vm;
            Loaded += (sender, e) =>
            {
                if (Application.Current is App myApp)
                    myApp.SplashScreen.Close(TimeSpan.FromMilliseconds(200));
                Activate();
            };
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            RootContainer = GetTemplateChild<MetroContentControl>("PART_Content").Parent as Panel;
            var behColl = Interaction.GetBehaviors(this);
            if (RootContainer != null && behColl != null)
            {
                var loadingContentBehavior = behColl.FirstOrDefault(b => b.GetType() == typeof(LoadingContentBehavior)) as LoadingContentBehavior;
                if (loadingContentBehavior?.Content != null)
                    RootContainer.Children.Add(loadingContentBehavior.Content);
            }
        }

        TControl GetTemplateChild<TControl>(string name)
            where TControl : FrameworkElement
        {
            var control = GetTemplateChild(name) as TControl;
            return control;
        }
    }
}
