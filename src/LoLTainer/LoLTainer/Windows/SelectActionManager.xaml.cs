using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LoLTainer.Windows
{
    /// <summary>
    /// Interaktionslogik für SelectActionManager.xaml
    /// </summary>
    public partial class SelectActionManager : Window
    {
        private Action<Misc.ActionManager?> _action;
        private bool _manualClose = true;

        public SelectActionManager()
        {
            this.Closing += OnWindowClose;
            InitializeComponent();
            DrawUISettings();
        }

        private JToken _uISettings;
        private void DrawUISettings()
        {
            try
            {
                var filePath = System.IO.Path.Combine(AppContext.BaseDirectory, "Settings", "UISettings.json");

                var jsontext = System.IO.File.ReadAllText(filePath);

                var uISettings = JObject.Parse(jsontext);

                _uISettings = uISettings["SelectActionManager"];
            }
            catch (Exception ex)
            {
                Loggings.Logger.Log(Loggings.LogType.UI, "Failed loading AddQueue UISettings from File, Error Message: " + ex.Message);
            }

            SetBackgroundFromSettings(this, "BackgroundColor");
        }

        public void Open(Action<Misc.ActionManager?> action, Interfaces.IApplicationManager applicationManager)
        {
            _action = action;
            var mngrs = applicationManager.GetAvailableActionManagers();
            if(mngrs.Count() == 0)
            {
                MessageBox.Show("No available Action Managers detected");
                action(null);
                return;
            }
            if(mngrs.Count() == 1)
            {
                action(mngrs.First());
                return;
            }
            foreach(var mng in mngrs)
            {
                BuildOption(mng);
            }

            this.Show();
        }

        private void BuildOption(Misc.ActionManager actionManager)
        {
            Button button = new Button();
            button.Margin = new Thickness(5);
            button.Padding = new Thickness(2);
            button.Width = 200;
            button.FontSize = 20;
            button.BorderThickness = new Thickness(0);
            button.Content = actionManager.ToString();
            button.Click += (s, o) => {
                _action(actionManager);
                _manualClose = false;
                this.Close();
            };
            STKActionManagers.Children.Add(button);
            SetBackgroundFromSettings(button, "BTNActionManagerBackgroundColor");
        }
        private void SetBackgroundFromSettings(Control control, string colorName)
        {

            Brush tmpBrush;
            try
            {
                tmpBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_uISettings[colorName].ToString()));
                control.Background = tmpBrush;
            }
            catch (Exception ex)
            {
                Loggings.Logger.Log(Loggings.LogType.UI, String.Format("Error getting Color from UISettings; Color:{0}, ErrorMessage:{1}", colorName, ex.Message));
            }
        }

        private void OnWindowClose(object s, object o)
        {
            if (_manualClose)
            {
                _action(null);
            }
        }
    }
}
