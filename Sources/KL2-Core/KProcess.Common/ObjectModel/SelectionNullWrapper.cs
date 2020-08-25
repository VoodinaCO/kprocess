using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace KProcess
{
    /// <summary>
    /// Représente un conteneur autour d'un tableau permettant de gérer la sélection des éléments null.
    /// </summary>
    /// <typeparam name="TContainer">Le type du conteneur qui contient le "SelectedItem".</typeparam>
    /// <typeparam name="TData">Le type de la donnée à sélectionner.</typeparam>
    public class SelectionNullWrapper<TContainer, TData> : NotifiableObject, IDisposable
        where TData : class
        where TContainer : class
    {
        private Action<TContainer, TData> _containerSetter;
        private Func<TContainer, TData> _containerGetter;
        private TContainer _container;
        private string _containerPropertyName;

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="SelectionNullWrapper&lt;TContainer, TData&gt;"/>.
        /// </summary>
        /// <param name="container">Le conteneur.</param>
        /// <param name="containerPropertyGetter">Un délégué qui récupère la propriété de type "SelectedItem".</param>
        /// <param name="items">Tous les objets valorisés.</param>
        /// <param name="nullItem">L'objet qui représente une valeur nulle.</param>
        /// <param name="nullPosition">La position de la valeur nulle par rapport aux autres éléments.</param>
        public SelectionNullWrapper(TContainer container, Expression<Func<TContainer, TData>> containerPropertyGetter, IEnumerable<TData> items, TData nullItem, NullItemPosition nullPosition)
        {
            _container = container;

            // Création des wrappers.
            var allItems = new NullWrapperItem<TData>[items.Count() + 1];

            int i = 0;

            if (nullPosition == NullItemPosition.Top)
                allItems[i] = new NullWrapperItem<TData>() { IsNull = true, Value = nullItem };

            foreach (var item in items)
            {
                allItems[i] = new NullWrapperItem<TData>() { Value = item };
                i++;
            }

            if (nullPosition == NullItemPosition.Bottom)
                allItems[i] = new NullWrapperItem<TData>() { IsNull = true, Value = nullItem };

            this.Items = allItems;


            _containerPropertyName = ReflectionHelper.GetExpressionPropertyName(containerPropertyGetter);

            _containerSetter = CreateSetter();
            _containerGetter = containerPropertyGetter.Compile();

            this.Container = container;
        }

        /// <summary>
        /// Crée une lambda capable de définir la valeur de l'élément sélectionné.
        /// </summary>
        /// <returns>La lambda.</returns>
        private Action<TContainer, TData> CreateSetter()
        {
            var containerParameter = Expression.Parameter(typeof(TContainer), "this");
            var valueParameter = Expression.Parameter(typeof(TData), "other");

            var body = Expression.Assign(
                Expression.Property(containerParameter, _containerPropertyName), valueParameter
                );

            var lambda = Expression.Lambda<Action<TContainer, TData>>(body, containerParameter, valueParameter);
            return lambda.Compile();
        }

        /// <summary>
        /// Souscrit à PropertyChanged sur le conteneur afin de pouvoir gérer le changement de valeur sélectionnée fait en dehors.
        /// </summary>
        private void SubscribeToPropertyChanged()
        {
            if (_container != null && _container is INotifyPropertyChanged)
                ((INotifyPropertyChanged)_container).PropertyChanged += new PropertyChangedEventHandler(OnContainerPropertyChanged);
        }

        /// <summary>
        /// A lieu lorsque l'évènement PropertyChanged est levé sur le conteneur.
        /// </summary>
        /// <param name="sender">La source de l'évènement.</param>
        /// <param name="e">Les <see cref="System.ComponentModel.PropertyChangedEventArgs"/> contenant les données de l'évènement.</param>
        private void OnContainerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == _containerPropertyName)
                UpdateSelectedItemFromTarget();
        }

        /// <summary>
        /// Met à jour la sélection dans le wrapper depuis la cible (le conteneur).
        /// </summary>
        private void UpdateSelectedItemFromTarget()
        {
            if (_container == null)
                this.SelectedItem = null;
            else
                this.SelectedItem = this.Items.FirstOrDefault(i => 
                    i.IsNull && _containerGetter(_container) == null ||
                    i.Value == _containerGetter(_container));
        }

        private NullWrapperItem<TData> _selectedItem;
        /// <summary>
        /// Obtient ou définit l'élément sélectionné.
        /// </summary>
        public NullWrapperItem<TData> SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnPropertyChanged("SelectedItem");

                    if (_container != null)
                    {
                        if (_selectedItem != null && !_selectedItem.IsNull)
                            _containerSetter(_container, _selectedItem.Value);
                        else
                            _containerSetter(_container, (TData)null);
                    }
                }
            }
        }

        private NullWrapperItem<TData>[] _items;
        /// <summary>
        /// Obtient la collection des éléments.
        /// </summary>
        public NullWrapperItem<TData>[] Items
        {
            get { return _items; }
            private set
            {
                if (_items != value)
                {
                    _items = value;
                    OnPropertyChanged("Items");
                }
            }
        }

        /// <summary>
        /// Obtient ou définit le conteneur.
        /// </summary>
        public TContainer Container
        {
            get { return _container; }
            set
            {
                if (_container != value)
                {
                    _container = value;
                    OnPropertyChanged("Container");
                    this.SubscribeToPropertyChanged();
                    this.UpdateSelectedItemFromTarget();
                }
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Supprimer les ressources managées.
        /// </summary>
        public void Dispose()
        {
            if (_container != null && _container is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)_container).PropertyChanged -= new PropertyChangedEventHandler(OnContainerPropertyChanged);
            }
        }

        #endregion
    }

    /// <summary>
    /// Représente un élément wrappé.
    /// </summary>
    /// <typeparam name="TData">Le type de la donnée.</typeparam>
    public class NullWrapperItem<TData>
    {
        /// <summary>
        /// Obtient ou définit la valeur associée
        /// </summary>
        public TData Value { get; set; }

        /// <summary>
        /// Obtient ou définit une valeur indiquant si la valeur associée représente un élément null.
        /// </summary>
        public bool IsNull { get; set; }
    }

    /// <summary>
    /// La position de l'élément null.
    /// </summary>
    public enum NullItemPosition
    {
        /// <summary>
        /// Au dessus des autres éléments.
        /// </summary>
        Top,

        /// <summary>
        /// En dessous des autres éléments.
        /// </summary>
        Bottom
    }
}
