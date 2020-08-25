// -----------------------------------------------------------------------
// <copyright file="IViewModel.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Définit l'interface de base des viewModels
    /// </summary>
    [InheritedExport]    
    public interface IViewModel : INotifyPropertyChanged, ICleanable
    {
        /// <summary>
        /// Obtient la commande de création du ViewModel
        /// </summary>
        IExtendedCommand AddCommand { get; }

        /// <summary>
        /// Obtient la commande d'annulation du ViewModel 
        /// </summary>
        IExtendedCommand CancelCommand { get; }
        
        /// <summary>
        /// Obtient la commande de fermeture du ViewModel
        /// </summary>
        IExtendedCommand CloseCommand { get; }

        /// <summary>
        /// Obtient la commande d'édition du ViewModel
        /// </summary>
        IExtendedCommand EditCommand { get; }

        /// <summary>
        /// Obtient la commande de rafraîchissement du ViewModel 
        /// </summary>
        IExtendedCommand RefreshCommand { get; }

        /// <summary>
        /// Obtient la commande de suppression du ViewModel
        /// </summary>
        IExtendedCommand RemoveCommand { get; }

        /// <summary>
        /// Obtient la commande de suppression du ViewModel
        /// </summary>
        IExtendedCommand RemoveFolderCommand { get; }

        /// <summary>
        /// Obtient la commande de suppression du ViewModel
        /// </summary>
        IExtendedCommand RemoveProcessCommand { get; }

        /// <summary>
        /// Obtient la commande de confirmation du ViewModel
        /// </summary>
        IExtendedCommand ValidateCommand { get; }

        /// <summary>
        /// Obtient l'état du ViewModel
        /// </summary>
        ViewModelStateEnum ViewModelState { get; }

        /// <summary>
        /// Indique si le ViewModel est en état d'édition
        /// </summary>
        bool IsEditing { get; }

        /// <summary>
        /// Indique si le ViewModel est en cours de chargement
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// Indique si le ViewModel est en état instable suite à une erreur
        /// </summary>
        bool IsOnError { get; }

        /// <summary>
        /// Indique si le ViewModel est prêt à être utilisé
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Indique si le ViewModel est en cours de réactualisation d'une ou plusieurs propriétés
        /// </summary>
        bool IsRefreshing { get; }

        /// <summary>
        /// Indique si le ViewModel attend un action de l'utilisateur
        /// </summary>
        bool IsWaiting { get; }

        /// <summary>
        /// Indique si le ViewModel est en train de se fermer.
        /// </summary>
        bool IsShuttingDown { get; }

        /// <summary>
        /// Indique si le view model est actuellement occupé.
        /// </summary>
        bool IsBusy { get; }

        /// <summary>
        /// Obtient le titre
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Charge l'état du ViewModel
        /// </summary>
        Task Load();

        /// <summary>
        /// Rafraîchit l'état du ViewModel
        /// </summary>
        Task Refresh();

        /// <summary>
        /// Indique si le ViewModel peut être fermé
        /// </summary>
        /// <returns>true s'il peut être fermé, false sinon</returns>
        bool CanClose();

        /// <summary>
        /// Demande la fermeture du viewModel
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Evénement déclenché lorsque le viewModel doit être disposé
        /// </summary>
        event EventHandler Shuttingdown;
    }
}
