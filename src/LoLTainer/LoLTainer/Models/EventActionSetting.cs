using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.Models
{
    public class EventActionSetting
    {
        Dictionary<Misc.Event, List<PropertyBundle>> _settings;

        Dictionary<Misc.Event, List<PropertyBundle>> Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                NotifyPropertyChanged();
            }
        }

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
