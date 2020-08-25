// -----------------------------------------------------------------------
// <copyright file="CommandOfT.cs" company="Tekigo">
//     Copyright (c) Tekigo. All rights reserved.
// </copyright>
// <email>contact@tekigo.com</email>
// -----------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Classe permettant de découpler les commandes avec paramètre
    /// </summary>
    public class Command<T> : IExtendedCommand
    {
        #region Attributs

        readonly Action<T> _execute;
        readonly Predicate<T> _canExecute;

        bool _isEnabled = true;

        #endregion

        #region Constructeurs

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="execute">Méthode d'exécution de la commande</param>
        public Command(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="execute">Méthode d'exécution de la commande</param>
        /// <param name="canExecute">Méthode permettant de savoir si la commande peut être exécutée</param>
        public Command(Action<T> execute, Predicate<T> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute), "Le délégué execute ne peut pas être null");
            _canExecute = canExecute;
        }

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la commande est activée.
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged();
                    Invalidate();
                }
            }
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Invalide la commande, forçant la réexécution du CanExecute.
        /// </summary>
        public void Invalidate()
        {
#if SILVERLIGHT
            DispatcherHelper.SafeInvoke(() =>
                {
                    if (CanExecuteChanged != null)
                        CanExecuteChanged(this, EventArgs.Empty);
                });
#else
            CommandManager.InvalidateRequerySuggested();
#endif
        }

        #endregion

        #region ICommand Membres

        /// <summary>
        /// Indique si la commande peut être exécutée
        /// </summary>
        /// <param name="parameter">Le paramètre de la commande.</param>
        /// <returns><c>true</c> si la commande peut être exécutée.</returns>
        public bool CanExecute(object parameter) =>
            (_isEnabled && _canExecute == null) || (_isEnabled && _canExecute((T)parameter));

#if SILVERLIGHT
        public event EventHandler CanExecuteChanged;
#else
        /// <summary>
        /// Evénement déclenché lors d'un changement du CanExecute
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                //if (_canExecute != null)
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                //if (_canExecute != null)
                CommandManager.RequerySuggested -= value;
            }
        }
#endif

        /// <summary>
        /// Exécute la commande
        /// </summary>
        /// <param name="parameter">paramètre de la commande</param>
        public void Execute(object parameter)
        {
            _execute((T)parameter);

            // Informe de son exécution
            OnCommandExecuted(parameter);
        }

        #endregion

        #region INotifyCommandExecuted Members

        /// <summary>
        /// Evénement déclenché lorsque la commande a été exécutée
        /// </summary>
        public event EventHandler<EventArgs<object>> CommandExecuted;

        /// <summary>
        /// Appelé lorsque la commande a été exécutée.
        /// </summary>
        /// <param name="parameter">Le paramètre de la commande.</param>
        protected virtual void OnCommandExecuted(object parameter) =>
            CommandExecuted?.Invoke(this, new EventArgs<object>(parameter));

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Evénement déclenché lors du changement de valeur d'une propriété
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Appelé lorsque la valeur d'une propriété a changé.
        /// </summary>
        /// <param name="propertyName">Le nom de la propriété.</param>
        protected void OnPropertyChanged([CallerMemberName]string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion
    }
}
