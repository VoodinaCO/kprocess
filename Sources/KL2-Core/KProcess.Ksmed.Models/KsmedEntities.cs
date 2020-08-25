
//------------------------------------------------------------------------------
// <automatiquement-généré>
//     Ce code a été généré depuis une template.
//
//     Les changements effectués directement sur ce fichier risquent d'être
//     perdus à la prochaine génération.
// </utomatiquement-généré>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using KProcess.Business;
using System.Linq;
using KProcess.Ksmed.Models.Validation;
using System.Web.Script.Serialization;

namespace KProcess.Ksmed.Models
{
    // Helper class that captures most of the change tracking work that needs to be done
    // for self tracking entities.
    [DataContract(IsReference = true)]
    [Serializable]
    public class ObjectChangeTracker
    {
        #region  Fields
    
        private bool _isDeserializing;
        private ObjectState _objectState = ObjectState.Added;
        private bool _changeTrackingEnabled;
        private OriginalValuesDictionary _originalValues;
        private ModifiedValuesDictionary _modifiedValues;
        private ExtendedPropertiesDictionary _extendedProperties;
        private ObjectsAddedToCollectionProperties _objectsAddedToCollections = new ObjectsAddedToCollectionProperties();
        private ObjectsRemovedFromCollectionProperties _objectsRemovedFromCollections = new ObjectsRemovedFromCollectionProperties();
    
        #endregion
    
        #region Events
    
        public event EventHandler<ObjectStateChangingEventArgs> ObjectStateChanging;
    
        #endregion
    
        protected virtual void OnObjectStateChanging(ObjectState newState)
        {
            if (ObjectStateChanging != null)
            {
                ObjectStateChanging(this, new ObjectStateChangingEventArgs(){ NewState = newState });
            }
        }
    
        [DataMember]
        public ObjectState State
        {
            get { return _objectState; }
            set
            {
                if (_isDeserializing || _changeTrackingEnabled)
                {
                    OnObjectStateChanging(value);
                    _objectState = value;
                }
            }
        }
    
        public bool ChangeTrackingEnabled
        {
            get { return _changeTrackingEnabled; }
            set { _changeTrackingEnabled = value; }
        }
    
        // Returns the removed objects to collection valued properties that were changed.
        [DataMember]
        public ObjectsRemovedFromCollectionProperties ObjectsRemovedFromCollectionProperties
        {
            get
            {
                if (_objectsRemovedFromCollections == null)
                {
                    _objectsRemovedFromCollections = new ObjectsRemovedFromCollectionProperties();
                }
                return _objectsRemovedFromCollections;
            }
        }
    
        // Returns the original values for properties that were changed.
        [DataMember]
        public OriginalValuesDictionary OriginalValues
        {
            get
            {
                if (_originalValues == null)
                {
                    _originalValues = new OriginalValuesDictionary();
                }
                return _originalValues;
            }
        }
    
        // Returns the modified values for properties that were changed.
        [DataMember]
        public ModifiedValuesDictionary ModifiedValues
        {
            get
            {
                if (_modifiedValues == null)
                {
                    _modifiedValues = new ModifiedValuesDictionary();
                }
                return _modifiedValues;
            }
        }
    
        // Returns the extended property values.
        // This includes key values for independent associations that are needed for the
        // concurrency model in the Entity Framework
        [DataMember]
        public ExtendedPropertiesDictionary ExtendedProperties
        {
            get
            {
                if (_extendedProperties == null)
                {
                    _extendedProperties = new ExtendedPropertiesDictionary();
                }
                return _extendedProperties;
            }
        }
    
        // Returns the added objects to collection valued properties that were changed.
        [DataMember]
        public ObjectsAddedToCollectionProperties ObjectsAddedToCollectionProperties
        {
            get
            {
                if (_objectsAddedToCollections == null)
                {
                    _objectsAddedToCollections = new ObjectsAddedToCollectionProperties();
                }
                return _objectsAddedToCollections;
            }
        }
    
        #region MethodsForChangeTrackingOnClient
    
        [OnDeserializing]
        private void OnDeserializingMethod(StreamingContext context)
        {
            _isDeserializing = true;
        }
    
