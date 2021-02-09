using LoLTainer.Interfaces;
using LoLTainer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.ActionAPIManagers
{
    public abstract class BaseActionAPIManager : IdentifiableObject, INotifyPropertyChanged, IActionAPIManager
    {
        protected BaseActionAPIManager()
        {

        }

        abstract public Dictionary<string, Type> PropertyList
        {
            get;
        }

        private bool _connect = false;

        public bool Connected
        {
            get => _connect;
            set
            {
                _connect = value;
                NotifyPropertyChanged();
            }
        }

        abstract public IActionWindow GetActionWindow();

        abstract public void PerformAction(PropertyBundle propertyBundle, EventTriggeredEventArgs eventTriggeredEventArgs = null);
        

        abstract public bool IsValidPropertyBundle(PropertyBundle propertyBundle);

        abstract public void Connect();
        

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
