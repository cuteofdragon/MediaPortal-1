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
    [KnownType(typeof(Card))]
    [KnownType(typeof(CardGroup))]
    public partial class CardGroupMap: IObjectWithChangeTracker, INotifyPropertyChanged
    {
        #region Primitive Eigenschaften
    
        [DataMember]
        public int IdMapping
        {
            get { return _idMapping; }
            set
            {
                if (_idMapping != value)
                {
                    if (ChangeTracker.ChangeTrackingEnabled && ChangeTracker.State != ObjectState.Added)
                    {
                        throw new InvalidOperationException("Die 'IdMapping'-Eigenschaft ist Teil des Objektschl�ssels und kann nicht geändert werden. �nderungen an Schl�sseleigenschaften k�nnen nur ausgef�hrt werden, wenn das Objekt nachverfolgt wird oder sich im Added-Zustand befindet.");
                    }
                    _idMapping = value;
                    OnPropertyChanged("IdMapping");
                }
            }
        }
        private int _idMapping;
    
        [DataMember]
        public int IdCard
        {
            get { return _idCard; }
            set
            {
                if (_idCard != value)
                {
                    ChangeTracker.RecordOriginalValue("IdCard", _idCard);
                    if (!IsDeserializing)
                    {
                        if (Card != null && Card.IdCard != value)
                        {
                            Card = null;
                        }
                    }
                    _idCard = value;
                    OnPropertyChanged("IdCard");
                }
            }
        }
        private int _idCard;
    
        [DataMember]
        public int IdCardGroup
        {
            get { return _idCardGroup; }
            set
            {
                if (_idCardGroup != value)
                {
                    ChangeTracker.RecordOriginalValue("IdCardGroup", _idCardGroup);
                    if (!IsDeserializing)
                    {
                        if (CardGroup != null && CardGroup.IdCardGroup != value)
                        {
                            CardGroup = null;
                        }
                    }
                    _idCardGroup = value;
                    OnPropertyChanged("IdCardGroup");
                }
            }
        }
        private int _idCardGroup;

        #endregion
        #region Navigationseigenschaften
    
        [DataMember]
        public Card Card
        {
            get { return _card; }
            set
            {
                if (!ReferenceEquals(_card, value))
                {
                    var previousValue = _card;
                    _card = value;
                    FixupCard(previousValue);
                    OnNavigationPropertyChanged("Card");
                }
            }
        }
        private Card _card;
    
        [DataMember]
        public CardGroup CardGroup
        {
            get { return _cardGroup; }
            set
            {
                if (!ReferenceEquals(_cardGroup, value))
                {
                    var previousValue = _cardGroup;
                    _cardGroup = value;
                    FixupCardGroup(previousValue);
                    OnNavigationPropertyChanged("CardGroup");
                }
            }
        }
        private CardGroup _cardGroup;

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
    
        // Dieser Entitätstyp ist das abhängige Ende in mindestens einer Zuordnung, die L�schweitergaben durchf�hrt.
        // Dieser Ereignishandler verarbeitet Benachrichtigungen, die beim L�schen des Prinzipalendes ausgel�st werden.
        internal void HandleCascadeDelete(object sender, ObjectStateChangingEventArgs e)
        {
            if (e.NewState == ObjectState.Deleted)
            {
                this.MarkAsDeleted();
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
            Card = null;
            CardGroup = null;
        }

        #endregion
        #region Fixup f�r Zuordnungen
    
        private void FixupCard(Card previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.CardGroupMaps.Contains(this))
            {
                previousValue.CardGroupMaps.Remove(this);
            }
    
            if (Card != null)
            {
                if (!Card.CardGroupMaps.Contains(this))
                {
                    Card.CardGroupMaps.Add(this);
                }
    
                IdCard = Card.IdCard;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("Card")
                    && (ChangeTracker.OriginalValues["Card"] == Card))
                {
                    ChangeTracker.OriginalValues.Remove("Card");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("Card", previousValue);
                }
                if (Card != null && !Card.ChangeTracker.ChangeTrackingEnabled)
                {
                    Card.StartTracking();
                }
            }
        }
    
        private void FixupCardGroup(CardGroup previousValue)
        {
            if (IsDeserializing)
            {
                return;
            }
    
            if (previousValue != null && previousValue.CardGroupMaps.Contains(this))
            {
                previousValue.CardGroupMaps.Remove(this);
            }
    
            if (CardGroup != null)
            {
                if (!CardGroup.CardGroupMaps.Contains(this))
                {
                    CardGroup.CardGroupMaps.Add(this);
                }
    
                IdCardGroup = CardGroup.IdCardGroup;
            }
            if (ChangeTracker.ChangeTrackingEnabled)
            {
                if (ChangeTracker.OriginalValues.ContainsKey("CardGroup")
                    && (ChangeTracker.OriginalValues["CardGroup"] == CardGroup))
                {
                    ChangeTracker.OriginalValues.Remove("CardGroup");
                }
                else
                {
                    ChangeTracker.RecordOriginalValue("CardGroup", previousValue);
                }
                if (CardGroup != null && !CardGroup.ChangeTracker.ChangeTrackingEnabled)
                {
                    CardGroup.StartTracking();
                }
            }
        }

        #endregion
    }
}