        [OnDeserialized]
        private void OnDeserializedMethod(StreamingContext context)
        {
            _isDeserializing = false;
        }
    
        // Resets the ObjectChangeTracker to the Unchanged state and
        // clears the original values as well as the record of changes
        // to collection properties
        public void AcceptChanges()
        {
            OnObjectStateChanging(ObjectState.Unchanged);
            OriginalValues.Clear();
    		ModifiedValues.Clear();
            ObjectsAddedToCollectionProperties.Clear();
            ObjectsRemovedFromCollectionProperties.Clear();
            ChangeTrackingEnabled = true;
            _objectState = ObjectState.Unchanged;
        }
    
        // Resets the ObjectChangeTracker to the Unchanged state and
        // sets the actual values to be the original values as well as the record of changes
        // to collection properties
        public void CancelChanges(IObjectWithChangeTracker entity)
        {
    		if (this.State == ObjectState.Added)
    			return;
    			
            OnObjectStateChanging(ObjectState.Unchanged);
            ChangeTrackingEnabled = false;
    
    		ModifiedValues.Clear();
    
    		var originalValues = OriginalValues.ToArray();
            foreach (var kvp in originalValues)
            {
                string propertyName = kvp.Key;
                object propertyValue = kvp.Value;
    
                entity.SetPropertyValue(propertyName, propertyValue);
            }
    
    		var objectsAddedToCollectionProperties = ObjectsAddedToCollectionProperties.ToArray();
            foreach (var kvp in objectsAddedToCollectionProperties)
            {
                var propertyName = kvp.Key;
                var items = kvp.Value;
    
                foreach (var item in items.ToArray())
    				entity.RemoveItemFromCollection(propertyName, item);
            }
    		ObjectsAddedToCollectionProperties.Clear();
    
    		var objectsRemovedFromCollectionProperties = ObjectsRemovedFromCollectionProperties.ToArray();
            foreach (var kvp in objectsRemovedFromCollectionProperties)
            {
                var propertyName = kvp.Key;
                var items = kvp.Value;
    
                foreach (var item in items.ToArray())
    				entity.AddItemToCollection(propertyName, item);
            }
            ObjectsRemovedFromCollectionProperties.Clear();
    		
            ChangeTrackingEnabled = true;
            _objectState = ObjectState.Unchanged;
        }
    
        // Captures the original value for a property that is changing.
        internal void RecordValue(string propertyName, object oldValue, object newValue)
        {
            if (_changeTrackingEnabled && _objectState != ObjectState.Added)
            {
    			if (OriginalValues.ContainsKey(propertyName) &&
    				OriginalValues[propertyName] == newValue)
    			{
    				// On repasse à l'état initial
    				OriginalValues.Remove(propertyName);
    				ModifiedValues.Remove(propertyName);
    			}
    			else
    			{
    				if (!OriginalValues.ContainsKey(propertyName))
    				{
    					OriginalValues[propertyName] = oldValue;
    				}
    				ModifiedValues[propertyName] = newValue;
    			}
            }
        }
    
        // Records an addition to collection valued properties on SelfTracking Entities.
        internal void RecordAdditionToCollectionProperties(string propertyName, object value)
        {
            if (_changeTrackingEnabled)
            {
                // Add the entity back after deleting it, we should do nothing here then
                if (ObjectsRemovedFromCollectionProperties.ContainsKey(propertyName)
                    && ObjectsRemovedFromCollectionProperties[propertyName].Contains(value))
                {
                    ObjectsRemovedFromCollectionProperties[propertyName].Remove(value);
                    if (ObjectsRemovedFromCollectionProperties[propertyName].Count == 0)
                    {
                        ObjectsRemovedFromCollectionProperties.Remove(propertyName);
                    }
                    return;
                }
    
                if (!ObjectsAddedToCollectionProperties.ContainsKey(propertyName))
                {
                    ObjectsAddedToCollectionProperties[propertyName] = new ObjectList();
                    ObjectsAddedToCollectionProperties[propertyName].Add(value);
                }
                else
                {
                    ObjectsAddedToCollectionProperties[propertyName].Add(value);
                }
            }
        }
    
