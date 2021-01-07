using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using LoLTainer.Models;

namespace LoLTainer.SettingsManager
{
    public class SettingsManager : Interfaces.ISettingsManager
    {
        private List<Setting> _settings;
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
                    ReadSettingsFromFile();
                }
                catch (Exception ex)
                {
                    _settings = new List<Setting>();
                }
            }
            else
            {
                _settings = new List<Setting>();
            }
        }

        #region file interactions
        private string _settingsPath
        {
            get
            {
                return Path.Combine(AppContext.BaseDirectory, "soundsettings.lt");
            }
        }
        private void ReadSettingsFromFile()
        {
            Loggings.Logger.Log(Loggings.LogType.Settings, "Reading settings from file");
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(_settingsPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            _settings = (List<Setting>)formatter.Deserialize(stream);
            stream.Close();
            Loggings.Logger.Log(Loggings.LogType.Settings, "Settings read from file");
        }
        private void WriteSettingsToFile()
        {
            Loggings.Logger.Log(Loggings.LogType.Settings, "Saving settings in file");
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(_settingsPath, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, _settings);
            stream.Close();
            Loggings.Logger.Log(Loggings.LogType.Settings, "Settings saved in file");
        }
        #endregion

        #region ISettingsManager Implementation
        public bool AddSetting(Setting setting)
        {
            if (_settings.Select(s => s.Event).Contains(setting.Event))
            {
                Loggings.Logger.Log(Loggings.LogType.Settings, "Adding settings tried, but already exists");
                return false;
            }
            _settings.Add(setting);
            Loggings.Logger.Log(Loggings.LogType.Settings, "New setting added: " + setting.Event.ToString() + " " + setting.FileName);
            WriteSettingsToFile();
            return true;
        }

        public bool CheckAllFilesExist()
        {
            Loggings.Logger.Log(Loggings.LogType.Settings, "Checking all files");
            foreach (var item in _settings)
            {
                if (!CheckFileExists(item))
                {
                    Loggings.Logger.Log(Loggings.LogType.Settings, "(!) File of setting not existing: " + item.Event.ToString() + " " + item.FileName);
                    return false;
                }
            }
            return true;
        }

        public bool CheckFileExists(Setting setting)
        {
            return File.Exists(setting.FileName);
        }

        public IEnumerable<Setting> GetAllSettings()
        {
            foreach (var item in _settings)
            {
                yield return item;
            }
        }

        public bool RemoveSetting(Setting setting)
        {
            var ret = _settings.Remove(setting);
            Loggings.Logger.Log(Loggings.LogType.Settings, "Returning Setting " + setting.Event.ToString() + " was " + (ret ? "successful" : "unsuccsessful"));
            if (ret)
            {
                this.WriteSettingsToFile();
            }
            return ret;
        }

        public void Close()
        {
            WriteSettingsToFile();
        }
        #endregion
    }
}
