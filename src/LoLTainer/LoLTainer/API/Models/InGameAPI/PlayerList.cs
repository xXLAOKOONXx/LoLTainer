using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.API.Models.InGameAPI
{
    public class PlayerList
    {
        private JArray _jArray;
        public PlayerList(JArray jArray)
        {
            _jArray = jArray;
        }
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


        public class Player
        {
            private JToken _jToken;
            public Player(JToken jToken)
            {
                _jToken = jToken;
            }

            public string ChampionName
            {
                get => _jToken["championName"].ToString();
            }
            public TeamSide Team
            {
                get => _jToken["team"].ToString() == "ORDER" ?
                    TeamSide.ORDER : TeamSide.CHAOS;
            }
            public string SummonerName
            {
                get => _jToken["summonerName"].ToString();
            }
        }

        public enum TeamSide
        {
            ORDER,
            CHAOS
        }
    }
}
