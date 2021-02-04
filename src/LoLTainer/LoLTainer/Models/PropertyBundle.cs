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
    public class PropertyBundle : INotifyPropertyChanged
    {
        public PropertyBundle()
        {

        }

        private Misc.ActionManager _actionManager;
        public Misc.ActionManager ActionManager
        {
            get => _actionManager;
            set
            {
                _actionManager = value;
                NotifyPropertyChanged();
            }
        }

        private Dictionary<string, object> properties;

        #region INotifyPropertyChanged implementation
        [field: NonSerialized]
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
