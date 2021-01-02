using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.API
{
    public class LCUEventMapper
    {
        #region InGame Actions
        public EventHandler EnterGame;
        public EventHandler EnterChampSelect;
        public EventHandler EndGame;
        #endregion

        #region private properties
        private bool isInGame;
        private bool isInChampSelect;
        /// <summary>
        /// Eventhandler to hold Events not triggered by this EventMapper.
        /// </summary>
        private EventHandler NoMapping;
        #endregion
        #region Constructors

        public LCUEventMapper(LCUManager lCUManager)
        {

            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Setting up LCUMapper");
            AddLCUGameFlowEvents(lCUManager);
            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "LCUMapper set up");
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
            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Adding LCUGameFlowEvents");
            lCUManager.GameFlowSessionEventHandler += OnGameFlowSession;
        }

        private void OnGameFlowSession(object sender, JArray jArray)
        {
            const string inGame_str = "InProgress";
            const string inChampSelect_str = "ChampSelect";

            var phase = jArray[2]["data"]["phase"].ToString();
            switch (phase)
            {
                case inGame_str:
                    if (isInGame)
                    {
                        return;
                    }
                    EnterGame?.Invoke(null, null);
                    isInChampSelect = false;
                    isInGame = true;
                    Loggings.Logger.Log(Loggings.LogType.LCU, "Enter Game occured");
                    return;
                case inChampSelect_str:
                    if (isInChampSelect)
                    {
                        return;
                    }
                    EnterChampSelect?.Invoke(null, null);
                    isInChampSelect = true;
                    isInGame = false;
                    Loggings.Logger.Log(Loggings.LogType.LCU, "Enter ChampSelect occured");
                    return;
                default:
                    if (isInGame)
                    {
                        isInGame = false;
                        EndGame?.Invoke(null, null);
                        Loggings.Logger.Log(Loggings.LogType.LCU, "End Game occured");
                        return;
                    }
                    isInChampSelect = false;
                    return;
            }
        }
        #endregion
    }
}