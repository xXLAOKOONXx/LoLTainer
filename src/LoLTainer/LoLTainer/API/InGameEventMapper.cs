using LoLTainer.API.Models.InGameAPI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.API
{
    public class InGameEventMapper
    {
        #region InGame Actions
        public EventHandler PlayerKill;
        public EventHandler PlayerDoubleKill;
        public EventHandler PlayerTripleKill;
        public EventHandler PlayerQuodraKill;
        public EventHandler PlayerPentaKill;

        public EventHandler PlayerBaron;
        public EventHandler PlayerDragon;
        #endregion

        #region private properties

        private TimeSpan _multiKillDifference = TimeSpan.FromSeconds(5);
        private IEnumerable<string> _teamMateSummonerNames;
        private IEnumerable<string> _enemySummonerNames;
        private string _playerSummonerName;
        private Models.InGameAPI.PlayerList.TeamSide _playerTeamSide;

        private EventData _mostRecentEventData;

        private List<TimeSpan> _playerKills = new List<TimeSpan>();
        private int _playerMultikill = 0;

        private EventHandler NoMapping;
        #endregion
        #region Constructors

        public InGameEventMapper(InGameApiManager inGameApiManager)
        {

            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Setting up InGameMapper");
            GetPlayerInformation(inGameApiManager).Wait();
            AddPlayerKillEvents(inGameApiManager);
            inGameApiManager.OnGameEvent += OnGameEvent;
            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "InGameMapper set up");
        }
        #endregion
        #region public methods
        /// <summary>
        /// Get the <see cref="EventHandler"/> for a specific <paramref name="event"/>
        /// </summary>
        /// <param name="event"></param>
        /// <returns>Eventhandler for the <paramref name="event"/>, if not supported it returns an eventhandler that will never be triggered</returns>
        public ref EventHandler GetEventHandler(Misc.Event @event)
        {
            switch (@event)
            {
                case Misc.Event.PlayerKill:
                    return ref PlayerKill;
                case Misc.Event.PlayerDoubleKill:
                    return ref PlayerDoubleKill;
                case Misc.Event.PlayerTripleKill:
                    return ref PlayerTripleKill;
                case Misc.Event.PlayerQuodraKill:
                    return ref PlayerQuodraKill;
                case Misc.Event.PlayerPentaKill:
                    return ref PlayerPentaKill;
                case Misc.Event.PlayerDragonKill:
                    return ref PlayerDragon;
                case Misc.Event.PlayerBaronKill:
                    return ref PlayerBaron;

                default:
                    return ref NoMapping;
            }
        }
        #endregion
        #region private methods

        private async Task GetPlayerInformation(InGameApiManager inGameApiManager)
        {

            _playerSummonerName = (await inGameApiManager.GetActivePlayer()).SummonerName;
            var playerList = await inGameApiManager.GetPlayerList();
            foreach (var p in playerList.Players)
            {
                if (p.SummonerName == _playerSummonerName)
                {
                    _playerTeamSide = p.Team;
                }
            }
            var teamMateSummonerNames = new List<string>();
            var enemySummonerNames = new List<string>();
            foreach (var p in playerList.Players)
            {
                if (p.SummonerName == _playerSummonerName) { }
                else
                if (p.Team == _playerTeamSide)
                {
                    teamMateSummonerNames.Add(p.SummonerName);
                }
                else
                {
                    enemySummonerNames.Add(p.SummonerName);
                }
            }
            _teamMateSummonerNames = teamMateSummonerNames;
            _enemySummonerNames = enemySummonerNames;
        }

        private void AddPlayerKillEvents(InGameApiManager inGameApiManager)
        {
            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Adding PlayerKillEvents");
            inGameApiManager.OnGameEvent += OnGameEvent;
        }

        private void OnGameEvent(object sender, EventData eventData)
        {
            foreach (var ev in eventData.Events)
            {
                if (_mostRecentEventData == null || !_mostRecentEventData.Events.Select(x => x.EventID).Contains(ev.EventID))
                {
                    if (ev.EventName == EventData.EventNames.ChampionKill)
                    {
                        if (ev.KillerName == _playerSummonerName)
                        {
                            if (_playerKills.Count == 0 || !(_playerKills.Last() + _multiKillDifference > ev.EventTime))
                            {
                                _playerKills.Add(ev.EventTime);
                                PlayerKill?.Invoke(null, null);
                                _playerMultikill = 1;
                                Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Player Single Kill occured");
                            }
                            else
                            {
                                _playerKills.Add(ev.EventTime);
                                _playerMultikill++;
                                switch (_playerMultikill)
                                {
                                    case 2:
                                        PlayerDoubleKill?.Invoke(null, null);
                                        Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Player Double Kill occured");
                                        break;
                                    case 3:
                                        PlayerTripleKill?.Invoke(null, null);
                                        Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Player Triple Kill occured");
                                        break;
                                    case 4:
                                        PlayerQuodraKill?.Invoke(null, null);
                                        Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Player Quodra Kill occured");
                                        break;
                                    default:
                                        PlayerPentaKill?.Invoke(null, null);
                                        Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Player Penta Kill occured");
                                        break;
                                }
                            }
                        }
                    }
                    else
                    if (ev.EventName == EventData.EventNames.DragonKill)
                    {
                        if (ev.KillerName == _playerSummonerName)
                        {
                            PlayerDragon?.Invoke(null, null);
                            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Player Dragon Kill occured");
                        }
                    }
                    else
                    if (ev.EventName == EventData.EventNames.BaronKill)
                    {
                        if (ev.KillerName == _playerSummonerName)
                        {
                            PlayerBaron?.Invoke(null, null);
                            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Player Baron Kill occured");
                        }
                    }
                }
            }
            _mostRecentEventData = eventData;
        }
        #endregion
    }
}
