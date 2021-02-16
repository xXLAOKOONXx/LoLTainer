using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.API
{
    public class LCUEventMapper : IdentifiableObject
    {
        #region InGame Actions
        public EventHandler EnterGame;
        public EventHandler EnterChampSelect;
        public EventHandler EndGame;
        #endregion

        #region private properties
        private LCUManager _lCUManager;
        /// <summary>
        /// Eventhandler to hold Events not triggered by this EventMapper.
        /// </summary>
        private EventHandler NoMapping;
        #endregion
        #region Constructors

        public LCUEventMapper(LCUManager lCUManager) : base()
        {
            _lCUManager = lCUManager;
            Loggings.Logger.Log(Loggings.LogType.LCU, "Setting up LCUMapper");
            AddLCUGameFlowEvents(lCUManager);
            Loggings.Logger.Log(Loggings.LogType.LCU, "LCUMapper set up");
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
                case Misc.Event.EnterChampSelect:
                    return ref this.EnterChampSelect;
                case Misc.Event.EnterGame:
                    return ref EnterGame;
                case Misc.Event.EndGame:
                    return ref EndGame;

                default:
                    return ref NoMapping;
            }
        }
        #endregion
        #region private methods


        private void AddLCUGameFlowEvents(LCUManager lCUManager)
        {
            Loggings.Logger.Log(Loggings.LogType.LCU, "Adding LCUGameFlowEvents");
            lCUManager.GameFlowSessionEventHandler += PREVOnGameFlowSession;
        }

        private void InvokeEvent(Misc.Event @event) => InvokeEvent(new LoLTainer.Models.EventTriggeredEventArgs(@event));
        private void InvokeEvent(LoLTainer.Models.EventTriggeredEventArgs args)
        {
            _lCUManager.EventHandler?.Invoke(this, args);
        }

        private string _clientPhase = "";
        private const string PHASE_NONE = "None";
        private const string PHASE_IN_PROGRESS = "InProgress";
        private const string PHASE_LOBBY = "Lobby";
        private const string PHASE_CHAMP_SELECT = "Champselect";
        private const string PHASE_MATCHMAKING = "Matchmaking";
        private void OnGameFlowSession(object sender, JArray jArray)
        {
            Loggings.Logger.Log(Loggings.LogType.LCU, string.Format("GameFlowSession changed"));
            try
            {
                var qid = int.Parse(jArray[2]["data"]["gameData"]["queue"]["id"].ToString());
                _lCUManager.QueueId = qid;
                Loggings.Logger.Log(Loggings.LogType.LCU, string.Format("Identified Queue {0}", qid));
            }
            catch (Exception ex)
            {
                Loggings.Logger.Log(Loggings.LogType.LCU, string.Format("Exception in getting QueueId from Phase Event: {0}", ex.Message));
            }

            var prevClientPhase = _clientPhase;
            string phase = prevClientPhase.ToString();
            try
            {
                phase = jArray[2]["data"]["phase"].ToString();
            }
            catch (Exception ex)
            {
                Loggings.Logger.Log(Loggings.LogType.LCU, string.Format("Exception on reading phase; {0}", ex.Message));
            }
            if (prevClientPhase == phase)
            {
                return;
            }
            _clientPhase = phase;
            Loggings.Logger.Log(Loggings.LogType.LCU, string.Format("New GameFlow Phase: {0}", phase));
            if (prevClientPhase == PHASE_IN_PROGRESS)
            {
                Loggings.Logger.Log(Loggings.LogType.LCU, "End Game occured");
                InvokeEvent(Misc.Event.EndGame);
            }
            switch (_clientPhase)
            {
                case PHASE_IN_PROGRESS:
                    Loggings.Logger.Log(Loggings.LogType.LCU, "Enter Game occured");
                    InvokeEvent(Misc.Event.EnterGame);
                    break;
                case PHASE_CHAMP_SELECT:
                    Loggings.Logger.Log(Loggings.LogType.LCU, "Enter ChampSelect occured");
                    InvokeEvent(Misc.Event.EnterChampSelect);
                    break;
                case PHASE_LOBBY:
                    Loggings.Logger.Log(Loggings.LogType.LCU, "Enter Lobby occured");
                    InvokeEvent(Misc.Event.EnterLobby);
                    break;
                case PHASE_MATCHMAKING:
                    Loggings.Logger.Log(Loggings.LogType.LCU, "Enter Matchmaking occured");
                    InvokeEvent(Misc.Event.EnterMatchmaking);
                    break;
                default:
                    break;
            }
        }
        // PREV
        private bool PREVisInGame = false;
        private bool PREVisInChampSelect = false;
        private ClientPhase PREVclientPhase = ClientPhase.None;
        private void PREVOnGameFlowSession(object sender, JArray jArray)
        {
            const string inGame_str = "InProgress";
            const string inChampSelect_str = "ChampSelect";

            var phase = jArray[2]["data"]["phase"].ToString();
            Loggings.Logger.Log(Loggings.LogType.LCU, string.Format("GameFlow Change to phase: {0}", phase));
            switch (phase)
            {
                case inGame_str:
                    if (PREVclientPhase == ClientPhase.InProgress)
                    {
                        return;
                    }
                    Loggings.Logger.Log(Loggings.LogType.LCU, "Enter Game occured");
                    InvokeEvent(Misc.Event.EnterGame);
                    PREVclientPhase = ClientPhase.InProgress;
                    return;
                case inChampSelect_str:
                    if (PREVclientPhase == ClientPhase.ChampSelect)
                    {
                        return;
                    }
                    Loggings.Logger.Log(Loggings.LogType.LCU, "Enter ChampSelect occured");
                    InvokeEvent(Misc.Event.EnterChampSelect);
                    PREVclientPhase = ClientPhase.ChampSelect;
                    return;
                case PHASE_LOBBY:
                    if(PREVclientPhase == ClientPhase.Lobby)
                    {
                        return;
                    }
                    Loggings.Logger.Log(Loggings.LogType.LCU, "Enter Lobby occured");
                    InvokeEvent(Misc.Event.EnterLobby);
                    PREVclientPhase = ClientPhase.Lobby;
                    return;
                case PHASE_MATCHMAKING:
                    if (PREVclientPhase == ClientPhase.Matchmaking)
                    {
                        return;
                    }
                    Loggings.Logger.Log(Loggings.LogType.LCU, "Enter Matchmaking occured");
                    InvokeEvent(Misc.Event.EnterMatchmaking);
                    PREVclientPhase = ClientPhase.Matchmaking;
                    return;

                default:
                    if (PREVclientPhase == ClientPhase.InProgress)
                    {
                        Loggings.Logger.Log(Loggings.LogType.LCU, "End Game occured");
                        PREVclientPhase = ClientPhase.None;
                        InvokeEvent(Misc.Event.EndGame);
                        return;
                    }
                    PREVclientPhase = ClientPhase.None;
                    return;
            }
            /*
             * const string inGame_str = "InProgress";
            const string inChampSelect_str = "ChampSelect";

            var phase = jArray[2]["data"]["phase"].ToString();
            Loggings.Logger.Log(Loggings.LogType.LCU, string.Format("GameFlow Change to phase: {0}", phase));
            switch (phase)
            {
                case inGame_str:
                    if (PREVisInGame)
                    {
                        return;
                    }
                    Loggings.Logger.Log(Loggings.LogType.LCU, "Enter Game occured");
                    InvokeEvent(Misc.Event.EnterGame);
                    PREVisInChampSelect = false;
                    PREVisInGame = true;
                    return;
                case inChampSelect_str:
                    if (PREVisInChampSelect)
                    {
                        return;
                    }
                    Loggings.Logger.Log(Loggings.LogType.LCU, "Enter ChampSelect occured");
                    InvokeEvent(Misc.Event.EnterChampSelect);
                    PREVisInChampSelect = true;
                    PREVisInGame = false;
                    return;
                default:
                    if (PREVisInGame)
                    {
                        Loggings.Logger.Log(Loggings.LogType.LCU, "End Game occured");
                        PREVisInGame = false;
                        InvokeEvent(Misc.Event.EndGame);
                        return;
                    }
                    PREVisInChampSelect = false;
                    return;
            }
            */
        }

        // ENDPREV

        private enum ClientPhase
        {
            [Description("None")]
            None,
            [Description("InProgress")]
            InProgress,
            [Description("Lobby")]
            Lobby,
            [Description("Champselect")]
            ChampSelect,
            [Description("Matchmaking")]
            Matchmaking,
            Unidentified

        }
        #endregion
    }
}