        // Records a removal to collection valued properties on SelfTracking Entities.
        internal void RecordRemovalFromCollectionProperties(string propertyName, object value)
        {
            if (_changeTrackingEnabled)
            {
                // Delete the entity back after adding it, we should do nothing here then
                if (ObjectsAddedToCollectionProperties.ContainsKey(propertyName)
                    && ObjectsAddedToCollectionProperties[propertyName].Contains(value))
                {
                    ObjectsAddedToCollectionProperties[propertyName].Remove(value);
                    if (ObjectsAddedToCollectionProperties[propertyName].Count == 0)
                    {
                        ObjectsAddedToCollectionProperties.Remove(propertyName);
                    }
                    return;
                }
    
                if (!ObjectsRemovedFromCollectionProperties.ContainsKey(propertyName))
                {
                    ObjectsRemovedFromCollectionProperties[propertyName] = new ObjectList();
                    ObjectsRemovedFromCollectionProperties[propertyName].Add(value);
                }
                else
                {
                    if (!ObjectsRemovedFromCollectionProperties[propertyName].Contains(value))
                    {
                        ObjectsRemovedFromCollectionProperties[propertyName].Add(value);
                    }
                }
            }
        }
    	
    	/// <summary>
        /// Remet l'état de l'entité à zéro.
        /// Supprime toutes les valeurs et désactive la mise à jour.
        /// </summary>
        public void Reset(IObjectWithChangeTracker entity)
        {
            var values = entity.GetCurrentValues();
    		
            foreach (var kvp in values)
    		{
                string propertyName = kvp.Key;
                object value = kvp.Value;
    
    			if (value != null)
    			{
    	            var type = value.GetType();
    	            var defaultValue = type.IsValueType ? Activator.CreateInstance(type) : null;
    
    	            entity.SetPropertyValue(propertyName, defaultValue);
    			}
    		}
    		entity.AcceptChanges();
    		entity.StopTracking();
        }
    	
        #endregion
    }
    
    #region EnumForObjectState
    [Flags]
    public enum ObjectState
    {
        Unchanged = 0x1,
        Added = 0x2,
        Modified = 0x4,
        Deleted = 0x8
    }
    #endregion
    
