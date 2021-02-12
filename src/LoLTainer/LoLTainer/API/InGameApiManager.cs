using LoLTainer.API.Models.InGameAPI;
using LoLTainer.Misc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.API
{
    /// <summary>
    /// Connection to InGame API and observer of InGameActivities.
    /// Triggers public EventHandler when they occure.
    /// With the help of <see cref="InGameEventMapper"/> translates the Activities into <see cref="Event"/> and triggers EventHandler with it.
    /// </summary>
    public class InGameApiManager : EventAPIManagers.BaseEventAPIManager, Interfaces.IIngameAPIInformationProvider
    {
        public EventHandler<bool> OnConnectionStatusChange;
        public EventHandler<EventData> OnGameEvent;

        #region API Constants
        private const string _baseUrl = "https://127.0.0.1:2999";
        private const string _gameEventUrl = "/liveclientdata/eventdata";
        private const string _activePlayerUrl = "/liveclientdata/activeplayer";
        private const string _playerListUrl = "/liveclientdata/playerlist";
        private const string _allGameDataUrl = "/liveclientdata/allgamedata";
        #endregion

        #region private properties
        #region GameActionCrawler properties
        /// <summary>
        /// Defines whether the gameActionCrawling should be done or not, once set on false the next iteration will break the loop. setting to true again will not reactivate the loop.
        /// </summary>
        private bool _gameActionCrawling = false;
        private string _mostRecentGameEventList;
        private InGameEventMapper _inGameEventMapper;
        private TimeSpan OnNoResponseDelayTime = TimeSpan.FromMilliseconds(1000);

        public string ChampionName
        {
            get
            {
                return this.GetActivePlayer().Result.SummonerName;
            }
        }
        #endregion


        #endregion

        /// <summary>
        /// Constructor of <see cref="InGameApiManager"/>. Once initiated a loop fetching game action every 200ms starts.
        /// </summary>
        public InGameApiManager() : base()
        {
            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "InGameApiManager started");
            _inGameEventMapper = new InGameEventMapper(this);
        }

        #region Endpoint Rquests
        public async Task<ActivePlayer> GetActivePlayer(int retrys = 5)
        {
            try
            {
                return new ActivePlayer(await GetJObject(_activePlayerUrl));
            }
            catch (Exception ex)
            {
                return new ActivePlayer(null);
            }
        }
        public async Task<PlayerList> GetPlayerList() => new PlayerList(await GetJArray(_playerListUrl));
        #endregion

        private async Task<string> GetHTTPResponse(string url, int retrys = -1)
        {
            try
            {
                string response;

                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; }; // automatically trust the certificate
                using (HttpClient Client = new HttpClient())
                {
                    HttpResponseMessage Response = await Client.GetAsync(url);
                    Response.EnsureSuccessStatusCode();

                    response = Response.Content.ReadAsStringAsync().Result;
                }

                return response;
            }
            catch (Exception ex)
            {
                Connected = false;
                _inGameEventMapper.PotentialNewGame = true;
                Task.Delay(OnNoResponseDelayTime).Wait();
                if (!_gameActionCrawling || retrys == 0)
                {
                    return null;
                }
                return await GetHTTPResponse(url, retrys - 1);
            }
        }
        private async Task<JObject> GetJObject(string partialUrl)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; }; // automatically trust the certificate

            string URL = _baseUrl + partialUrl;

            string response = await GetHTTPResponse(URL);

            if (response == null)
            {
                return null;
            }

            return JObject.Parse(response);
        }
        private async Task<JArray> GetJArray(string partialUrl)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; }; // automatically trust the certificate

            string URL = _baseUrl + partialUrl;

            string response = await GetHTTPResponse(URL);

            if (response == null)
            {
                return null;
            }

            return JArray.Parse(response);
        }

        private async Task GameActionLooper(TimeSpan delay)
        {
            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "GameActionLooper started with a delay in seconds: " + delay.TotalSeconds, base.Id);
            while (_gameActionCrawling)
            {
                try
                {
                    await GameActionRequester();
                    Connected = true;
                }
                catch (Exception ex)
                {
                    Loggings.Logger.Log(Loggings.LogType.IngameAPI, String.Format("!!! Exception cought in GameActionLooper (InGameApiManager {0}), loop continue anyway, Exception: {1}", base.Id, ex.Message));
                }
                await Task.Delay(delay);
            }
            Connected = false;
            _inGameEventMapper.PotentialNewGame = true;
        }

        private async Task GameActionRequester()
        {
            Console.WriteLine("Iteration of GameActionCrawler");

            string URL = _baseUrl + _gameEventUrl;
            string response;
            try
            {
                response = await GetHTTPResponse(URL);
            }
            catch (Exception ex)
            {
                Connected = false;
                _inGameEventMapper.PotentialNewGame = true;
                throw ex;
            }

            if (!_gameActionCrawling)
            {
                return;
            }

            if (response != _mostRecentGameEventList)
            {
                Loggings.Logger.Log(Loggings.LogType.IngameAPI, "New InGame Event occured", base.Id);

                _mostRecentGameEventList = response;

                var eventListRequest = JObject.Parse(response);

                this.OnGameEvent?.Invoke(this, new EventData(eventListRequest));
            }
        }

        /// <summary>
        /// Stops all crawling activity within this class.
        /// </summary>
        public void Close()
        {
            _gameActionCrawling = false;
            OnGameEvent = null;
            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Closing InGameApiManager", base.Id);
        }

        public override void Connect()
        {
            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Connecting with inGame API", base.Id);
            Loggings.Logger.Log(Loggings.LogType.IngameAPI, String.Format("Presets are Connected:{0} Crawling:{1}",this.Connected,_gameActionCrawling), base.Id);
            if (!this.Connected && !_gameActionCrawling)
            {
                this._gameActionCrawling = true;
                GameActionLooper(TimeSpan.FromMilliseconds(200));
            }
        }

        public override void DisConnect()
        {
            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Disconnecting with inGame API", base.Id);
            _gameActionCrawling = false;
        }

        public override IEnumerable<Event> GetSupportedEvents()
        {
            // GameAccurances
            //yield return Event.GameStart; //TODO: Implement GameStartEvent Trigger
            //yield return Event.NexusFall; //TODO: Implement NexusFall Trigger + CHeck how accurate it is
            // enemynexus / teamnexus


            // Objectivekills
            yield return Event.PlayerBaronKill;
            yield return Event.PlayerDragonKill;
            yield return Event.PlayerDragonSteal;
            yield return Event.PlayerBaronSteal;

            yield return Event.PlayerAnyKill;
            yield return Event.PlayerFirstBlood;
            // Multikills
            yield return Event.PlayerSingleKill;
            yield return Event.PlayerDoubleKill;
            yield return Event.PlayerTripleKill;
            yield return Event.PlayerQuodraKill;
            yield return Event.PlayerPentaKill;
        }
    }
}
