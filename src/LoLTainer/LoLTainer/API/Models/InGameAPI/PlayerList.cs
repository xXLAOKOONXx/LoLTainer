using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.API.Models.InGameAPI
{
    /// <summary>
    /// Virtual object representing the JSON Array from PlayerList Endpoint
    /// </summary>
    public class PlayerList
    {
        private JArray _jArray;
        /// <summary>
        /// Constructor of <see cref="PlayerList"/>
        /// </summary>
        /// <param name="jArray">JSON Array from PlayerList endpoint</param>
        public PlayerList(JArray jArray)
        {
            _jArray = jArray;
        }
        /// <summary>
        /// Virtuall list of all player entries in the list
        /// </summary>
        public IEnumerable<Player> Players
        {
            get
            {
                foreach(var p in _jArray)
                {
                    yield return new Player(p);
                }
            }
        }

        /// <summary>
        /// Virtual object representing a JSON Array Entry from PlayerList Endpoint
        /// </summary>
        public class Player
        {
            private JToken _jToken;
            /// <summary>
            /// Constructor of <see cref="Player"/>
            /// </summary>
            /// <param name="jToken">JSON Array Entry</param>
            public Player(JToken jToken)
            {
                _jToken = jToken;
            }
            /// <summary>
            /// JSON Property ChampionName
            /// </summary>
            public string ChampionName
            {
                get => _jToken["championName"].ToString();
            }
            /// <summary>
            /// JSON Property Team
            /// </summary>
            public TeamSide Team
            {
                get => _jToken["team"].ToString() == "ORDER" ?
                    TeamSide.ORDER : TeamSide.CHAOS;
            }
            /// <summary>
            /// JSON Property SummonerName
            /// </summary>
            public string SummonerName
            {
                get => _jToken["summonerName"].ToString();
            }
        }

        /// <summary>
        /// Enum to represent the TeamSide a player is on.
        /// </summary>
        public enum TeamSide
        {
            ORDER,
            CHAOS
        }
    }
}
