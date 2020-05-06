using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebSocketSharp;

namespace LoLTainer.API
{
    public class LCUManager
    {
        public EventHandler<bool> Connected;
        public EventHandler<bool> InGame;

        public LCUManager()
        {
            WebsocketMessageEventHandler += OnWebSocketMessage;
            GameFlowSessionEventHandler += OnGameFlowSession;
            InitiateClientConnection();
        }
        /// <summary>
        /// EventArgument = Websocket is active;
        /// </summary>
        private EventHandler<bool> WebSocketActivityChanged;
        private WebSocket _clientWebSocket;


        #region private constants
        private const string SummonerIconChangedEvent = "OnJsonApiEvent_lol-summoner_v1_current-summoner";
        private const string LoggedInEvent = "OnJsonApiEvent_lol-login_v1_login-data-packet";
        private const string QueueUpEvent = "OnJsonApiEvent_lol-lobby-team-builder_v1_lobby";
        private const string LobbyChangedEvent = "OnJsonApiEvent_lol-lobby_v2_lobby";
        private const string GameEvent = "OnJsonApiEvent_lol-gameflow_v1_session";

        static readonly string _authRegexPattern = @"""--remoting-auth-token=(?'token'.*?)"" | ""--app-port=(?'port'|.*?)""";
        static readonly RegexOptions _authRegexOptions = RegexOptions.Multiline;
        #endregion
        #region private properties
        private bool _connected = false;
        private bool _terminationRequested = false;
        private string _apiDomain;
        private AuthenticationHeaderValue _authHeader;
        private string _token;
        private string _port;

        private WebSocket _webSocket;
        #endregion
        #region EventHandler
        public EventHandler<MessageEventArgs> WebsocketMessageEventHandler { get; private set; }
        public EventHandler<JArray> SummonerIconChangedEventHandler { get; private set; }
        public EventHandler<JArray> LoggedInEventHandler { get; private set; }
        public EventHandler<JArray> QueueUpEventHandler { get; private set; }
        public EventHandler<JArray> LobbyChangedEventHandler { get; private set; }
        public EventHandler<JArray> GameFlowSessionEventHandler { get; private set; }
        #endregion

        private void OnGameFlowSession(object sender, JArray jArray)
        {
            var b = jArray[2]["data"]["phase"].ToString() == "InProgress";
            InGame.Invoke(this, b);
        } 

        private void OnWebSocketMessage(object sender, MessageEventArgs e)
        {
            var Messages = JArray.Parse(e.Data);

            int MessageType = 0;
            if (!int.TryParse(Messages[0].ToString(), out MessageType) || MessageType != 8)
                return;

            var EventName = Messages[1].ToString();

            Console.WriteLine("Event: " + EventName + " uri " + Messages[2]["uri"]);

            switch (EventName)
            {
                case SummonerIconChangedEvent:
                    SummonerIconChangedEventHandler.Invoke(sender, Messages);
                    break;
                case LoggedInEvent:
                    //LoggedInEventHandler.Invoke(sender, Messages);
                    break;
                case QueueUpEvent:
                    //QueueUpEventHandler.Invoke(sender, Messages);
                    break;
                case LobbyChangedEvent:
                    //LobbyChangedEventHandler.Invoke(sender, Messages);
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

        private void SetUpConnection()
        {
            string port;
            string token;
            GetAuth(out port, out token);


            var BaseString = string.Format("{0}:{1}", "riot", token);
            var Base64Data = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(BaseString));
            _authHeader = new AuthenticationHeaderValue("Basic", Base64Data);

            var wb = new WebSocket("wss://127.0.0.1:" + port + "/", "wamp");

            wb.OnClose += OnWebSocketClose;


            wb.SetCredentials("riot", token, true);
            wb.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
            wb.OnMessage += WebsocketMessageEventHandler;

            wb.Connect();

            //wb.Send("[5,\"" + SummonerIconChangedEvent + "\"]");
            // wb.Send("[5,\"" + QueueUpEvent + "\"]");
            //  wb.Send("[5,\"" + LobbyChangedEvent + "\"]");
            wb.Send("[5,\"" + GameEvent + "\"]");
            //  wb.Send("[5,\"" + LoggedInEvent + "\"]");

            _webSocket = wb;
            WebSocketActivityChanged.Invoke(this, true);

            _token = token;
            _port = port;
            _apiDomain = String.Format("https://127.0.0.1:{0}", port);

        }

        private void OnWebSocketClose(object sender, CloseEventArgs closeEventArgs)
        {
            WebSocketActivityChanged(this, false);
            InitiateClientConnection();
        }


        private async Task InitiateClientConnection()
        {
            string port;
            string token;
            var successfulGetAuth = false;
            while (!successfulGetAuth)
            {
                try
                {
                    GetAuth(out port, out token);
                    successfulGetAuth = true;
                }
                catch (Exception e)
                {
                    await Task.Delay(10000);
                }
            }
            try
            {
                SetUpConnection();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
