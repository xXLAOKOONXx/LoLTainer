using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LoLTainer.Misc;

namespace LoLTainer.Models
{
    public class Setting : INotifyPropertyChanged
    {
        private Misc.Event _event;
        private string _fileName;
        private int _playLengthInSec = 10;
        private int _soundPlayerGroup = 0;

        public string FileName
        {
            get => _fileName; set
            {
                NotifyPropertyChanged();
                _fileName = value;
            }
        }
        public int PlayLengthInSec
        {
            get => _playLengthInSec; set
            {
                NotifyPropertyChanged();
                _playLengthInSec = value;
            }
        }
        public int SoundPlayerGroup
        {
            get => _soundPlayerGroup; set
            {
                NotifyPropertyChanged();
                _soundPlayerGroup = value;
            }
        }
        public Event Event
        {
            get => _event; set
            {
                NotifyPropertyChanged();
                _event = value;
            }
        }

        public Setting(Event @event, string fileName)
        {
            _event = @event;
            _fileName = fileName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
