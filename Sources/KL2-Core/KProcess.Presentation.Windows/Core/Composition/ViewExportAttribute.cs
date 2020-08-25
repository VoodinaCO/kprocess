// -----------------------------------------------------------------------
// <copyright file="ViewExportAttribute.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel.Composition;
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
    /// Définit l'attribut à apposer sur les vues
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ViewExportAttribute : ExportAttribute
    {
        #region Constructeurs

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="viewModelType">type du viewModel</param>
        public ViewExportAttribute(Type viewModelType)
            : base(null, typeof(IView))
        {
            ViewModelType = viewModelType;
            LifetimeManagement = Windows.LifetimeManagement.PerCall;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="viewModelType">type du viewModel</param>
        /// <param name="themeGuid">Le GUID du thème.</param>
        public ViewExportAttribute(Type viewModelType, string themeGuid)
            : base(null, typeof(IView))
        {
            ViewModelType = viewModelType;
            LifetimeManagement = Windows.LifetimeManagement.PerCall;
            ThemeID = themeGuid;
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="viewModelType">type du viewModel</param>
        /// <param name="themeGuid">Le GUID du thème.</param>
        /// <param name="name">nom du filtre</param>
        public ViewExportAttribute(Type viewModelType, string themeGuid = null, string name = null)
            : base(name, typeof(IView))
        {
            ViewModelType = viewModelType;
            LifetimeManagement = Windows.LifetimeManagement.PerCall;
            ThemeID = themeGuid;
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient le type du viewModel
        /// </summary>
        public Type ViewModelType { get; private set; }

        /// <summary>
        /// Obtient le type de de gestion de cycle de vie de la vue
        /// </summary>
        public LifetimeManagement LifetimeManagement { get; private set; }

        /// <summary>
        /// Obtient l'id du thème.
        /// </summary>
        public string ThemeID { get; private set; }

        #endregion
    }
}
