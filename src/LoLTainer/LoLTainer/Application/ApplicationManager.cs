using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LoLTainer.Interfaces;
using LoLTainer.Misc;
using LoLTainer.Models;

namespace LoLTainer
{
    public class ApplicationManager : Interfaces.IApplicationManager, IAppInformationProvider, INotifyPropertyChanged
    {
        #region Private Properties
        ISettingsManager _settingsManager;
        API.InGameApiManager _inGameApiManager;
        API.LCUManager _lCUManager;
        SoundPlayer.NAudioPlayer _nAudioPlayer;
        private ActionAPIManagers.OBSActionManager _oBSActionManager;
        private EventHandler<Models.EventTriggeredEventArgs> eventHandler;
        private bool _appOn = true;
        private bool _obsOn = false;
        #endregion
        #region Public Properties
        public bool AppOn
        {
            get => _appOn;
            set
            {
                _appOn = value;
                ApplyAppOnOff(_appOn);
                NotifyPropertyChanged();
            }
        }

        public bool OBSOn
        {
            get => _obsOn;
            set
            {
                _obsOn = value;
                ApplyOBSOnOff(_obsOn);
                NotifyPropertyChanged();
            }
        }


        private void ApplyOBSOnOff(bool obsOn)
        {
            if (obsOn)
            {
                _oBSActionManager.Connect();
            }
            else
            {
                _oBSActionManager.DisConnect();
            }
        }
        private void ApplyAppOnOff(bool appOn)
        {
            if (appOn)
            {
                _lCUManager.Connect();
                _nAudioPlayer.Connect();
            }
            else
            {
                _lCUManager.DisConnect();
                _inGameApiManager.DisConnect();
                _nAudioPlayer.DisConnect();
            }
        }

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
            _oBSActionManager = new ActionAPIManagers.OBSActionManager();

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
                case ActionManager.OBS:
                    return _oBSActionManager;
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
            yield return ActionManager.OBS;
        }

        public void SaveChanges()
        {
            foreach(var manager in EventAPIManagers)
            {
                manager.SetActiveEvents(_settingsManager.EventActionSetting.Settings.Keys.AsEnumerable());
            }
            _settingsManager.Save();
        }

        #region INotifyPropertyChanged implementation
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
