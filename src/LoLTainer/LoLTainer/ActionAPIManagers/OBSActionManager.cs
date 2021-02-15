using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LoLTainer.Interfaces;
using LoLTainer.Models;
using OBSWebsocketDotNet;

namespace LoLTainer.ActionAPIManagers
{
    public class OBSActionManager : BaseActionAPIManager
    {
        private OBSWebsocket _oBSWebsocket;
        private string _url = "ws://localhost:4444";
        private string _psw = "";

        public string Url
        {
            get => _url;
            set
            {
                _url = value;
            }
        }
        public string Psw
        {
            get => _psw;
            set
            {
                _psw = value;
            }
        }

        public OBSActionManager()
        {
            _oBSWebsocket = new OBSWebsocket();
            _oBSWebsocket.Connected += (o, s) =>
            {
                Connected = true;
            };
            _oBSWebsocket.Disconnected += (o, s) =>
            {
                Connected = false;
            };
        }

        public override void Connect()
        {
            try
            {
                _oBSWebsocket.Connect(_url, _psw);
            }
            catch (Exception ex)
            {
                Loggings.Logger.Log(Loggings.LogType.OBS, String.Format("Exception connecting to OBS WebSocket; Message: {0}", ex.Message));
                MessageBox.Show("You need to use default websocket setup right now:\rurl: ws://localhost:4444\rand set no password\r btw the ui is bugged, OBS is not on yet, press twice on the button to retry.");
            }
        }

        public override void DisConnect()
        {
            _oBSWebsocket.Disconnect();
        }

        public override IActionWindow GetActionWindow()
        {
            return new Windows.SetOBSSetting();
        }

        public override bool IsValidPropertyBundle(PropertyBundle propertyBundle)
        {
            throw new NotImplementedException();
        }

        public override void PerformAction(PropertyBundle propertyBundle, EventTriggeredEventArgs eventTriggeredEventArgs = null)
        {
            if (!Connected)
            {
                return;
            }

            var bundle = new Services.PropertyBundleTranslator.OBSPropertyBundle(propertyBundle);

            switch (bundle.ActionType)
            {
                case Misc.OBSActionType.Scene:
                    PerformSceneSwitch(bundle);
                    break;
                case Misc.OBSActionType.ItemVisibility:
                    PerformItemVisibility(bundle);
                    break;
                case Misc.OBSActionType.FilterVisibility:
                    PerformSceneSwitch(bundle);
                    break;
            }
        }

        private async void PerformSceneSwitch(Services.PropertyBundleTranslator.OBSPropertyBundle oBSPropertyBundle)
        {
            var prevSceneName = _oBSWebsocket.GetCurrentScene().Name;
            _oBSWebsocket.SetCurrentScene(oBSPropertyBundle.SceneName);
            if (oBSPropertyBundle.ActionDuration != null && oBSPropertyBundle.ActionDuration >= TimeSpan.FromMilliseconds(0))
            {
                await Task.Delay((TimeSpan)oBSPropertyBundle.ActionDuration);
                _oBSWebsocket.SetCurrentScene(prevSceneName);
            }
        }

        private async void PerformItemVisibility(Services.PropertyBundleTranslator.OBSPropertyBundle oBSPropertyBundle)
        {
            var itemName = oBSPropertyBundle.ItemName;
            var sceneName = oBSPropertyBundle.SceneName;
            var props = _oBSWebsocket.GetSceneItemProperties(itemName, sceneName);
            bool targetValue;
            if (oBSPropertyBundle.VisibilityToggleOnly)
            {
                targetValue = !props.Visible;
            }
            else
            {
                targetValue = oBSPropertyBundle.VisibilityTargetValue;
            }
            props.Visible = targetValue;
            _oBSWebsocket.SetSceneItemProperties(props);
            if (oBSPropertyBundle.ActionDuration != null && oBSPropertyBundle.ActionDuration >= TimeSpan.FromMilliseconds(0))
            {
                await Task.Delay((TimeSpan)oBSPropertyBundle.ActionDuration);
                props.Visible = !targetValue;
                _oBSWebsocket.SetSceneItemProperties(props);
            }
        }

        private async void PerformFilterVisibility(Services.PropertyBundleTranslator.OBSPropertyBundle oBSPropertyBundle)
        {
            var itemName = oBSPropertyBundle.ItemName;
            var filterName = oBSPropertyBundle.FilterName;
            var props = _oBSWebsocket.GetSourceFilterInfo(itemName, filterName);
            bool targetValue;
            if (oBSPropertyBundle.VisibilityToggleOnly)
            {
                targetValue = !props.IsEnabled;
            }
            else
            {
                targetValue = oBSPropertyBundle.VisibilityTargetValue;
            }
            _oBSWebsocket.SetSourceFilterVisibility(itemName, filterName, targetValue);
            if (oBSPropertyBundle.ActionDuration != null && oBSPropertyBundle.ActionDuration >= TimeSpan.FromMilliseconds(0))
            {
                await Task.Delay((TimeSpan)oBSPropertyBundle.ActionDuration);
                _oBSWebsocket.SetSourceFilterVisibility(itemName, filterName, !targetValue);
            }
        }
    }
}
