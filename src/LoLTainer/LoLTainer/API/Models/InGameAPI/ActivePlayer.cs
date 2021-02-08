using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.API.Models.InGameAPI
{
    /// <summary>
    /// Virtual object for JSON Object from ActivePlayer Endpoint
    /// </summary>
    public class ActivePlayer
    {
        private JObject _jObject;
        /// <summary>
        /// Constructor of <see cref="ActivePlayer"/>
        /// </summary>
        /// <param name="jObject">JSON Object from endpoint</param>
        public ActivePlayer(JObject jObject)
        {
            _jObject = jObject;
        }
        /// <summary>
        /// Returns the field value of the JSON Object for "summonerName"
        /// </summary>
        public string SummonerName
        {
            get
            {
                if(_jObject == null)
                {
                    return "";
                }
                return _jObject["summonerName"].ToString();
            }
        }
    }
}
