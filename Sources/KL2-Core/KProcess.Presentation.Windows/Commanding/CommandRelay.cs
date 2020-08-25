using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Représente un relai vers une commande.
    /// </summary>
    public class CommandRelay : ICommand, INotifyPropertyChanged
    {
        #region Attributs

        private ICommand _command;

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient ou définit 
        /// </summary>
        public ICommand Command
        {
            get { return _command; }
            set { _command = value; }
        }

        #endregion

        #region INotifyPropertyChanged Membres

        /// <summary>
        /// Evénement de notification de changement de valeur d'une propriété.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Déclenche l'événement PropertyChanged.
        /// </summary>
        /// <param name="propertyName">Nom de la propriété.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region ICommand Members

        /// <summary>
        /// Indique si la commande peut être exécutée.
        /// </summary>
        /// <param name="parameter">Le paramètre fourni à la commande.</param>
        /// <returns><c>true</c> si la commande peut être exécutée.</returns>
        public bool CanExecute(object parameter)
        {
            return (_command == null) ? false : _command.CanExecute(parameter);
        }

        /// <summary>
        /// Survient lorsque des changements ont lieu et qui affectent le fait qu'une commande puisse être exécutée.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add 
            { 
                if (_command != null) 
                    _command.CanExecuteChanged += value; 
            }
            remove 
            { 
                if (_command != null)
                    _command.CanExecuteChanged -= value; 
            }
        }

        /// <summary>
        /// Exécute la commande.
        /// </summary>
        /// <param name="parameter">Le paramètre de la commande.</param>
        public void Execute(object parameter)
        {
            if (_command != null)
                _command.Execute(parameter);
        }

        #endregion
    }
}
