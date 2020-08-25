﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KProcess.Ksmed.Models;
using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{
    /// <summary>
    /// Définit le comportement du modèle de vue de l'écran de gestion des opérateurs des projets.
    /// </summary>
    [InheritedExportAsPerCall]
    public interface IAdminOperatorsViewModel : IAdminSubReferentialViewModel
    {
    }
}