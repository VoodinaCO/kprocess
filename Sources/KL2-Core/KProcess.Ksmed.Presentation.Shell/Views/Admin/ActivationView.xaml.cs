using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using KProcess.Globalization;
using KProcess.Ksmed.Security.Activation;

namespace KProcess.Ksmed.Presentation.Shell.Views
{
    /// <summary>
    /// Interaction logic for ActivationView.xaml
    /// </summary>
    [ViewExport(typeof(IActivationViewModel))]
    public partial class ActivationView : Window, IModalWindowView
    {
        public ActivationView()
        {
            InitializeComponent();

            this.ActivationExplanationTB.Text = 
                string.Format(
                    LocalizationManager.GetString("View_ActivationView_ActivationExplanation"), ActivationConstants.KProcessEmail);
        }
    }
}
