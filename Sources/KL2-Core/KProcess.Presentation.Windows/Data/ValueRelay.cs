using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace KProcess.Presentation.Windows
{
    /// <summary>
    /// Représente un relai contenant une valeur.
    /// </summary>
    public class ValueRelay : INotifyPropertyChanged
    {
        #region Attributs

        private object _value;

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient ou définit la valeur.
        /// </summary>
        /// <value>La valeur.</value>
        public object Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged("Value");
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Survient lorsque la valeur d'une propriété a changé.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Est appelé lorsque la valeur d'une propriété a changé.
        /// </summary>
        /// <param name="propertyName">Le nom de la propriété.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
