﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using KProcess.Ksmed.Presentation.Core.Controls;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.Shell.Views
{
    /// <summary>
    /// Représente la vue de l'écran VUEVUEVUEVUEVUE.
    /// </summary>
    [ViewExport(typeof(KProcess.Ksmed.Presentation.ViewModels.Interfaces.IValidateSimulateViewModel))]
    public partial class ValidateSimulateView : UserControl, IView
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="ValidateSimulateView"/>.
        /// </summary>
        public ValidateSimulateView()
        {
            InitializeComponent();
        }
    }

}

