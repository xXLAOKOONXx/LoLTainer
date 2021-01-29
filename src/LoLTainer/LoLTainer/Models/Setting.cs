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
        private TimeSpan? _playLength = TimeSpan.FromSeconds(10);
        private int _soundPlayerGroup = 0;
        private int _volume = -1;
        private PlayMode _playMode = PlayMode.StopPlaying;

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
        public TimeSpan? PlayLength
        {
            get => _playLength; set
            {
                NotifyPropertyChanged();
                _playLength = value;
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
        /// The Volume the sound will play at. Normalized to values between 0 and 100. '-1' stands for not changing the volume.
        /// </summary>
        public int Volume
        {
            get => _volume; set
            {
                NotifyPropertyChanged();
                _volume = value;
            }
        }
        /// <summary>
        /// The <see cref="PlayMode"/> the sound will be played with.
        /// </summary>
        public PlayMode PlayMode
        {
            get => _playMode; set
            {
                NotifyPropertyChanged();
                _playMode = value;
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
            Loggings.Logger.Log(Loggings.LogType.Settings, "Setting changed " + this.Event.ToString() + " " + propertyName);
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
