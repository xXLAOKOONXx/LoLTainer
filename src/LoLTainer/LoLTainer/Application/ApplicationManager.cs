using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoLTainer.Interfaces;
using LoLTainer.Misc;
using LoLTainer.Models;

namespace LoLTainer
{
    public class ApplicationManager : Interfaces.IApplicationManager, IAppInformationProvider
    {
        #region Private Properties
        ISettingsManager _settingsManager;
        API.InGameApiManager _inGameApiManager;
        API.LCUManager _lCUManager;
        SoundPlayer.NAudioPlayer _nAudioPlayer;
        private EventHandler<Models.EventTriggeredEventArgs> eventHandler;
        #endregion
        #region Public Properties
        public EventActionSetting EventActionSetting
        {
            get => _settingsManager.EventActionSetting;
        }
        #endregion

        #region Constructors
        public ApplicationManager()
        {
            _settingsManager = new SettingsManager.SettingsManager();
            _lCUManager = new API.LCUManager();
            _inGameApiManager = new API.InGameApiManager();
            _nAudioPlayer = new SoundPlayer.NAudioPlayer();

            eventHandler += EventTriggered;
            eventHandler += InGameListener;

            foreach (var manager in EventAPIManagers)
            {
                manager.EventHandler += eventHandler;
            }

            _lCUManager.Connect();
            _nAudioPlayer.Connect();
        }

        #endregion

        #region EventHandling
        private async void EventTriggered(object sender, EventTriggeredEventArgs eventTriggeredEventArgs)
        {
            if (_settingsManager.EventActionSetting.Settings.ContainsKey(eventTriggeredEventArgs.Event))
            {
                var sets = _settingsManager.EventActionSetting.Settings[eventTriggeredEventArgs.Event];
                foreach (var item in sets)
                {
                    GetActionAPIManager(item.ActionManager).PerformAction(item, eventTriggeredEventArgs);
                }
            }
        }

        private void InGameListener(object sender, EventTriggeredEventArgs eventTriggeredEventArgs)
        {            
            if (eventTriggeredEventArgs.Event == Event.EnterGame)
            {
                Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Detected EnterGame, connecting to ingame");
                _inGameApiManager.Connect();
            }
            if (eventTriggeredEventArgs.Event == Event.EndGame)
            {
                _inGameApiManager.DisConnect();
                Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Detected EndGame, disconnecting from ingame");
            }
        }

        public IActionAPIManager GetActionAPIManager(ActionManager actionManager)
        {
            switch (actionManager)
            {
                case ActionManager.SoundPlayer:
                    return _nAudioPlayer;
                default:
                    return null;
            }
        }
        #endregion

        public IEnumerable<IActionAPIManager> ActionAPIManagers
        {
            get
            {
                yield return _nAudioPlayer;
            }
        }

        public IEnumerable<IEventAPIManager> EventAPIManagers
        {
            get
            {
                yield return _inGameApiManager;
                yield return _lCUManager;
            }
        }

        public IEnumerable<Event> AllAvailableEvents
        {
            get
            {
                foreach (var manager in EventAPIManagers)
                {
                    foreach (var ev in manager.GetSupportedEvents())
                    {
                        yield return ev;
                    }
                }
            }
        }

        public ILCUAPIInformationProvider LCUAPIInformationProvider => _lCUManager;

        public IIngameAPIInformationProvider IngameAPIInformationProvider => _inGameApiManager;

        public void OpenNewSetting(string fileName)
        {
            if (File.Exists(fileName))
            {
                _settingsManager.ReadFromFile(fileName);
            }
        }


        public IEnumerable<Misc.ActionManager> GetAvailableActionManagers()
        {
            yield return Misc.ActionManager.SoundPlayer;
        }

        public void SaveChanges()
        {
            _settingsManager.Save();
        }
    }
}
