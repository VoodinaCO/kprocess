// -----------------------------------------------------------------------
// <copyright file="IController.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System.ComponentModel;
using System.Threading.Tasks;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Définit l'interface de base d'un contrôleur
    /// </summary>
    public interface IController : INotifyPropertyChanged
    {
        /// <summary>
        /// Démarre le contrôleur
        /// </summary>
        Task Start();

        /// <summary>
        /// Arrête le contrôleur
        /// </summary>
        Task Stop();
    }
}