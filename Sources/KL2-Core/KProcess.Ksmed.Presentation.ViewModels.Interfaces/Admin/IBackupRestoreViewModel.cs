﻿using KProcess.Ksmed.Presentation.Core;
using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KProcess.Ksmed.Presentation.ViewModels.Interfaces
{
    /// <summary>
    /// Définit le comportement du modèle de vue de l'écran de gestion des backups restores.
    /// </summary>
    [InheritedExportAsPerCall]
    public interface IBackupRestoreViewModel : IViewModel, IFrameContentViewModel
    {
    }
}
