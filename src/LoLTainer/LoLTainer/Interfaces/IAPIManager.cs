using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LoLTainer.Interfaces
{
    interface IAPIManager
    {
        Binding SummonerNameBinding();

        Binding APIConnectionMessageBinding();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="active">On true activates the InGameAPI, on false deactivates it.</param>
        void SetInGameAPIOnOff(bool active);

        /*
         * How to implement binding:
         * 
        Binding ClubTagBinding = new Binding("CurrentSummonerClubTag");
        ClubTagBinding.Source = _lCUConnection;
            */
    }
}
