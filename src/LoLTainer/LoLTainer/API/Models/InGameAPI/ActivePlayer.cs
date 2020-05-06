using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.API.Models.InGameAPI
{
    public class ActivePlayer
    {
        private JObject _jObject;
        public ActivePlayer(JObject jObject)
        {
            _jObject = jObject;
        }
        public string SummonerName
        {
            get
            {
                return _jObject["summonerName"].ToString();
            }
        }
    }
}
