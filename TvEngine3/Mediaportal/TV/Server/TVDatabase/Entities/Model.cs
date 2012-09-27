﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Der Code wurde aus einer Vorlage generiert.
//
//     Änderungen an dieser Datei führen möglicherweise zu falschem Verhalten und gehen verloren, falls
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace Mediaportal.TV.Server.TVDatabase.Entities
{
    // Hilfsprogrammklasse, die den Großteil der erforderlichen Nachverfolgungsschritte erfasst
    // für Entitäten mit Selbstnachverfolgung.
    [DataContract(IsReference = true)]
    public class ObjectChangeTracker
    {
        #region  Fields
    
        private bool _isDeserializing;
        private ObjectState _objectState = ObjectState.Added;
        private bool _changeTrackingEnabled;
        private OriginalValuesDictionary _originalValues;
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
    
        // Gibt die entfernten Objekte an Eigenschaften mit einem Auflistungswert zurück, die geändert wurden.
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
    
        // Gibt die ursprünglichen Werte für die geänderten Eigenschaften zurück.
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
    
        // Gibt die erweiterten Eigenschaftswerte zurück.
        // Dies schließt Schlüsselwerte für unabhängige Zuordnungen ein, die erforderlich sind für
        // Parallelitätsmodell im Entity Framework
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
    
        // Gibt die hinzugefügten Werte an Eigenschaften mit einem Auflistungswert zurück, die geändert wurden.
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
        public void OnDeserializingMethod(StreamingContext context)
        {
            _isDeserializing = true;
        }
    
        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            _isDeserializing = false;
        }
    
        // Setzt ObjectChangeTracker auf den Zustand "Unverändert" zurück und
        // löscht die ursprünglichen Werte sowie die Aufzeichnung der Änderungen
        // an Sammlungseigenschaften
        public void AcceptChanges()
        {
            OnObjectStateChanging(ObjectState.Unchanged);
            OriginalValues.Clear();
            ObjectsAddedToCollectionProperties.Clear();
            ObjectsRemovedFromCollectionProperties.Clear();
            ChangeTrackingEnabled = true;
            _objectState = ObjectState.Unchanged;
        }
    
        // Erfasst den ursprünglichen Wert für eine Eigenschaft, die geändert wird.
        internal void RecordOriginalValue(string propertyName, object value)
        {
            if (_changeTrackingEnabled && _objectState != ObjectState.Added)
            {
                if (!OriginalValues.ContainsKey(propertyName))
                {
                    OriginalValues[propertyName] = value;
                }
            }
        }
    
        // Zeichnet einen Hinzufügevorgang für Auflistungswerteigenschaften in SelfTracking-Entitäten auf.
        internal void RecordAdditionToCollectionProperties(string propertyName, object value)
        {
            if (_changeTrackingEnabled)
            {
                // Entität nach dem Löschen wieder hinzufügen (es wird empfohlen, hier keine weiteren Schritte auszuführen)
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
    
        // Zeichnet einen Entfernungsvorgang für Auflistungswerteigenschaften in SelfTracking-Entitäten auf.
        internal void RecordRemovalFromCollectionProperties(string propertyName, object value)
        {
            if (_changeTrackingEnabled)
            {
                // Entität nach dem Hinzufügen wieder löschen (es wird empfohlen, hier keine weiteren Schritte auszuführen)
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
    
    [CollectionDataContract (Name = "ObjectsAddedToCollectionProperties",
        ItemName = "AddedObjectsForProperty", KeyName = "CollectionPropertyName", ValueName = "AddedObjects")]
    public class ObjectsAddedToCollectionProperties : Dictionary<string, ObjectList> { }
    
    [CollectionDataContract (Name = "ObjectsRemovedFromCollectionProperties",
        ItemName = "DeletedObjectsForProperty", KeyName = "CollectionPropertyName",ValueName = "DeletedObjects")]
    public class ObjectsRemovedFromCollectionProperties : Dictionary<string, ObjectList> { }
    
    [CollectionDataContract(Name = "OriginalValuesDictionary",
        ItemName = "OriginalValues", KeyName = "Name", ValueName = "OriginalValue")]
    public class OriginalValuesDictionary : Dictionary<string, Object> { }
    
    [CollectionDataContract(Name = "ExtendedPropertiesDictionary",
        ItemName = "ExtendedProperties", KeyName = "Name", ValueName = "ExtendedProperty")]
    public class ExtendedPropertiesDictionary : Dictionary<string, Object> { }
    
    [CollectionDataContract(ItemName = "ObjectValue")]
    public class ObjectList : List<object> { }
    // Die Schnittstelle wird durch die Entitäten mit Selbstnachverfolgung implementiert, die von EF generiert werden.
    // Es ist ein Adapter verfügbar, der die Schnittstelle in die von EF erwartete Schnittstelle konvertiert.
    // Der Adapter befindet sich auf der Serverseite.
    public interface IObjectWithChangeTracker
    {
        // Besitzt alle Informationen zur Änderungsverfolgung für das untergeordnete Diagramm eines angegebenen Objekts.
        ObjectChangeTracker ChangeTracker { get; }
    }
    
    public class ObjectStateChangingEventArgs : EventArgs
    {
        public ObjectState NewState { get; set; }
    }
    
    public static class ObjectWithChangeTrackerExtensions
    {
        public static T MarkAsDeleted<T>(this T trackingItem) where T : IObjectWithChangeTracker
        {
            if (trackingItem == null)
            {
                throw new ArgumentNullException("trackingItem");
            }
    
            trackingItem.ChangeTracker.ChangeTrackingEnabled = true;
            trackingItem.ChangeTracker.State = ObjectState.Deleted;
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
    
        public static void StopTracking(this IObjectWithChangeTracker trackingItem)
        {
            if (trackingItem == null)
            {
                throw new ArgumentNullException("trackingItem");
            }
    
            trackingItem.ChangeTracker.ChangeTrackingEnabled = false;
        }
    
        public static void AcceptChanges(this IObjectWithChangeTracker trackingItem)
        {
            if (trackingItem == null)
            {
                throw new ArgumentNullException("trackingItem");
            }
    
            trackingItem.ChangeTracker.AcceptChanges();
        }
    }
    
    // Eine System.Collections.ObjectModel.ObservableCollection, die Folgendes auslöst:
    // Benachrichtigungen zum Entfernen einzelner Elemente beim Löschen, und das Hinzufügen von Duplikaten wird verhindert.
    public class TrackableCollection<T> : ObservableCollection<T>
    {
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
    
    // Eine Schnittstelle, die ein Ereignis bereitstellt, das bei Änderungen von komplexen Eigenschaften ausgelöst wird.
    // Änderungen können der Ersatz einer komplexen Eigenschaft mit einer neuen komplexen Typinstanz sein oder
    // eine Änderung an einer skalaren Eigenschaft innerhalb einer komplexen Typinstanz.
    public interface INotifyComplexPropertyChanging
    {
        event EventHandler ComplexPropertyChanging;
    }
    
    public static class EqualityComparer
    {
        // Hilfsmethode zum Bestimmen, ob zwei Bytearrays denselben Wert haben, auch wenn sie verschiedene Objektverweise sind
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
}
