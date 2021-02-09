using LoLTainer.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoLTainer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private JToken _uISettings;
        private ApplicationManager _applicationManager;


        public MainWindow()
        {
            InitializeComponent();
            _applicationManager = new ApplicationManager();

            DrawUISettings();
            DrawList();
            SetBindings();
        }

        private void SetBindings()
        {
            /*
             * TODO
            LBLClientStatus.SetBinding(ContentProperty, _aPIManager.APIConnectionMessageBinding());
            */
            var bnd = new Binding("CurrentSummonerName");
            bnd.Source = _applicationManager.LCUAPIInformationProvider;
            LBLSummonerName.SetBinding(ContentProperty, bnd);
        }

        private void DrawUISettings()
        {
            var filePath = System.IO.Path.Combine(AppContext.BaseDirectory, "Settings", "UISettings.json");

            var jsontext = System.IO.File.ReadAllText(filePath);

            var uISettings = JObject.Parse(jsontext);

            _uISettings = uISettings["MainWindow"];

            SetBackgroundFromSettings(this, "BackgroundColor");
            SetBackgroundFromSettings(BTNAddEvent, "BTNAddEventBackgroundColor");

            Brush tmpBrush;
            try
            {
                tmpBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_uISettings["TXTBLKRiotLegalBackgroundColor"].ToString()));
                TXTBLKRiotLegal.Background = tmpBrush;
            }
            catch (Exception ex)
            {
                Loggings.Logger.Log(Loggings.LogType.UI, String.Format("Error getting Color from UISettings; Color:{0}, ErrorMessage:{1}", "TXTBLKRiotLegalBackgroundColor", ex.Message));
            }
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

        private void DrawList()
        {
            SettingsPanel.Children.Clear();

            foreach (var item in _applicationManager.EventActionSetting.Settings)
            {
                SettingsPanel.Children.Add(GetUIElement(item));
            }
        }

        private UIElement GetUIElement(KeyValuePair<Misc.Event, List<Models.PropertyBundle>> setting)
        {
            var magicWrap = new StackPanel();
            var head = new Button();
            head.Content = setting.Key.ToString();
            magicWrap.Children.Add(head);

            var body = new Grid();
            body.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_uISettings["QueueBodyBackgroundColor"].ToString()));
            body.ColumnDefinitions.Add(new ColumnDefinition());
            body.ColumnDefinitions.Add(new ColumnDefinition());
            body.ColumnDefinitions.Add(new ColumnDefinition());
            

            // Edit Button
            var btn = new Button();
            btn.Content = "Edit";
            Grid.SetColumn(btn, 1);
            btn.Click += (s, e) =>
            {
                ChangeSetting(setting);
            };
            body.Children.Add(btn);
            


            magicWrap.Children.Add(body);

            body.Visibility = Visibility.Collapsed;

            head.Click += (sender, e) =>
            {
                if (body.Visibility == Visibility.Collapsed)
                {
                    body.Visibility = Visibility.Visible;
                }
                else if (body.Visibility == Visibility.Visible)
                {
                    body.Visibility = Visibility.Collapsed;
                }
            };
            head.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_uISettings["BTNheadBackgroundColor"].ToString()));
            head.FontSize = 20;

            // body.Orientation = Orientation.Horizontal;


            var BTNDelete = new Button();
            BTNDelete.Content = "Delete";
            body.Children.Add(BTNDelete);
            BTNDelete.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_uISettings["BTNDeleteBackgroundColor"].ToString()));
            BTNDelete.Click += (sender, e) =>
            {
                this.RemoveSetting(setting);
            };

            Grid.SetColumn(BTNDelete, 2);

            return magicWrap;
        }

        private void RemoveSetting(KeyValuePair<Misc.Event, List<PropertyBundle>> setting)
        {
            if (!_applicationManager.EventActionSetting.Settings.ContainsKey(setting.Key))
            {
                return;
            }
            _applicationManager.EventActionSetting.Settings.Remove(setting.Key);
            DrawList();
        }

        private void ChangeSetting(KeyValuePair<Misc.Event, List<Models.PropertyBundle>> setting)
        {
            //this.IsEnabled = false;
            Windows.EditEvent editEvent = new Windows.EditEvent(setting.Key, setting.Value, _applicationManager);
            editEvent.Show();
            // TODO Improve Section for error handling + enable disable
        }

        private void EditingEventFinished()
        {
            this.IsEnabled = true;
        }


        private void BTNAddMapping_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            var win = new Windows.AddEvent(_applicationManager.AllAvailableEvents, AddingEventCompleted);
            win.Show();
        }

        void AddingEventCompleted(Misc.Event? @event)
        {
            this.IsEnabled = true;
            if(@event != null && 
                !_applicationManager.EventActionSetting.Settings.ContainsKey((Misc.Event)@event))
            {
                _applicationManager.EventActionSetting.Settings.Add((Misc.Event)@event, new List<PropertyBundle>());
            }
            _applicationManager.SaveChanges();
            DrawList();
        }
    }
}
