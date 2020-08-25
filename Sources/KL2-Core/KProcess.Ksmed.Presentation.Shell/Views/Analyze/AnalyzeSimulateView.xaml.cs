using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using System.Windows.Threading;
using DlhSoft.Windows.Controls;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Core.Controls;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Shell.Views
{
    /// <summary>
    /// Représente la vue de l'écran de simulation dans la phase de construction.
    /// </summary>
    [ViewExport(typeof(KProcess.Ksmed.Presentation.ViewModels.Interfaces.IAnalyzeSimulateViewModel))]
    public partial class AnalyzeSimulateView : UserControl, IView
    {

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="AnalyzeSimulateView"/>.
        /// </summary>
        public AnalyzeSimulateView()
        {
            InitializeComponent();
        }

    }

}

