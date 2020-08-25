using KProcess.Ksmed.Models;
using KProcess.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace KProcess.Ksmed.Presentation.Core
{
    /// <summary>
    /// Représente un ViewModel de base pour l'application Ksmed.
    /// </summary>
    public abstract class KsmedViewModelBase : CompositeViewModelBase, IKsmedViewModelBase
    {

        /// <summary>
        /// Affiche le spinner.
        /// </summary>
        /// <param name="message">Le message à afficher.</param>
        protected void ShowSpinner(string message = null)
        {
            if (!DesignMode.IsInDesignMode)
                ServiceBus.Get<ISpinnerService>().ShowIncrement(message);
        }

        /// <summary>
        /// Modifie le message du spinner.
        /// </summary>
        /// <param name="message">Le message à afficher.</param>
        protected void SetMessageSpinner(string message = null)
        {
            if (!DesignMode.IsInDesignMode)
                ServiceBus.Get<ISpinnerService>().SetMessage(message);
        }

        /// <summary>
        /// Cache le spinner.
        /// </summary>
        protected void HideSpinner()
        {
            if (!DesignMode.IsInDesignMode)
                ServiceBus.Get<ISpinnerService>().HideDecrement();

            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Gère l'affichage d'une erreur
        /// </summary>
        /// <param name="ex">exception levée</param>
        protected override void OnError(Exception ex)
        {
            HideSpinner();
            base.OnError(ex);
        }

        private List<IObjectWithChangeTracker> _toUnregister;

        /// <summary>
        /// Appelé lorsque l'état de l'entité courante (spécifiée via RegisterToStateChanged) a changé.
        /// </summary>
        /// <param name="newState">le nouvel état.</param>
        protected virtual void OnEntityStateChanged(ObjectState newState)
        {
        }

        /// <summary>
        /// Désenregistre l'élément au changement d'état.
        /// </summary>
        /// <param name="item">L'élément à désabonner.</param>
        protected void UnregisterToStateChanged(IObjectWithChangeTracker item)
        {
            if (item != null)
            {
                if (_toUnregister == null)
                    _toUnregister = new List<IObjectWithChangeTracker>();

                item.ChangeTracker.ObjectStateChanging -= new EventHandler<ObjectStateChangingEventArgs>(OnEntityObjectStateChanging);
                _toUnregister.Remove(item);
            }
        }

        /// <summary>
        /// Enregistre l'élément au changement d'état.
        /// </summary>
        /// <param name="item">L'élément à abonner.</param>
        protected void RegisterToStateChanged(IObjectWithChangeTracker item)
        {
            if (item != null)
            {
                if (_toUnregister == null)
                    _toUnregister = new List<IObjectWithChangeTracker>();

                item.ChangeTracker.ObjectStateChanging += new EventHandler<ObjectStateChangingEventArgs>(OnEntityObjectStateChanging);
                if (!_toUnregister.Contains(item))
                    _toUnregister.Add(item);
            }
        }

        /// <summary>
        /// Enregistre l'élément au changement d'état.
        /// </summary>
        /// <param name="previousItem">L'élément précédent à désabonner.</param>
        /// <param name="newItem">Le nouvel élément à abonner.</param>
        protected void RegisterToStateChanged(IObjectWithChangeTracker previousItem, IObjectWithChangeTracker newItem)
        {
            UnregisterToStateChanged(previousItem);
            RegisterToStateChanged(newItem);
        }

        /// <summary>
        /// Appelé lorsque l'état de l'entité a changé.
        /// </summary>
        /// <param name="sender">La source.</param>
        /// <param name="e">Les <see cref="KProcess.Ksmed.Models.ObjectStateChangingEventArgs"/> contenant les données de l'évènement.</param>
        private void OnEntityObjectStateChanging(object sender, ObjectStateChangingEventArgs e) =>
            OnEntityStateChanged(e.NewState);

        /// <summary>
        /// Appelé afin de nettoyer les ressources.
        /// </summary>
        protected override void OnCleanup()
        {
            base.OnCleanup();

            if (_toUnregister != null)
                foreach (var entity in _toUnregister.ToArray())
                {
                    entity.ChangeTracker.ObjectStateChanging -= new EventHandler<ObjectStateChangingEventArgs>(OnEntityObjectStateChanging);
                    OnItemUnregisteredToStateChangedOnCleanup(entity);
                }
        }

        /// <summary>
        /// Appelé lorsqu'un élément est désabonné du changement d'état lors du Cleanup du VM.
        /// </summary>
        /// <param name="item">L'élément.</param>
        protected virtual void OnItemUnregisteredToStateChangedOnCleanup(IObjectWithChangeTracker item)
        {
        }

        private bool? _canCurrentUserWrite;
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'utilisateur courant peut écrire sur la fiche.
        /// </summary>
        public bool CanCurrentUserWrite
        {
            get { return _canCurrentUserWrite.GetValueOrDefault(); }
            set
            {
                if (_canCurrentUserWrite.HasValue)
                    throw new InvalidOperationException("CanCurrentUserWrite ne peut être définit qu'une seule fois");

                _canCurrentUserWrite = value;
            }
        }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la fiche est en lecture seul pour l'utilisateur courant.
        /// </summary>
        public bool IsReadOnlyForCurrentUser =>
            !CanCurrentUserWrite;

    }
}