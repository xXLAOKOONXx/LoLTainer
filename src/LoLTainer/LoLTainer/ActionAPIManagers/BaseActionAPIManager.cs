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
    /// <summary>
    /// Abstract class implementing basic functionality for Action API Managers.
    /// </summary>
    public abstract class BaseActionAPIManager : IdentifiableObject, INotifyPropertyChanged, IActionAPIManager
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected BaseActionAPIManager()
        {

        }
        
        /// <summary>
        /// Use <see cref="Connected"/> to modify; Private variable to store whether the manager is connected or not.
        /// </summary>
        private bool _connect = false;

        /// <summary>
        /// WHether the Manager is connected to the API or not.
        /// </summary>
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
        abstract public void DisConnect();
        public void ReConnect()
        {
            DisConnect();
            Connect();
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
