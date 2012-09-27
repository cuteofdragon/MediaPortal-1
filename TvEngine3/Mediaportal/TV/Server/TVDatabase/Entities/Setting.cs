//------------------------------------------------------------------------------
// <auto-generated>
//     Der Code wurde aus einer Vorlage generiert.
//
//     �nderungen an dieser Datei f�hren m�glicherweise zu falschem Verhalten und gehen verloren, falls
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
    [DataContract(IsReference = true)]
    public partial class Setting: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Eigenschaften
    
        [DataMember]
        public int IdSetting
        {
            get { return _idSetting; }
            set
            {
                if (_idSetting != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("Die 'IdSetting'-Eigenschaft ist Teil des Objektschl�ssels und kann nicht geändert werden. �nderungen an Schl�sseleigenschaften k�nnen nur ausgef�hrt werden, wenn das Objekt nachverfolgt wird oder sich im Added-Zustand befindet.");
                    }
                    _idSetting = value;
                    OnPropertyChanged("IdSetting");
                }
            }
        }
        private int _idSetting;
    
        [DataMember]
        public string Tag
        {
            get { return _tag; }
            set
            {
                if (_tag != value)
                {
                    _tag = value;
                    OnPropertyChanged("Tag");
                }
            }
        }
        private string _tag;
    
        [DataMember]
        public string Value
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
        private string _value;

        #endregion
        #region ChangeTracking
    
        protected virtual void OnPropertyChanged(String propertyName)
        {
            if (ChangeTracker.State != ObjectState.Added && ChangeTracker.State != ObjectState.Deleted)
            {
                ChangeTracker.State = ObjectState.Modified;
            }
            if (_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    
        protected virtual void OnNavigationPropertyChanged(String propertyName)
        {
            if (_propertyChanged != null)
            {
                _propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged{ add { _propertyChanged += value; } remove { _propertyChanged -= value; } }
        private event PropertyChangedEventHandler _propertyChanged;
        private ObjectChangeTracker _changeTracker;
    
        [DataMember]
        public ObjectChangeTracker ChangeTracker
        {
            get
            {
                if (_changeTracker == null)
                {
                    _changeTracker = new ObjectChangeTracker();
                    _changeTracker.ObjectStateChanging += HandleObjectStateChanging;
                }
                return _changeTracker;
            }
            set
            {
                if(_changeTracker != null)
                {
                    _changeTracker.ObjectStateChanging -= HandleObjectStateChanging;
                }
                _changeTracker = value;
                if(_changeTracker != null)
                {
                    _changeTracker.ObjectStateChanging += HandleObjectStateChanging;
                }
            }
        }
    
        private void HandleObjectStateChanging(object sender, ObjectStateChangingEventArgs e)
        {
            if (e.NewState == ObjectState.Deleted)
            {
                ClearNavigationProperties();
            }
        }
    
        protected bool IsDeserializing { get; private set; }
    
        [OnDeserializing]
        public void OnDeserializingMethod(StreamingContext context)
        {
            IsDeserializing = true;
        }
    
        [OnDeserialized]
        public void OnDeserializedMethod(StreamingContext context)
        {
            IsDeserializing = false;
            ChangeTracker.ChangeTrackingEnabled = true;
        }
    
        protected virtual void ClearNavigationProperties()
        {
        }

        #endregion
    }
}
