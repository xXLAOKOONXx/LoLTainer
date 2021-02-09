using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.Interfaces
{
    public interface ILCUAPIInformationProvider : INotifyPropertyChanged
    {
        string Queue { get; }
        string ChampionName { get; }
        string CurrentSummonerName { get; }
    }
}
