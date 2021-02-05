using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.Models
{
    [Serializable]
    public class BaseBundle : INotifyPropertyChanged
    {
        public BaseBundle()
        {

        }

        private Dictionary<string, object> _properties;

        public Dictionary<string, object> Properties
        {
            get => _properties;
            set
            {
                _properties = value;
                NotifyPropertyChanged();
            }
        }

        #region INotifyPropertyChanged implementation
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
