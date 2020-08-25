// -----------------------------------------------------------------------
// <copyright file="ControllerBase.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Définit l'implémentation de base d'un contrôleur
    /// </summary>    
    public abstract class ControllerBase : IController
    {
        #region IController Members

        /// <summary>
        /// Démarre le contrôleur
        /// </summary>
        public Task Start()
        {
            return OnStart();
        }

        /// <summary>
        /// Arrête le contrôleur
        /// </summary>
        public async Task Stop() =>
            await OnStop();

        #endregion

        #region Propriétés protégées

        /// <summary>
        /// Obtient le bus d'événements de l'application
        /// </summary>
        [Import]
        protected IEventBus EventBus { get; set; }

        /// <summary>
        /// Obtient le bus de services de l'application
        /// </summary>
        [Import]
        protected IServiceBus ServiceBus { get; set; }

        /// <summary>
        /// Obtient la fabrique de gestion de l'experience utilisateur
        /// </summary>
        [Import]
        protected IUXFactory UXFactory { get; set; }

        /// <summary>
        /// Obtient la fabrique de gestion des interfaces de dialogue
        /// </summary>
        [Import]
        protected IDialogFactory DialogFactory { get; set; }

        [Import]
        protected ISignalRFactory SignalRFactory { get; set; }

        /// <summary>
        /// Méthode invoquée lors du démarrage du contrôleur
        /// </summary>
        protected virtual async Task OnStart() =>
            await OnLoaded();

        /// <summary>
        /// Méthode invoquée lorsque le contrôleur est chargé
        /// </summary>
        protected virtual Task OnLoaded() =>
            Task.CompletedTask;

        /// <summary>
        /// Méthode invoquée lors de l'arrêt du contrôleur
        /// </summary>
        protected virtual Task OnStop() =>
            Task.CompletedTask;

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Evénement notifiant que la valeur d'une propriété a changé
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Déclencheur de l'événement PropertyChanged
        /// </summary>
        /// <param name="propertyName">nom de la propriété qui a changé</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}