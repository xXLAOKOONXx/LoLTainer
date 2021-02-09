using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LoLTainer.Misc;

namespace LoLTainer.EventAPIManagers
{
    public abstract class BaseEventAPIManager : IdentifiableObject, INotifyPropertyChanged, Interfaces.IEventAPIManager
    {
        private bool _connected = false;

        protected IEnumerable<Misc.Event> _activeEvents = null;

        protected EventHandler<Models.EventTriggeredEventArgs> _eventHandler;

        protected void TriggerEvent(Event @event) => _eventHandler?.Invoke(this, new Models.EventTriggeredEventArgs(@event));

        protected BaseEventAPIManager()
        {

        }

        public bool Connected
        {
            get => _connected;
            protected set
            {
                _connected = value;
                NotifyPropertyChanged();
            }
        }


        abstract public void Connect();

        abstract public void DisConnect();

        public EventHandler<Models.EventTriggeredEventArgs> EventHandler
        {
            get
            {
                return _eventHandler;
            }
            set
            {
                _eventHandler = value;
                NotifyPropertyChanged();
            }
        }

        abstract public IEnumerable<Event> GetSupportedEvents();

        public void RestartConnection()
        {
            DisConnect();
            Connect();
        }

        public void SetActiveEvents(IEnumerable<Event> events)
        {
            _activeEvents = events;
        }

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
