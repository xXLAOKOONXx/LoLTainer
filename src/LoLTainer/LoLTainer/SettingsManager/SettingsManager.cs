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

        public SettingsManager()
        {
            /* If SettingsFile Exists and works load into _settings
             * else build new empty list
             * 
             * 
             */
            if (File.Exists(_settingsPath))
            {
                ReadSettingsFromFile();
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
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(_settingsPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            _settings = (List<Setting>)formatter.Deserialize(stream);
            stream.Close();
        }
        private void WriteSettingsToFile()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(_settingsPath, FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, _settings);
            stream.Close();
        }
        #endregion

        public bool AddSetting(Setting setting)
        {
            if(_settings.Select(s => s.Event).Contains(setting.Event))
            {
                return false;
            }
            _settings.Add(setting);
            WriteSettingsToFile();
            return true;
        }

        public bool CheckAllFilesExist()
        {
            foreach(var item in _settings)
            {
                if (!CheckFileExists(item))
                {
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
            foreach(var item in _settings)
            {
                yield return item;
            }
        }

        public bool RemoveSetting(Setting setting)
        {
            var ret = _settings.Remove(setting);
            return ret;
        }

        public void Close()
        {
            WriteSettingsToFile();
        }
    }
}
