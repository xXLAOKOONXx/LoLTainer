using LoLTainer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.Interfaces
{
    public interface ISettingsManager : INotifyPropertyChanged
    {
        EventActionSetting EventActionSetting { get; }

        void ReadFromFile(string fileName);
        void Save();
        bool CheckFileExists(EventActionSetting eventActionSetting);
        void Close();
    }
}
