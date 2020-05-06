using LoLTainer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.Interfaces
{
    public interface ISettingsManager
    {
        IEnumerable<Setting> GetAllSettings();
        bool RemoveSetting(Setting setting);
        bool AddSetting(Setting setting);
        bool CheckFileExists(Setting setting);
        bool CheckAllFilesExist();

        void Close();
    }
}
