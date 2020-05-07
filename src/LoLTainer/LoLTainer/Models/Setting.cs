using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using LoLTainer.Misc;

namespace LoLTainer.Models
{
    [Serializable]
    public class Setting : INotifyPropertyChanged
    {
        private Misc.Event _event;
        private string _fileName;
        private int _playLengthInSec = 10;
        private int _soundPlayerGroup = 0;

        /// <summary>
        /// Full file name for the sound played when setting triggers
        /// </summary>
        public string FileName
        {
            get => _fileName; set
            {
                NotifyPropertyChanged();
                _fileName = value;
            }
        }
        /// <summary>
        /// Duration to play the sound (in seconds), default value is 10 seconds
        /// </summary>
        public int PlayLengthInSec
        {
            get => _playLengthInSec; set
            {
                NotifyPropertyChanged();
                _playLengthInSec = value;
            }
        }
        /// <summary>
        /// Group of the Sound. Sounds in the same group override each other, default value is 0;
        /// </summary>
        public int SoundPlayerGroup
        {
            get => _soundPlayerGroup; set
            {
                NotifyPropertyChanged();
                _soundPlayerGroup = value;
            }
        }
        /// <summary>
        /// The <see cref="Event"/> that should trigger the sound to play.
        /// </summary>
        public Event Event
        {
            get => _event; set
            {
                NotifyPropertyChanged();
                _event = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="event">Event</param>
        /// <param name="fileName">Full file path of sound to play</param>
        public Setting(Event @event, string fileName)
        {
            _event = @event;
            _fileName = fileName;
        }

        /// <summary>
        /// Implementation of INotifyPropertyChanged
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Implementation of INotifyPropertyChanged
        /// </summary>
        /// <param name="propertyName"></param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Constructor for Serialization, please use <see cref="Setting.Setting(Event, string)"/> instead
        /// </summary>
        public Setting()
        {

        }
    }
}
