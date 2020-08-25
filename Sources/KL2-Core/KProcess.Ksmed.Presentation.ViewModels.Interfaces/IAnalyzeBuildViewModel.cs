using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows;
using DlhSoft.Windows.Controls;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{
    /// <summary>
    /// Définit le comportement du modèle de vue de l'écran de construction du scénario initial.
    /// </summary>
    [InheritedExportAsPerCall]
    public interface IAnalyzeBuildViewModel : IBuildViewModel
    {
    }
}
