using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebSocketSharp;

namespace LoLTainer.API
{
    public class LCUManager : INotifyPropertyChanged
    {
        public EventHandler<bool> Connected;
        public EventHandler<bool> InGame;
        public bool IsConnected
        {
            get;
            private set;
        }

        /// <summary>
        /// ID of the summoner icon the current summoner has equiped
        /// </summary>
        public int CurrentSummonerIconId
        {
            get => _currentSummonerIconId; internal set
            {
                _currentSummonerIconId = value;
                NotifyPropertyChanged("CurrentSummonerIconId");
            }
        }
        /// <summary>
        /// Display summoner name of the current summoner
        /// </summary>
        public string CurrentSummonerName
        {
            get => _currentSummonerName; internal set
            {
                _currentSummonerName = value;
                NotifyPropertyChanged("CurrentSummonerName");
            }
        }
        /// <summary>
        /// Summoner id of the current summoner.
        /// </summary>
        public string CurrentSummonerId
        {
            get => _currentSummonerId; internal set
            {
                _currentSummonerId = value;
                NotifyPropertyChanged("CurrentSummonerId");
            }
        }

        public LCUManager()
        {
            Connected += (o, b) => IsConnected = b;

            Loggings.Logger.Log(Loggings.LogType.LCU,"Setting up LCU Manager");
            WebsocketMessageEventHandler += OnWebSocketMessage;
            GameFlowSessionEventHandler += OnGameFlowSession;
            InitiateClientConnection();
            Loggings.Logger.Log(Loggings.LogType.LCU, "LCU Manager set up");
            SummonerChangedEventHandler += OnSummonerChanged;
        }
        /// <summary>
        /// EventArgument = Websocket is active;
        /// </summary>
        private EventHandler<bool> WebSocketActivityChanged;
        private WebSocket _clientWebSocket;


        #region private constants
        private const string GameEvent = "OnJsonApiEvent_lol-gameflow_v1_session";
        /* Below EndPoints are not in use yet, as they might be relevant in future they are listed here.
        private const string QueueUpEvent = "OnJsonApiEvent_lol-lobby-team-builder_v1_lobby";
        private const string LobbyChangedEvent = "OnJsonApiEvent_lol-lobby_v2_lobby";
        */
        private const string SummonerIconChangedEvent = "OnJsonApiEvent_lol-summoner_v1_current-summoner";
        private const string LoggedInEvent = "OnJsonApiEvent_lol-login_v1_login-data-packet";
        static readonly string _authRegexPattern = @"""--remoting-auth-token=(?'token'.*?)"" | ""--app-port=(?'port'|.*?)""";
        static readonly RegexOptions _authRegexOptions = RegexOptions.Multiline;
        #endregion
        #region private properties
        private string _apiDomain;
        private AuthenticationHeaderValue _authHeader;
        private string _token;
        private string _port;

        private int _currentSummonerIconId;
        private string _currentSummonerName;
        private string _currentSummonerId;

        private WebSocket _webSocket;
        #endregion
        #region EventHandler
        private EventHandler<MessageEventArgs> WebsocketMessageEventHandler { get; set; }
        public EventHandler<JArray> GameFlowSessionEventHandler { get; set; }
        /* Below EndPoints are not in use yet, as they might be relevant in future they are listed here.
        public EventHandler<JArray> LoggedInEventHandler { get; private set; }
        public EventHandler<JArray> QueueUpEventHandler { get; private set; }
        public EventHandler<JArray> LobbyChangedEventHandler { get; private set; }
        */
        public EventHandler<JArray> SummonerChangedEventHandler { get; private set; }
        #endregion

        private void OnGameFlowSession(object sender, JArray jArray)
        {
            var b = jArray[2]["data"]["phase"].ToString() == "InProgress";
            InGame.Invoke(this, b);
            Loggings.Logger.Log(Loggings.LogType.LCU, "GameFlowSession message: " + (b?"InGame":"Not InGame"));
        } 

