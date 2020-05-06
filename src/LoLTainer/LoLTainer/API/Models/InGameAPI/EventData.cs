using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.API.Models.InGameAPI
{
    public class EventData
    {
        private JObject _jArray;

        public EventData(JObject jArray)
        {
            _jArray = jArray;
        }
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

        public class Event
        {
            private JToken _jToken;
            public Event(JToken jToken)
            {
                _jToken = jToken;
            }
            public string EventName
            {
                get => _jToken["EventName"].ToString();
            }
            public TimeSpan EventTime
            {
                get => TimeSpan.FromSeconds(double.Parse( _jToken["EventTime"].ToString()));
            }
            public string EventID
            {
                get => _jToken["EventID"].ToString();
            }
            public string KillerName
            {
                get => _jToken["KillerName"].ToString();
            }
            public string VictimName
            {
                get => _jToken["VictimName"].ToString();
            }
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
