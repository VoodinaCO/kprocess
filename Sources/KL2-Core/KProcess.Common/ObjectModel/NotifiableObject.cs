//------------------------------------------------------------------------------
//*                                    Creation                            
//*************************************************************************
//* Fichier  : NotifiableObject.cs
//* Auteur   : Tekigo
//* Creation : 
//* Role     : Classe de base implémentant INotifyPropertyChanged.
//*************************************************************************
//*                                    Modifications              
//*************************************************************************
//*     Auteur            Date            Objet de la modification        
//*************************************************************************
//*   
//------------------------------------------------------------------------------

using KProcess.Common;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace KProcess
{
    /// <summary>
    /// Classe de base implémentant INotifyPropertyChanged.
    /// </summary>
    [DataContract(IsReference = true)]
    [Serializable]
    public abstract class NotifiableObject : INotifyPropertyChanged, ICleanable
    {
        #region Attributs

        PropertyChangedManager _propertyChangedManager;
        bool _isCleaned;

        #endregion

        #region Propriétés

        /// <summary>
        /// Obtient le <see cref="PropertyChangedManager"/>
        /// </summary>
        protected PropertyChangedManager PropertyChangedManager
        {
            get
            {
                if (_propertyChangedManager == null)
                    _propertyChangedManager = new PropertyChangedManager(this);

                return _propertyChangedManager;
            }
        }

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        protected NotifiableObject()
        {
        }

        #endregion

        #region Destructeur

        /// <summary>
        /// Destructeur
        /// </summary>
        ~NotifiableObject()
        {
            Cleanup(false);
        }

        #endregion

        #region Gestion de la déserialisation

        /// <summary>
        /// Appelé lorsque l'objet est désérialisé.
        /// </summary>
        [OnDeserializing]
        void OnDeserializing(StreamingContext context) =>
            OnDeserializing();

        /// <summary>
        /// Appelé lorsque l'objet est désérialisé.
        /// </summary>
        [OnDeserialized]
        void OnDeserialized(StreamingContext context) =>
            OnDeserialized();

        /// <summary>
        /// Appelé lorsque l'objet commence à être désérialisé.
        /// </summary>
        protected virtual void OnDeserializing()
        {
        }

        /// <summary>
        /// Appelé lorsque l'objet a fini d'être désérialisé.
        /// </summary>
        protected virtual void OnDeserialized()
        {
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Evénement déclenché lors du changement de valeur d'une propriété
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { PropertyChangedManager.AddHandler(value); }
            remove { PropertyChangedManager.RemoveHandler(value); }
        }

        /// <summary>
        /// Déclenche l'événement <see cref="PropertyChanged"/>
        /// </summary>
        /// <param name="propertyName"></param>
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Vérifie que le nom de la propriété existe
            CheckPropertyName(propertyName);

            // Déclenche les callbacks
            PropertyChangedManager.NotifyPropertyChanged(propertyName);
        }

        /// <summary>
        /// Déclenche l'événement <see cref="PropertyChanged"/>
        /// </summary>
        /// <param name="property"></param>
        public virtual void OnPropertyChanged(Expression<Func<object>> property) =>
            OnPropertyChanged(property.GetMemberPath());

        /// <summary>
        /// Vérifie que le nom de la propriété existe bien
        /// </summary>
        /// <param name="propertyName">nom de la propriété</param>
        /// <remarks>n'existe qu'en debug</remarks>
        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerStepThrough()]
        void CheckPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                throw new Exception($"Unknown property name '{propertyName}'");
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Créé un abonnement faible sur le changement de valeur d'une propriété
        /// </summary>
        /// <typeparam name="T">type d'objet sur lequel on veut s'abonner qui sera passé au callback</typeparam>
        /// <param name="propertyName">nom de la propriété à surveiller</param>
        /// <param name="callback">callback à appeler lors du déclenchement de l'événement PropertyChanged</param>
        /// <returns>un objet à disposer lorsqu'on souhaite se désabonner</returns>
        public IDisposable SubscribeToPropertyChanged<T>(string propertyName, Action<T> callback)
            where T : INotifyPropertyChanged =>
            PropertyChangedManager.SubscribeToPropertyChanged(propertyName, callback);

        /// <summary>
        /// Créé un abonnement faible sur le changement de valeur d'une propriété
        /// </summary>
        /// <param name="propertyName">nom de la propriété à surveiller</param>
        /// <param name="callback">callback à appeler lors du déclenchement de l'événement PropertyChanged</param>
        /// <returns>un objet à disposer lorsqu'on souhaite se désabonner</returns>
        public IDisposable SubscribeToPropertyChanged(string propertyName, Action callback) =>
            PropertyChangedManager.SubscribeToPropertyChanged<NotifiableObject>(propertyName, n => callback());

        /// <summary>
        /// Supprime un abonnement faible sur le changement de valeur d'une propriété
        /// </summary>
        /// <typeparam name="T">type d'objet sur lequel on veut se désabonner</typeparam>
        /// <param name="propertyName">nom de la propriété à ne plus surveiller</param>
        /// <param name="callback">callback appelé lors du déclenchement de l'événement PropertyChanged</param>
        public void UnsubscribeToPropertyChanged<T>(string propertyName, Action<T> callback)
            where T : INotifyPropertyChanged =>
            PropertyChangedManager.UnsubscribeToPropertyChanged(propertyName, callback);

        /// <summary>
        /// Supprime un abonnement faible sur le changement de valeur d'une propriété
        /// </summary>
        /// <param name="propertyName">nom de la propriété à ne plus surveiller</param>
        /// <param name="callback">callback appelé lors du déclenchement de l'événement PropertyChanged</param>
        public void UnsubscribeToPropertyChanged(string propertyName, Action callback) =>
            PropertyChangedManager.UnsubscribeToPropertyChanged<NotifiableObject>(propertyName, n => callback());

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Nettoie les resources
        /// </summary>
        public void Cleanup()
        {
            Cleanup(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Nettoie les resources
        /// </summary>
        /// <param name="cleaning">Indique si le nettoyage est en cours.</param>
        public void Cleanup(bool cleaning)
        {
            if (!_isCleaned)
            {
                _isCleaned = true;

                if (cleaning)
                {
                    PropertyChangedManager.Dispose();
                    OnCleanup();
                }
            }
        }

        /// <summary>
        /// Appelé afin de nettoyer les ressources.
        /// </summary>
        protected virtual void OnCleanup()
        {
        }

        #endregion
    }
}
