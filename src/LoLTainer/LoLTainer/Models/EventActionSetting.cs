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
    public class EventActionSetting : INotifyPropertyChanged
    {
        public EventActionSetting()
        {
            
        }
        private string _fileName;

        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                NotifyPropertyChanged();
            }
        }

        private Dictionary<Misc.Event, List<PropertyBundle>> _settings = new Dictionary<Misc.Event, List<PropertyBundle>>();

        public Dictionary<Misc.Event, List<PropertyBundle>> Settings
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
