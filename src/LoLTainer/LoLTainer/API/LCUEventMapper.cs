using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
        private bool isInGame;
        private bool isInChampSelect;
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

        private void OnGameFlowSession(object sender, JArray jArray)
        {
            const string inGame_str = "InProgress";
            const string inChampSelect_str = "ChampSelect";

            var phase = jArray[2]["data"]["phase"].ToString();
            Loggings.Logger.Log(Loggings.LogType.LCU, string.Format("GameFlow Change to phase: {0}", phase));
            switch (phase)
            {
                case inGame_str:
                    if (isInGame)
                    {
                        return;
                    }
                    Loggings.Logger.Log(Loggings.LogType.LCU, "Enter Game occured");
                    InvokeEvent(Misc.Event.EnterGame);
                    isInChampSelect = false;
                    isInGame = true;
                    return;
                case inChampSelect_str:
                    if (isInChampSelect)
                    {
                        return;
                    }
                    Loggings.Logger.Log(Loggings.LogType.LCU, "Enter ChampSelect occured");
                    InvokeEvent(Misc.Event.EnterChampSelect);
                    isInChampSelect = true;
                    isInGame = false;
                    return;
                default:
                    if (isInGame)
                    {
                        Loggings.Logger.Log(Loggings.LogType.LCU, "End Game occured");
                        isInGame = false;
                        InvokeEvent(Misc.Event.EndGame);
                        return;
                    }
                    isInChampSelect = false;
                    return;
            }
        }
        #endregion
    }
}