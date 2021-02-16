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
            lCUManager.GameFlowSessionEventHandler += OnGameFlowSession;
        }

        private void InvokeEvent(Misc.Event @event) => InvokeEvent(new LoLTainer.Models.EventTriggeredEventArgs(@event));
        private void InvokeEvent(LoLTainer.Models.EventTriggeredEventArgs args)
        {
            _lCUManager.EventHandler?.Invoke(this, args);
        }

        private ClientPhase _clientPhase = ClientPhase.None;
        private void OnGameFlowSession(object sender, JArray jArray)
        {
            try
            {
                var qid = int.Parse(jArray[2]["data"]["gameData"]["queue"]["id"].ToString());
                _lCUManager.QueueId = qid;
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

            }
            foreach (ClientPhase clientPhase in Enum.GetValues(typeof(ClientPhase)))
            {
                if (clientPhase.ToString() == phase)
                {
                    _clientPhase = clientPhase;
                }
            }
            if (_clientPhase == prevClientPhase)
            {
                return;
            }
            Loggings.Logger.Log(Loggings.LogType.LCU, string.Format("New GameFlow Phase: {0}", phase));
            if (prevClientPhase == ClientPhase.InProgress)
            {
                Loggings.Logger.Log(Loggings.LogType.LCU, "End Game occured");
                InvokeEvent(Misc.Event.EndGame);
            }
            switch (_clientPhase)
            {
                case ClientPhase.InProgress:
                    Loggings.Logger.Log(Loggings.LogType.LCU, "Enter Game occured");
                    InvokeEvent(Misc.Event.EnterGame);
                    break;
                case ClientPhase.ChampSelect:
                    Loggings.Logger.Log(Loggings.LogType.LCU, "Enter ChampSelect occured");
                    InvokeEvent(Misc.Event.EnterChampSelect);
                    break;
                case ClientPhase.Lobby:
                    Loggings.Logger.Log(Loggings.LogType.LCU, "Enter Lobby occured");
                    InvokeEvent(Misc.Event.EnterLobby);
                    break;
                case ClientPhase.Matchmaking:
                    Loggings.Logger.Log(Loggings.LogType.LCU, "Enter Matchmaking occured");
                    InvokeEvent(Misc.Event.EnterMatchmaking);
                    break;
                case ClientPhase.None:
                case ClientPhase.Unidentified:
                default:
                    break;
            }
        }

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