    [Serializable]
    [CollectionDataContract (Name = "ObjectsAddedToCollectionProperties",
        ItemName = "AddedObjectsForProperty", KeyName = "CollectionPropertyName", ValueName = "AddedObjects", Namespace = ModelsConstants.DataContractNamespace)]
    public class ObjectsAddedToCollectionProperties : Dictionary<string, ObjectList>
    {
        public ObjectsAddedToCollectionProperties() {}
        protected ObjectsAddedToCollectionProperties(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
    
    [Serializable]
    [CollectionDataContract (Name = "ObjectsRemovedFromCollectionProperties",
        ItemName = "DeletedObjectsForProperty", KeyName = "CollectionPropertyName",ValueName = "DeletedObjects", Namespace = ModelsConstants.DataContractNamespace)]
    public class ObjectsRemovedFromCollectionProperties : Dictionary<string, ObjectList>
    {
        public ObjectsRemovedFromCollectionProperties() {}
        protected ObjectsRemovedFromCollectionProperties(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
    
    [Serializable]
    [CollectionDataContract(Name = "OriginalValuesDictionary",
        ItemName = "OriginalValues", KeyName = "Name", ValueName = "OriginalValue", Namespace = ModelsConstants.DataContractNamespace)]
    public class OriginalValuesDictionary : Dictionary<string, Object>
    {
        public OriginalValuesDictionary() {}
        protected OriginalValuesDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
    
    [Serializable]
    [CollectionDataContract(Name = "ModifiedValuesDictionary",
        ItemName = "ModifiedValues", KeyName = "Name", ValueName = "ModifiedValue", Namespace = ModelsConstants.DataContractNamespace)]
    public class ModifiedValuesDictionary : Dictionary<string, Object>
    {
        public ModifiedValuesDictionary() {}
        protected ModifiedValuesDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
    
    [Serializable]
    [CollectionDataContract(Name = "ExtendedPropertiesDictionary",
        ItemName = "ExtendedProperties", KeyName = "Name", ValueName = "ExtendedProperty", Namespace = ModelsConstants.DataContractNamespace)]
    public class ExtendedPropertiesDictionary : Dictionary<string, Object>
    {
        public ExtendedPropertiesDictionary() {}
        protected ExtendedPropertiesDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
    
    [CollectionDataContract(ItemName = "ObjectValue")]
    public class ObjectList : List<object> { }
    // The interface is implemented by the self tracking entities that EF will generate.
    // We will have an Adapter that converts this interface to the interface that the EF expects.
    // The Adapter will live on the server side.
    public interface IObjectWithChangeTracker
    {
        // Has all the change tracking information for the subgraph of a given object.
        ObjectChangeTracker ChangeTracker { get; set; }
    
    	/// <summary>
    	/// Assigne une valeur à la propriété spécifiée.
    	/// </summary>
    	/// <param name="propertyName">Le nom de la propriété.</param>
    	/// <param name="value">La valeur</param>
    	void SetPropertyValue(string propertyName, object value);
    
    	/// <summary>
    	/// Ajoute un élément à la collection spécifiée.
    	/// </summary>
    	/// <param name="propertyName">Le nom de la propriété.</param>
    	/// <param name="value">La valeur</param>
    	void AddItemToCollection(string propertyName, object value);
    
    	/// <summary>
    	/// Ajoute un élément à la collection spécifiée.
    	/// </summary>
    	/// <param name="propertyName">Le nom de la propriété.</param>
    	/// <param name="value">La valeur</param>
    	void RemoveItemFromCollection(string propertyName, object value);
    	
        /// <summary>
        /// Obtient un dump des valeurs actuelles des propriétés.
        /// </summary>
        /// <returns>Un dump des valeurs actuelles des propriétés.</returns>
    	IDictionary<string,object> GetCurrentValues();
    
        /// <summary>
    	/// Obtient un dump des valeurs actuelles des propriétés trackables.
    	/// </summary>
    	/// <returns>Un dump des valeurs actuelles des propriétés trackables.</returns>
    	IDictionary<string,dynamic> GetGraphValues();
    
        #region Propriétés de présentation
        
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'entité est nouvelle.
        /// </summary>
        bool IsMarkedAsAdded { get; }
        
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'entité est modifiée.
        /// </summary>
        bool IsMarkedAsModified { get; }
        
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'entité est inchangée.
        /// </summary>
        bool IsMarkedAsUnchanged { get; }
        
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'entité est supprimée.
        /// </summary>
        bool IsMarkedAsDeleted { get; }
        
        /// <summary>
        /// Obtient ou définit une valeur indiquant si l'entité n'est pas inchangée.
        /// </summary>
        bool IsNotMarkedAsUnchanged { get; }
    
        /// <summary>
        /// Lève l'évènement PropertyChanged pour les champs de types "Marked as".
        /// </summary>
        void NotifyMarkedAsPropertyChanged();
    
        #endregion
    }
    
    public class ObjectStateChangingEventArgs : EventArgs
    {
        public ObjectState NewState { get; set; }
    }
    
    public class STEGraphIterator : IEnumerable<IObjectWithChangeTracker>
    {
        protected List<IObjectWithChangeTracker> _items;
    
        #region Properties
    
        public ReadOnlyCollection<IObjectWithChangeTracker> Items =>
            WritableItems.AsReadOnly();
    
        protected List<IObjectWithChangeTracker> WritableItems =>
            _items ?? (_items = new List<IObjectWithChangeTracker>());
    
        #endregion
    
        #region IEnumerable Implementation
    
        public IEnumerator<IObjectWithChangeTracker> GetEnumerator() =>
            Items.GetEnumerator();
    
        IEnumerator IEnumerable.GetEnumerator() =>
            Items.GetEnumerator();
    
        #endregion
    
        public static STEGraphIterator Create<T>(T entity)
        {
            var iterator = new STEGraphIterator();
            iterator.Visit(entity);
            return iterator;
        }
    
        internal void Visit(dynamic entity)
        {
            if (entity == null || WritableItems.Contains(entity))
                return;
            WritableItems.Add(entity);
            Traverse(entity);
        }
    
        internal void Traverse(IObjectWithChangeTracker entity)
        {
            foreach (var currentProperty in entity.GetGraphValues())
            {
                switch (currentProperty.Value)
                {
                    case IObjectWithChangeTracker child:
                        Visit(child);
                        break;
                    case IEnumerable trackableCollection:
                    {
                        foreach (var item in trackableCollection)
                            Visit(item);
                        break;
                    }
                }
            }
        }
    }
    
    public static class ObjectWithChangeTrackerExtensions
    {
        public static bool HasChanges<T>(this T trackingItem) where T : IObjectWithChangeTracker
        {
            var graph = STEGraphIterator.Create(trackingItem);
            return graph.Any(_ => _.ChangeTracker.State != ObjectState.Unchanged);
        }
    
        public static List<IObjectWithChangeTracker> GetChanges<T>(this T trackingItem) where T : IObjectWithChangeTracker
        {
            var graph = STEGraphIterator.Create(trackingItem);
            return graph.Where(_ => _.ChangeTracker.State != ObjectState.Unchanged).ToList();
        }
    
        public static T MarkAsDeleted<T>(this T trackingItem) where T : IObjectWithChangeTracker
        {
            if (trackingItem == null)
            {
                throw new ArgumentNullException("trackingItem");
            }
    
            trackingItem.ChangeTracker.ChangeTrackingEnabled = true;
            trackingItem.ChangeTracker.State = ObjectState.Deleted;
            trackingItem.NotifyMarkedAsPropertyChanged();
            return trackingItem;
        }
    
        public static T MarkAsAdded<T>(this T trackingItem) where T : IObjectWithChangeTracker
        {
            if (trackingItem == null)
            {
                throw new ArgumentNullException("trackingItem");
            }
    
            trackingItem.ChangeTracker.ChangeTrackingEnabled = true;
            trackingItem.ChangeTracker.State = ObjectState.Added;
            trackingItem.NotifyMarkedAsPropertyChanged();
            return trackingItem;
        }
    
        public static T MarkAsModified<T>(this T trackingItem) where T : IObjectWithChangeTracker
        {
            if (trackingItem == null)
            {
                throw new ArgumentNullException("trackingItem");
            }
    
            trackingItem.ChangeTracker.ChangeTrackingEnabled = true;
            trackingItem.ChangeTracker.State = ObjectState.Modified;
            trackingItem.NotifyMarkedAsPropertyChanged();
            return trackingItem;
        }
    
        public static T MarkAsUnchanged<T>(this T trackingItem) where T : IObjectWithChangeTracker
        {
            if (trackingItem == null)
            {
                throw new ArgumentNullException("trackingItem");
            }
    
            trackingItem.ChangeTracker.ChangeTrackingEnabled = true;
            trackingItem.ChangeTracker.State = ObjectState.Unchanged;
            trackingItem.NotifyMarkedAsPropertyChanged();
            return trackingItem;
        }
    
        public static void StartTracking(this IObjectWithChangeTracker trackingItem)
        {
            if (trackingItem == null)
            {
                throw new ArgumentNullException("trackingItem");
            }
    
            trackingItem.ChangeTracker.ChangeTrackingEnabled = true;
        }
    
        public static void StartTracking(params IEnumerable<IObjectWithChangeTracker>[] entitiesCollections)
        {
            foreach (var collection in entitiesCollections)
                foreach (var entity in collection)
                    entity.StartTracking();
        }
    
        public static void StopTracking(this IObjectWithChangeTracker trackingItem)
        {
            if (trackingItem == null)
            {
                throw new ArgumentNullException("trackingItem");
            }
    
            trackingItem.ChangeTracker.ChangeTrackingEnabled = false;
        }
    
        public static void StopTracking(params IEnumerable<IObjectWithChangeTracker>[] entitiesCollections)
        {
            foreach (var collection in entitiesCollections)
                foreach (var entity in collection)
                    entity.StopTracking();
        }
    
        public static void AcceptChanges(this IObjectWithChangeTracker trackingItem)
        {
            if (trackingItem == null)
            {
                throw new ArgumentNullException("trackingItem");
            }
    
            trackingItem.ChangeTracker.AcceptChanges();
            trackingItem.NotifyMarkedAsPropertyChanged();
        }
    	
        public static void CancelChanges(this IObjectWithChangeTracker trackingItem)
        {
            if (trackingItem == null)
            {
                throw new ArgumentNullException("trackingItem");
            }
    
            trackingItem.ChangeTracker.CancelChanges(trackingItem);
            trackingItem.NotifyMarkedAsPropertyChanged();
        }
    	
    	/// <summary>
        /// Remet l'état de l'entité à zéro.
        /// Supprime toutes les valeurs et désactive la mise à jour.
        /// </summary>
        public static void Reset(this IObjectWithChangeTracker trackingItem)
        {
            if (trackingItem == null)
            {
                throw new ArgumentNullException("trackingItem");
            }
    
            trackingItem.ChangeTracker.Reset(trackingItem);
        }
    	
        /// <summary>
        /// Annule les changements sur les collection d'entités spécifiées.
        /// </summary>
        /// <param name="entitiesCollections"></param>
        public static void CancelChanges(params IEnumerable<IObjectWithChangeTracker>[] entitiesCollections)
        {
            foreach (var collection in entitiesCollections)
                foreach (var entity in collection)
                    entity.StopTracking();
    
            foreach (var collection in entitiesCollections)
                foreach (var entity in collection)
                    entity.CancelChanges();
        }
    }
    
    // An System.Collections.ObjectModel.ObservableCollection that raises
    // individual item removal notifications on clear and prevents adding duplicates.
    [CollectionDataContract(Name = "TrackableCollection", Namespace = ModelsConstants.DataContractNamespace)]
    [Serializable]
    public class TrackableCollection<T> : ObservableCollection<T>
    {
    	public TrackableCollection() 
    	{
    	}
    
    	public TrackableCollection(IEnumerable<T> list)
    		: base(list)
    	{
    	}
    
        protected override void ClearItems()
        {
            new List<T>(this).ForEach(t => Remove(t));
        }
    
        protected override void InsertItem(int index, T item)
        {
            if (!this.Contains(item))
            {
                base.InsertItem(index, item);
            }
        }
    }
    
    // An interface that provides an event that fires when complex properties change.
    // Changes can be the replacement of a complex property with a new complex type instance or
    // a change to a scalar property within a complex type instance.
    public interface INotifyComplexPropertyChanging
    {
        event EventHandler ComplexPropertyChanging;
    }
    
    public static class EqualityComparer
    {
        // Helper method to determine if two byte arrays are the same value even if they are different object references
        public static bool BinaryEquals(object binaryValue1, object binaryValue2)
        {
            if (Object.ReferenceEquals(binaryValue1, binaryValue2))
            {
                return true;
            }
    
            byte[] array1 = binaryValue1 as byte[];
            byte[] array2 = binaryValue2 as byte[];
    
            if (array1 != null && array2 != null)
            {
                if (array1.Length != array2.Length)
                {
                    return false;
                }
    
                for (int i = 0; i < array1.Length; i++)
                {
                    if (array1[i] != array2[i])
                    {
                        return false;
                    }
                }
    
                return true;
            }
    
            return false;
        }
    }
    	
    /// <summary>
    /// Représente les données de l'évènement de changement de valeur d'une propriété.
    /// </summary>
    /// <typeparam name="TData">Le type de données stockées.</typeparam>
    public class PropertyChangedEventArgs<TData> : EventArgs
    {
    
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="PropertyChangedEventArgs&lt;TData&gt;"/>.
        /// </summary>
        /// <param name="oldValue">L'ancienne valeur.</param>
        /// <param name="newValue">La nouvelle valeur.</param>
        public PropertyChangedEventArgs(TData oldValue, TData newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }
    
        /// <summary>
        /// Obtient l'ancienne valeur.
        /// </summary>
        public TData OldValue { get; private set; }
    
        /// <summary>
        /// Obtient la nouvelle valeur.
        /// </summary>
        public TData NewValue { get; private set; }
    }
    	
}
