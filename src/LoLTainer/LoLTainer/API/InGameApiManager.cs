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
    public class InGameApiManager
    {
        public EventHandler<bool> OnConnectionStatusChange;
        public EventHandler<EventData> OnGameEvent;
        private string _mostRecentGameEventList;

        private object prevRequest;

        private const string _baseUrl = "https://127.0.0.1:2999";
        private const string _gameEventUrl = "/liveclientdata/eventdata";
        private const string _activePlayerUrl = "/liveclientdata/activeplayer";
        private const string _playerListUrl = "/liveclientdata/playerlist";

        private bool _gameActionCrawling = true;

        public InGameApiManager()
        {
            Task.Run(()=>
            GameActionLooper(TimeSpan.FromMilliseconds(200)));
        }

        public async Task<ActivePlayer> GetActivePlayer() => new ActivePlayer(await GetJObject(_activePlayerUrl));
        public async Task<PlayerList> GetPlayerList() => new PlayerList(await GetJArray(_playerListUrl));
        
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
            catch(Exception ex)
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
            Console.WriteLine("GameActionLooper started with a delay in seconds: " + delay.TotalSeconds);
            while (_gameActionCrawling)
            {
                try
                {

                await GameActionRequester();
                }catch(Exception ex)
                {
                    Console.WriteLine("EXCEPTION!!!");
                }
                await Task.Delay(delay);
            }
        }

        private async Task GameActionRequester()
        {
            Console.WriteLine("Iteration of GameActionCrawler");

            string URL = _baseUrl + _gameEventUrl;

            string response = GetHTTPResponse(URL);

            if(response != _mostRecentGameEventList)
            {
                Console.WriteLine("New Event occured!");

                _mostRecentGameEventList = response;

                var eventListRequest = JObject.Parse(response);

                this.OnGameEvent.Invoke(this, new EventData(eventListRequest));
            }
        }

        public void Close()
        {
            _gameActionCrawling = false;
        }
    }
}
