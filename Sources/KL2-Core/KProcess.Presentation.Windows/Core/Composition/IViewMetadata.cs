// -----------------------------------------------------------------------
// <copyright file="IViewMetadata.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Définit l'interface fournissant les métadonnées sur une vue
    /// </summary>
    public interface IViewMetadata : IExport
    {
        /// <summary>
        /// Obtient le type de l'interface du viewModel utilisé par la vue
        /// </summary>
        Type ViewModelType { get; }

        /// <summary>
        /// Obtient l'id du thème (GUID).
        /// </summary>
        string ThemeID { get; }
    }
}
