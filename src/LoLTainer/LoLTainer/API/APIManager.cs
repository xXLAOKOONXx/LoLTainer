using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using WebSocketSharp;

namespace LoLTainer.API
{

    public class APIManager : Interfaces.IAPIManager, INotifyPropertyChanged
    {
        static APIManager _activeManager = null;
        /// <summary>
        /// Returns the latest build API Manager to gain access to APIs like SoundPlayer
        /// </summary>
        /// <returns></returns>
        public static APIManager GetActiveManager()
        {
            return _activeManager;
        }

        /// <summary>
        /// SoundPlayer used by APIManager
        /// </summary>
        public Interfaces.ISoundPlayer SoundPlayer { get => _soundPlayer; }

        public string APIConnectionMessage
        {
            get => _aPIConnectionMessage;
            set
            {
                _aPIConnectionMessage = value;
                OnPropertyChanged("APIConnectionMessage");
            }
        }
        #region const
        private const string LCUNotConnected = "Not Connected";
        private const string LCUConnected = "Connected to Client";
        #endregion
        #region private properties
        private Interfaces.ISettingsManager _settingsManager;
        private Interfaces.ISoundPlayer _soundPlayer;
        private LCUManager _lCUManager;
        private LCUEventMapper _lCUEventMapper;
        private InGameApiManager _inGameApiManager;
        private InGameEventMapper _inGameEventMapper;
        private string _aPIConnectionMessage = LCUNotConnected;
        #endregion

        /// <summary>
        /// Constructor of <see cref="APIManager"/>
        /// </summary>
        /// <param name="settingsManager"><see cref="Interfaces.ISettingsManager"/> to draw the Settings from</param>
        public APIManager(Interfaces.ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _soundPlayer = new SoundPlayer.NAudioPlayer();

            _lCUManager = new LCUManager();
            _lCUManager.Connected += OnLCUConnectionChange;
            _lCUManager.InGame += OnIngameChange;
            if (_lCUManager.IsConnected)
            {
                OnLCUConnectionChange(this, true);
            }

            _lCUEventMapper = new LCUEventMapper(_lCUManager);
            MakeLCUMapping();

            _activeManager = this;
        }

        #region IAPIManager Implementation
        public Binding APIConnectionMessageBinding()
        {
            var bnd = new Binding("APIConnectionMessage");
            bnd.Source = this;
            return bnd;
        }

        public void SetInGameAPIOnOff(bool active)
        {
            throw new NotImplementedException();
        }

        public Binding SummonerIconBinding()
        {
            throw new NotImplementedException();
        }

        public Binding SummonerNameBinding()
        {
            var bnd = new Binding("CurrentSummonerName");
            bnd.Source = _lCUManager;
            return bnd;
        }
        #endregion

        /// <summary>
        /// Listener for changes on the State of being ingame.
        /// Opens / Closes InGameApiManager
        /// </summary>
        /// <param name="sender">can be null</param>
        /// <param name="inGame">true if ingame</param>
        private void OnIngameChange(object sender, bool inGame)
        {
            if (inGame && _inGameApiManager == null)
            {
                _inGameApiManager = new InGameApiManager();
                _inGameEventMapper = new InGameEventMapper(_inGameApiManager);
                MakeIngameMapping();
            }
            else
            if (!inGame && _inGameApiManager != null)
            {
                _inGameApiManager.Close();
                _inGameApiManager = null;
                _inGameEventMapper = null;
            }
        }
        /// <summary>
        /// Listener for changes on the State of LCU Manager being connected to Client.
        /// Updates Connection Message.
        /// </summary>
        /// <param name="sender">can be null</param>
        /// <param name="inGame">true if ingame</param>
        private void OnLCUConnectionChange(object sender, bool connected)
        {
            if (connected)
            {
                APIConnectionMessage = LCUConnected;
            }
            else
            {
                APIConnectionMessage = LCUNotConnected;
            }
        }

        private void MakeIngameMapping()
        {
            foreach (var setting in _settingsManager.GetAllSettings())
            {
                _inGameEventMapper.GetEventHandler(setting.Event) += (s, e) =>
                {
                    _soundPlayer.PlaySound(
                        playerId: setting.SoundPlayerGroup,
                        fileName: setting.FileName,
                        startTime: setting.StartTime,
                        playLength: setting.PlayLength,
                        volume: setting.Volume,
                        playMode: setting.PlayMode);
                };
                Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Costum-InGame-Eventlistener set up: " + setting.Event.ToString());
            }
        }

        private void MakeLCUMapping()
        {
            foreach (var setting in _settingsManager.GetAllSettings())
            {
                _lCUEventMapper.GetEventHandler(setting.Event) += (s, e) =>
                {
                    _soundPlayer.PlaySound(
                        playerId: setting.SoundPlayerGroup,
                        fileName: setting.FileName,
                        startTime: setting.StartTime,
                        playLength: setting.PlayLength,
                        volume: setting.Volume,
                        playMode: setting.PlayMode);
                };
                Loggings.Logger.Log(Loggings.LogType.LCU, "Costum-LCU-Eventlistener set up: " + setting.Event.ToString());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
