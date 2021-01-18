using LoLTainer.API.Models.InGameAPI;
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
    /// Triggers public EventHandlers when they occure.
    /// Current Events supported: <see cref="InGameApiManager.OnGameEvent"/>
    /// </summary>
    public class InGameApiManager : IdentifiableObject
    {
        public EventHandler<bool> OnConnectionStatusChange;
        public EventHandler<EventData> OnGameEvent;

        #region API Constants
        private const string _baseUrl = "https://127.0.0.1:2999";
        private const string _gameEventUrl = "/liveclientdata/eventdata";
        private const string _activePlayerUrl = "/liveclientdata/activeplayer";
        private const string _playerListUrl = "/liveclientdata/playerlist";
        #endregion

        #region private properties
        #region GameActionCrawler properties
        /// <summary>
        /// Defines whether the gameActionCrawling should be done or not, once set on false the next iteration will break the loop. setting to true again will not reactivate the loop.
        /// </summary>
        private bool _gameActionCrawling = true;
        private string _mostRecentGameEventList;
        #endregion


        #endregion

        /// <summary>
        /// Constructor of <see cref="InGameApiManager"/>. Once initiated a loop fetching game action every 200ms starts.
        /// </summary>
        public InGameApiManager() : base()
        {
            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "InGameApiManager started");
            Task.Run(() =>
            GameActionLooper(TimeSpan.FromMilliseconds(200)));
        }

        #region Endpoint Rquests
        public async Task<ActivePlayer> GetActivePlayer() => new ActivePlayer(await GetJObject(_activePlayerUrl));
        public async Task<PlayerList> GetPlayerList() => new PlayerList(await GetJArray(_playerListUrl));
        #endregion

        private string GetHTTPResponse(string url)
        {
            try
            {
                string response;

                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; }; // automatically trust the certificate
                using (HttpClient Client = new HttpClient())
                {
                    HttpResponseMessage Response = Client.GetAsync(url).Result;
                    Response.EnsureSuccessStatusCode();

                    response = Response.Content.ReadAsStringAsync().Result;
                }

                return response;
            }
            catch (Exception ex)
            {
                Task.Delay(1000).Wait();
                if (!_gameActionCrawling)
                {
                    return null;
                }
                return GetHTTPResponse(url);
            }
        }
        private async Task<JObject> GetJObject(string partialUrl)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; }; // automatically trust the certificate

            string URL = _baseUrl + partialUrl;

            string response = GetHTTPResponse(URL);

            return JObject.Parse(response);
        }
        private async Task<JArray> GetJArray(string partialUrl)
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; }; // automatically trust the certificate

            string URL = _baseUrl + partialUrl;

            string response = GetHTTPResponse(URL);

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
                }
                catch (Exception ex)
                {
                    Loggings.Logger.Log(Loggings.LogType.IngameAPI, String.Format("!!! Exception cought in GameActionLooper (InGameApiManager {0}), loop continue anyway, Exception: {1}", base.Id, ex.Message));
                }
                await Task.Delay(delay);
            }
        }

        private async Task GameActionRequester()
        {
            Console.WriteLine("Iteration of GameActionCrawler");

            string URL = _baseUrl + _gameEventUrl;

            string response = GetHTTPResponse(URL);

            if (!_gameActionCrawling)
            {
                return;
            }

            if (response != _mostRecentGameEventList)
            {
                Loggings.Logger.Log(Loggings.LogType.IngameAPI, "New InGame Event occured", base.Id);

                _mostRecentGameEventList = response;

                var eventListRequest = JObject.Parse(response);

                this.OnGameEvent.Invoke(this, new EventData(eventListRequest));
            }
        }

        /// <summary>
        /// Stops all crawling activity within this class.
        /// </summary>
        public void Close()
        {
            _gameActionCrawling = false;
            OnGameEvent = null;
            Loggings.Logger.Log(Loggings.LogType.IngameAPI, "Closing InGameApiManager",base.Id);
        }
    }
}
