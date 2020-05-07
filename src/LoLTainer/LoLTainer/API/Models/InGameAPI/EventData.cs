using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.API.Models.InGameAPI
{
    /// <summary>
    /// Virtual object for JSON Array from EventData Endpoint
    /// </summary>
    public class EventData
    {
        private JObject _jArray;

        /// <summary>
        /// Constructor of <see cref="EventData"/>
        /// </summary>
        /// <param name="jArray">JSON Array from endpoint</param>
        public EventData(JObject jArray)
        {
            _jArray = jArray;
        }
        /// <summary>
        /// Virtual List of Events within the given JSON Array
        /// </summary>
        public IEnumerable<Event> Events
        {
            get
            {
                JArray jArray = _jArray["Events"] as JArray;

                foreach(var ev in jArray)
                {
                    yield return new Event(ev);
                }
            }
        }

        /// <summary>
        /// Virtual object for JSON Array entries from EventData Endpoint.
        /// Not all properties might be available. Check <see cref="Event.EventName"/> as base for further request. (e.g. ChampionKill has no stolen property)
        /// </summary>
        public class Event
        {
            private JToken _jToken;
            /// <summary>
            /// Constructor of <see cref="Event"/>
            /// </summary>
            /// <param name="jToken">Entry from JSON Array from EventData Endpoint</param>
            public Event(JToken jToken)
            {
                _jToken = jToken;
            }
            /// <summary>
            /// JSON Property EventName
            /// </summary>
            public string EventName
            {
                get => _jToken["EventName"].ToString();
            }
            /// <summary>
            /// JSON Property EventTime
            /// </summary>
            public TimeSpan EventTime
            {
                get => TimeSpan.FromSeconds(double.Parse( _jToken["EventTime"].ToString()));
            }
            /// <summary>
            /// JSON Property EventID
            /// </summary>
            public string EventID
            {
                get => _jToken["EventID"].ToString();
            }
            /// <summary>
            /// JSON Property KillerName
            /// </summary>
            public string KillerName
            {
                get => _jToken["KillerName"].ToString();
            }

            /// <summary>
            /// JSON Property VictimName
            /// </summary>
            public string VictimName
            {
                get => _jToken["VictimName"].ToString();
            }
            /// <summary>
            /// JSON Property Stolen
            /// </summary>
            public bool Stolen
            {
                get => _jToken["Stolen"].ToString() != "False";
            }
        }

        public static class EventNames
        {
            public const string ChampionKill = "ChampionKill";
            public const string DragonKill = "DragonKill";
            public const string BaronKill = "BaronKill";
        }

    }
}