        private void OnWebSocketMessage(object sender, MessageEventArgs e)
        {
            Loggings.Logger.Log(Loggings.LogType.LCU, "WebSocket Message received");
            var Messages = JArray.Parse(e.Data);

            int MessageType = 0;
            if (!int.TryParse(Messages[0].ToString(), out MessageType) || MessageType != 8)
                return;

            var EventName = Messages[1].ToString();

            // Console.WriteLine("Event: " + EventName + " uri " + Messages[2]["uri"]);

            switch (EventName)
            {
                /* Below EndPoints are not in use yet, as they might be relevant in future they are listed here.
                case QueueUpEvent:
                    //QueueUpEventHandler.Invoke(sender, Messages);
                    break;
                case LobbyChangedEvent:
                    //LobbyChangedEventHandler.Invoke(sender, Messages);
                    break;
                case LoggedInEvent:
                    //LoggedInEventHandler.Invoke(sender, Messages);
                    break;
                    */
                case SummonerIconChangedEvent:
                    SummonerChangedEventHandler.Invoke(sender, Messages);
                    break;
                case GameEvent:
                    GameFlowSessionEventHandler.Invoke(sender, Messages);
                    break;
                default:
                    break;
            }
        }
        private static void GetAuth(out string Port, out string Token)
        {
            String token = "";
            String port = "";
            var mngmt = new ManagementClass("Win32_Process");
            foreach (ManagementObject o in mngmt.GetInstances())
            {
                if (o["Name"].Equals("LeagueClientUx.exe"))
                {
                    //Console.WriteLine(o["CommandLine"]);


                    foreach (Match m in Regex.Matches(o["CommandLine"].ToString(), _authRegexPattern, _authRegexOptions))
                    {
                        if (!String.IsNullOrEmpty(m.Groups["port"].ToString()))
                        {
                            port = m.Groups["port"].ToString();
                        }
                        else if (!String.IsNullOrEmpty(m.Groups["token"].ToString()))
                        {
                            token = m.Groups["token"].ToString();
                        }
                    }
                    //return o["CommandLine"].GetType().ToString();
                }
            }
            if (String.IsNullOrEmpty(token) || String.IsNullOrEmpty(port))
            {
                throw new Exception("No League client found");
            }

            Token = token;
            Port = port;

        }

        private void SetUpConnection(string port, string token)
        {
            var BaseString = string.Format("{0}:{1}", "riot", token);
            var Base64Data = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(BaseString));
            _authHeader = new AuthenticationHeaderValue("Basic", Base64Data);

            var wb = new WebSocket("wss://127.0.0.1:" + port + "/", "wamp");

            wb.OnClose += OnWebSocketClose;


            wb.SetCredentials("riot", token, true);
            wb.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            wb.OnMessage += WebsocketMessageEventHandler;

            wb.Connect();

            // Subscribe to GameEvents
            wb.Send("[5,\"" + GameEvent + "\"]");

            /* Below EndPoints are not in use yet, as they might be relevant in future they are listed here.
            wb.Send("[5,\"" + QueueUpEvent + "\"]");
            wb.Send("[5,\"" + LobbyChangedEvent + "\"]");
            wb.Send("[5,\"" + LoggedInEvent + "\"]");
            */
            wb.Send("[5,\"" + SummonerIconChangedEvent + "\"]");

            _webSocket = wb;
            WebSocketActivityChanged?.Invoke(this, true);

            _token = token;
            _port = port;
            _apiDomain = String.Format("https://127.0.0.1:{0}", port);

        }

        private void OnWebSocketClose(object sender, CloseEventArgs closeEventArgs)
        {
            WebSocketActivityChanged?.Invoke(this, false);
            Connected?.Invoke(this, false);
            InitiateClientConnection();
        }


        private async Task InitiateClientConnection()
        {
            string port = "";
            string token = "";
            var successfulGetAuth = false;
            while (!successfulGetAuth)
            {
                try
                {
                    Loggings.Logger.Log(Loggings.LogType.LCU, "Get LCU port and token");
                    GetAuth(out port, out token);
                    successfulGetAuth = true;
                    Loggings.Logger.Log(Loggings.LogType.LCU, String.Format("LCU Port: {0}, Token: {1}",port,token));
                }
                catch (Exception e)
                {
                    // No Client found => retry in a few seconds
                    await Task.Delay(10000);
                }
            }
            try
            {
                SetUpConnection(port:port,token:token);
                Loggings.Logger.Log(Loggings.LogType.LCU, "LCU Connection established");
                Connected?.Invoke(this, true);
                UpdateSummonerInformation();
            }
            catch (Exception ex)
            {

            }
        }
        private void UpdateSummonerInformation(JToken jToken)
        {
            try
            {
                this.CurrentSummonerIconId = int.Parse(jToken["profileIconId"].ToString());
                this.CurrentSummonerId = jToken["summonerId"].ToString();
                this.CurrentSummonerName = jToken["displayName"].ToString();
            }
            catch (Exception ex)
            {
                Loggings.Logger.Log(Loggings.LogType.LCU, "Error occured updating Summoner Information with JToken: " + ex.Message);
            }
        }
        private void OnSummonerChanged(object sender, JArray e)
        {
            UpdateSummonerInformation(e[2]["data"]);
        }
        private void UpdateSummonerInformation()
        {
            string partialUrl = "/lol-summoner/v1/current-summoner";
            string body = "";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => { return true; };


            RestClient restClient = new RestClient(_apiDomain);
            restClient.Authenticator = new HttpBasicAuthenticator("riot", _token);
            RestRequest request = new RestRequest(partialUrl, Method.GET);

            var response = restClient.Execute(request).Content.ToString();

            var jArray = JObject.Parse(response);

            this.CurrentSummonerIconId = int.Parse(jArray["profileIconId"].ToString());
            this.CurrentSummonerId = jArray["summonerId"].ToString();
            this.CurrentSummonerName = jArray["displayName"].ToString();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
