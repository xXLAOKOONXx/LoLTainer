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
        /// <summary>
        /// Provides a list of all settings. Changes will not be saved in file, therefore trigger <see cref="ISettingsManager.Close"/> on close of the application.
        /// On adding or removing an item there is no notification, the function need to be called again.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Setting> GetAllSettings();
        /// <summary>
        /// Removes a setting from the list
        /// </summary>
        /// <param name="setting"><see cref="Setting"/> to remove</param>
        /// <returns>whether there was such an setting to remove or not</returns>
        bool RemoveSetting(Setting setting);
        /// <summary>
        /// Adds an setting to the list if there is no setting in list with the same <see cref="Misc.Event"/>
        /// </summary>
        /// <param name="setting"><see cref="Setting"/>Setting to add</param>
        /// <returns>false when there was an setting in the list having the same <see cref="Misc.Event"/>, true when added successfully</returns>
        bool AddSetting(Setting setting);
        /// <summary>
        /// Check Existance of the file provided with associated setting.
        /// </summary>
        /// <param name="setting"><see cref="Setting"/> to check, does not need to be part of the list</param>
        /// <returns>true if file exists, false otherwise</returns>
        bool CheckFileExists(Setting setting);
        /// <summary>
        /// Method to check all settings inside <see cref="ISettingsManager"/> on file existance
        /// </summary>
        /// <returns>true if all files exist</returns>
        bool CheckAllFilesExist();
        /// <summary>
        /// Method to ensure proper closing and saving of done changes
        /// </summary>
        void Close();
    }
}
