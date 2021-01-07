using LoLTainer.API;
using LoLTainer.Misc;
using LoLTainer.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaktionslogik für AddSetting.xaml
    /// </summary>
    public partial class AddSetting : Window
    {
        private bool _manualClose = true;
        private Event _event;
        private string _fileName = "";
        private int _soundPlayerGroup = 0;
        private Action<Setting> _action;
        private double _volume = -1;
        private bool _playingSound = false;
        private PlayMode _playMode = PlayMode.StopPlaying;
        public AddSetting(Action<Setting> action, IEnumerable<Event> usedEvents)
        {
            _action = action;
            InitializeComponent();
            UpdatePlayModeRadios();
            DrawUISettings();
            DrawPickList(usedEvents);
        }

        public void DrawPickList(IEnumerable<Event> usedEvents)
        {
            var freshEvents = new List<Event>();
            var events = Enum.GetValues(typeof(Event));
            foreach (Event item in events)
            {
                if (!usedEvents.Contains(item))
                {
                    freshEvents.Add(item);
                }
            }
            if (freshEvents.Count == 0)
            {

                var msgbx = MessageBox.Show("Check your settings list, you already have every available Event in your list.");
                _manualClose = false;
                this.Close();

                return;
            }
            foreach (var item in freshEvents)
            {
                this.EventPicker.Children.Add(PickerOption(item));
            }
            _event = freshEvents[0];
            EventRadioButtonClicked.Invoke(null, _event);
        }

        private JToken _uISettings;
        private void DrawUISettings()
        {
            var filePath = System.IO.Path.Combine(AppContext.BaseDirectory, "Settings", "UISettings.json");

            var jsontext = System.IO.File.ReadAllText(filePath);

            var uISettings = JObject.Parse(jsontext);

            _uISettings = uISettings["AddQueue"];

            this.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_uISettings["BackgroundColor"].ToString()));

            this.BTNAddSetting.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_uISettings["BTNAddQueueBackgroundColor"].ToString()));

            this.BTNFileName.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_uISettings["BTNPickIconBackgroundColor"].ToString()));
        }
        private EventHandler<Event> EventRadioButtonClicked;
        private UIElement PickerOption(Event @event)
        {
            var horistack = new StackPanel();
            horistack.Margin = new Thickness(5, 5, 5, 5);
            horistack.Orientation = Orientation.Horizontal;
            var radio = new RadioButton();
            radio.Padding = new Thickness(10);
            radio.VerticalAlignment = VerticalAlignment.Center;
            radio.Click += (sender, e) =>
            {
                _event = @event;
                EventRadioButtonClicked.Invoke(sender, @event);
            };
            EventRadioButtonClicked += (sender, selectedEvent) => { radio.IsChecked = selectedEvent == @event; };

            var lbl = new Label();
            lbl.Content = @event.ToString();
            lbl.VerticalAlignment = VerticalAlignment.Center;
            lbl.Width = 150;
            lbl.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_uISettings["LBLQueueBackgroundColor"].ToString()));
            lbl.MouseDown += (sender, e) =>
            {
                _event = @event;
                EventRadioButtonClicked.Invoke(sender, @event);
            };

            horistack.Children.Add(radio);
            horistack.Children.Add(lbl);

            return horistack;
        }

        private void PlayModeWaitClicked(object sender, EventArgs args)
        {
            _playMode = PlayMode.WaitPlaying;
            UpdatePlayModeRadios();
        }
        private void PlayModeStopClicked(object sender, EventArgs args)
        {
            _playMode = PlayMode.StopPlaying;
            UpdatePlayModeRadios();

        }
        private void PlayModeStopAllClicked(object sender, EventArgs args)
        {
            _playMode = PlayMode.StopAllPlaying;
            UpdatePlayModeRadios();

        }

        private void UpdatePlayModeRadios()
        {
            RDBWait.IsChecked = _playMode == PlayMode.WaitPlaying;
            RDBStop.IsChecked = _playMode == PlayMode.StopPlaying;
            RDBStopAll.IsChecked = _playMode == PlayMode.StopAllPlaying;
        }

        private bool AllValid(out int playLength, out int group)
        {
            if (!File.Exists(_fileName))
            {
                playLength = 10;
                group = 0;
                return false;
            }
            if (!int.TryParse(TXTDuration.Text, out playLength))
            {
                group = 0;
                return false;
            }
            if (!int.TryParse(TXTGroup.Text, out group))
                return false;
            return true;
        }

        private void BTNFileName_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();


            var filter = "SoundFiles (WAV)|*.wav";

            dialog.Filter = filter;
            dialog.FilterIndex = 1;
            //"txt files (*.txt)|*.txt|All files (*.*)|*.*";

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _fileName = dialog.FileName;
                this.LBLFileName.Content = _fileName;
            }
        }

        private void BTNAddSetting_Click(object sender, RoutedEventArgs e)
        {
            if (AllValid(out int playLength, out int group))
            {
                var set = new Setting(_event, _fileName);
                set.SoundPlayerGroup = group;
                set.Volume = (int)Math.Round(SLDVolume.Value, 0);
                set.PlayLengthInSec = playLength;
                _action(set);
                this.Close();
            }
            else
            {
                System.Windows.MessageBox.Show("One or more entries are not valid.");
            }
        }

        private async void BTNPlaySound_Click(object sender, RoutedEventArgs e)
        {
            if (_playingSound)
            {
                return;
            }
            if (AllValid(out int playLength, out int group))
            {
                _playingSound = true;
                var set = new Setting(_event, _fileName);
                set.SoundPlayerGroup = group;
                set.Volume = (int)Math.Round(SLDVolume.Value, 0);
                set.PlayLengthInSec = playLength;
                var t = new Task(async () =>
                {
                    var soundplayer = APIManager.GetActiveManager().SoundPlayer;
                    await soundplayer.PlaySound(set.SoundPlayerGroup, set.FileName, set.PlayLengthInSec, set.Volume, _playMode);
                    _playingSound = false;
                });
                t.Start();
            }
            else
            {
                System.Windows.MessageBox.Show("One or more entries are not valid.");
            }
        }
    }
}
