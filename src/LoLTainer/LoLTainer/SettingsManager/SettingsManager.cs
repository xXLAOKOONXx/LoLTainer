using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using LoLTainer.Models;

namespace LoLTainer.SettingsManager
{
    public class SettingsManager : Interfaces.ISettingsManager
    {
        private EventActionSetting _eventActionSetting;
        /// <summary>
        /// Constructor for <see cref="SettingsManager"/>.
        /// Reads stored settings from file or creates a blank list of settings otherwise
        /// </summary>
        public SettingsManager()
        {
            if (File.Exists(_settingsPath))
            {
                try
                {
                    ReadSettingsFromFile(_settingsPath);
                }
                catch (Exception ex)
                {
                    _eventActionSetting = new EventActionSetting();
                    _eventActionSetting.FileName = _settingsPath;
                }
            }
            else
            {
                _eventActionSetting = new EventActionSetting();
                _eventActionSetting.FileName = _settingsPath;
            }
            _eventActionSetting.PropertyChanged += (o, s) => this.NotifyPropertyChanged("EventActionSetting");
        }

        #region file interactions
        private string _settingsPath
        {
            get
            {
                return Path.Combine(AppContext.BaseDirectory, "soundsettings.lt");
            }
        }
        private void ReadSettingsFromFile(string fileName)
        {
            Loggings.Logger.Log(Loggings.LogType.Settings, "Reading settings from file");
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            _eventActionSetting = (EventActionSetting)formatter.Deserialize(stream);
            stream.Close();
            Loggings.Logger.Log(Loggings.LogType.Settings, "Settings read from file");
        }
        private void WriteSettingsToFile()
        {
            Loggings.Logger.Log(Loggings.LogType.Settings, "Saving settings in file");
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(_eventActionSetting.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, _eventActionSetting);
            stream.Close();
            Loggings.Logger.Log(Loggings.LogType.Settings, "Settings saved in file");
        }
        #endregion

        #region ISettingsManager Implementation
        public void ReadFromFile(string fileName)
        {
            Save();
            ReadSettingsFromFile(fileName);

        }

        public EventActionSetting EventActionSetting
        {
            get
            {
                return _eventActionSetting;
            }
        }

        public void Save()
        {
            WriteSettingsToFile();
        }
        
        public bool CheckFileExists(EventActionSetting eventActionSetting)
        {
            return File.Exists(eventActionSetting.FileName);
        }

        public void Close()
        {
            WriteSettingsToFile();
        }
        #endregion

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
