using LoLTainer.API.Models.InGameAPI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.API
{
    public class InGameEventMapper : IdentifiableObject
    {
        #region InGame Actions
        public EventHandler PlayerAnyKill;
        public EventHandler PlayerSingleKill;
        public EventHandler PlayerDoubleKill;
        public EventHandler PlayerTripleKill;
        public EventHandler PlayerQuodraKill;
        public EventHandler PlayerPentaKill;
        public EventHandler PlayerFirstBlood;

        public EventHandler PlayerBaron;
        public EventHandler PlayerDragon;
        #endregion

        #region private properties

        private InGameApiManager _inGameApiManager;
        private TimeSpan _multiKillDifference = TimeSpan.FromSeconds(5);
        private IEnumerable<string> _teamMateSummonerNames;
        private IEnumerable<string> _enemySummonerNames;
        private string _playerSummonerName;
        private Models.InGameAPI.PlayerList.TeamSide _playerTeamSide;

        private EventData _mostRecentEventData;

        private bool _potentialNewGame = true;
        /// <summary>
        /// Eventhandler to hold Events not triggered by this EventMapper.
        /// </summary>
        private EventHandler NoMapping;
        #endregion
        #region Constructors

        public bool PotentialNewGame
        {
            set
            {
                _potentialNewGame = value;
            }
        }

        public InGameEventMapper(InGameApiManager inGameApiManager) : base()
        {
            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Setting up InGameEventMapper with OID " + base.Id, base.Id);
            _inGameApiManager = inGameApiManager;
            inGameApiManager.OnGameEvent += OnGameEvent;
            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "InGameMapper set up", base.Id);
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
                case Misc.Event.PlayerAnyKill:
                    return ref PlayerAnyKill;
                case Misc.Event.PlayerSingleKill:
                    return ref PlayerSingleKill;
                case Misc.Event.PlayerDoubleKill:
                    return ref PlayerDoubleKill;
                case Misc.Event.PlayerTripleKill:
                    return ref PlayerTripleKill;
                case Misc.Event.PlayerQuodraKill:
                    return ref PlayerQuodraKill;
                case Misc.Event.PlayerPentaKill:
                    return ref PlayerPentaKill;
                case Misc.Event.PlayerFirstBlood:
                    return ref PlayerFirstBlood;
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
            Loggings.Logger.Log(Loggings.LogType.IngameAPI, String.Format("Summoner identified: '{0}'", _playerSummonerName), base.Id);
            var playerList = await inGameApiManager.GetPlayerList();
            foreach (var p in playerList.Players)
            {
                if (p.SummonerName == _playerSummonerName)
                {
                    _playerTeamSide = p.Team;
                    break;
                }
            }
            Loggings.Logger.Log(Loggings.LogType.IngameAPI, String.Format("Identified Summoner Side: '{0}'", _playerTeamSide), base.Id);
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
            Loggings.Logger.Log(Loggings.LogType.IngameAPI, String.Format("Allies identified: '{0}'", _teamMateSummonerNames.ToString()), base.Id);
            Loggings.Logger.Log(Loggings.LogType.IngameAPI, String.Format("Enemies identified: '{0}'", _enemySummonerNames.ToString()), base.Id);
        }

        private void AddPlayerKillEvents(InGameApiManager inGameApiManager)
        {
            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Adding OnGameEvents", base.Id);
            inGameApiManager.OnGameEvent += OnGameEvent;
        }

        private void InvokeEvent(Misc.Event @event, Dictionary<string, object> eventArgs = null) =>
                            _inGameApiManager.EventHandler?.Invoke(this, new LoLTainer.Models.EventTriggeredEventArgs(@event, eventArgs));

        private async Task CheckAndResolveNewGame(EventData eventData)
        {
            if (eventData.Events.Count() == 0 ||
                eventData.Events.Count() < _mostRecentEventData.Events.Count())
            {
                _mostRecentEventData = null;
                await GetPlayerInformation(_inGameApiManager);
            }
        }

        private void OnGameEvent(object sender, EventData eventData)
        {
            if (_potentialNewGame)
            {
                CheckAndResolveNewGame(eventData).Wait();
            }

            foreach (var ev in eventData.Events)
            {
                if (_mostRecentEventData == null || !_mostRecentEventData.Events.Select(x => x.EventID).Contains(ev.EventID))
                {
                    if (ev.EventName == EventData.EventNames.ChampionKill)
                    {
                        if (ev.KillerName == _playerSummonerName)
                        {
                            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Player Any Kill occured", base.Id);
                            InvokeEvent(Misc.Event.PlayerAnyKill);
                            if (IsSingleKill(eventData, ev))
                            {
                                Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Player Single Kill occured", base.Id);
                                InvokeEvent(Misc.Event.PlayerSingleKill);
                            }
                        }
                    }
                    else
                    if (ev.EventName == EventData.EventNames.FirstBlood && ev.Recipient == _playerSummonerName)
                    {
                        Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Player First Blood occured", base.Id);
                        InvokeEvent(Misc.Event.PlayerFirstBlood);
                    }
                    else
                    if (ev.EventName == EventData.EventNames.MultiKill && ev.KillerName == _playerSummonerName)
                    {
                        switch (ev.KillStreak)
                        {
                            case 2:
                                Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Player Double Kill occured", base.Id);
                                InvokeEvent(Misc.Event.PlayerDoubleKill);
                                break;
                            case 3:
                                Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Player Triple Kill occured", base.Id);
                                InvokeEvent(Misc.Event.PlayerTripleKill);
                                break;
                            case 4:
                                Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Player Quodra Kill occured", base.Id);
                                InvokeEvent(Misc.Event.PlayerQuodraKill);
                                break;
                            case 5:
                                Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Player Penta Kill occured", base.Id);
                                InvokeEvent(Misc.Event.PlayerPentaKill);
                                break;
                        }
                    }
                    else
                    if (ev.EventName == EventData.EventNames.DragonKill)
                    {
                        if (ev.KillerName == _playerSummonerName)
                        {
                            if (ev.Stolen && _inGameApiManager.ActiveEvents.Contains(Misc.Event.PlayerDragonSteal))
                            {
                                Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Player Dragon Steal occured", base.Id);
                                InvokeEvent(Misc.Event.PlayerDragonSteal);
                            }
                            else
                            {
                                Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Player Dragon Kill occured", base.Id);
                                InvokeEvent(Misc.Event.PlayerDragonKill);
                            }
                        }
                    }
                    else
                    if (ev.EventName == EventData.EventNames.BaronKill)
                    {
                        if (ev.KillerName == _playerSummonerName)
                        {
                            if (ev.Stolen && _inGameApiManager.ActiveEvents.Contains(Misc.Event.PlayerBaronSteal))
                            {
                                Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Player Baron Steal occured", base.Id);
                                InvokeEvent(Misc.Event.PlayerBaronSteal);
                            }
                            else
                            {
                                Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Player Baron Kill occured", base.Id);
                                InvokeEvent(Misc.Event.PlayerBaronKill);
                            }
                        }
                    }
                    else
                    if (ev.EventName == EventData.EventNames.GameEnd)
                    {
                        if(ev.Result && _inGameApiManager.ActiveEvents.Contains(Misc.Event.EnemyTeamNexusDestroyed))
                        {
                            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Enemy Team Nexus Destroyed occured", base.Id);
                            InvokeEvent(Misc.Event.EnemyTeamNexusDestroyed);
                        }
                        else
                        if(!ev.Result && _inGameApiManager.ActiveEvents.Contains(Misc.Event.TeamNexusDestroyed))
                        {
                            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Team Nexus Destroyed occured", base.Id);
                            InvokeEvent(Misc.Event.TeamNexusDestroyed);
                        }
                        else
                        {
                            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Any Nexus Destroyed occured", base.Id);
                            InvokeEvent(Misc.Event.AnyNexusDestroyed);
                        }
                    }
                    else
                        if(ev.EventName == EventData.EventNames.GameStart)
                    {
                        Loggings.Logger.Log(Loggings.LogType.IngameAPI, string.Format("Game Start occured"), base.Id);
                        InvokeEvent(Misc.Event.StartGame);
                    }
                }
            }
            _mostRecentEventData = eventData;
        }

        private bool IsSingleKill(EventData eventData, EventData.Event ev)
        {
            Loggings.Logger.Log(Loggings.LogType.IngameAPI, String.Format("Checking whether event is single kill {0}", ev.ToString()));
            return eventData.Events
                .Where(item => item.EventTime == ev.EventTime)
                .Where(item => item.EventName == EventData.EventNames.MultiKill ||
                (_inGameApiManager.ActiveEvents.Contains(Misc.Event.PlayerFirstBlood) && item.EventName == EventData.EventNames.FirstBlood))
                .Count() == 0;
        }
        #endregion
    }
}
