using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using DlhSoft.Windows.Controls;
using DlhSoft.Windows.Data;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Ksmed.Presentation.Core.Controls;
using KProcess.Ksmed.Presentation.ViewModels;
using KProcess.Ksmed.Presentation.ViewModels.Interfaces;
using KProcess.Presentation.Windows;
using KProcess.Presentation.Windows.Controls;

namespace KProcess.Ksmed.Presentation.Shell.Views
{
    /// <summary>
    /// Représente la vue de l'écran d'acquisition du scénario principal.
    /// </summary>
    [ViewExport(typeof(KProcess.Ksmed.Presentation.ViewModels.Interfaces.IAnalyzeAcquireViewModel))]
    public partial class AnalyzeAcquireView : UserControl, IView
    {

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="AnalyzeAcquireView"/>.
        /// </summary>
        public AnalyzeAcquireView()
        {
            InitializeComponent();
        }

    }

}

