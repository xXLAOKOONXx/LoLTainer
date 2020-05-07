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
        /// <summary>
        /// Creates a binding with the summoner name as string
        /// </summary>
        /// <returns>new binding on a string</returns>
        Binding SummonerNameBinding();
        /// <summary>
        /// Creates a binding with the summoner icon id as string
        /// </summary>
        /// <returns>new binding on a int</returns>
        Binding SummonerIconBinding();
        /// <summary>
        /// Creates a binding with the connection messages that give the user information about the current status of the connection towards league client.
        /// </summary>
        /// <returns>new binding on a string</returns>
        Binding APIConnectionMessageBinding();
        /// <summary>
        /// Function to configure whether the services using the ingame api are in use or not
        /// </summary>
        /// <param name="active">On true activates the InGameAPI, on false deactivates it.</param>
        void SetInGameAPIOnOff(bool active);

        /*
         * How to implement binding:
         * 
        Binding ClubTagBinding = new Binding("CurrentSummonerClubTag");
        ClubTagBinding.Source = _lCUConnection;
        return ClubTagBinding;
            */
    }
}
