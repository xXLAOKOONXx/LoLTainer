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
        private Action<Setting> _action;
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
            try
            {
                var filePath = System.IO.Path.Combine(AppContext.BaseDirectory, "Settings", "UISettings.json");

                var jsontext = System.IO.File.ReadAllText(filePath);

                var uISettings = JObject.Parse(jsontext);

                _uISettings = uISettings["AddQueue"];
            }
            catch (Exception ex)
            {
                Loggings.Logger.Log(Loggings.LogType.UI, "Failed loading AddQueue UISettings from File, Error Message: " + ex.Message);
            }

            SetBackgroundFromSettings(this, "BackgroundColor");
            SetBackgroundFromSettings(BTNAddSetting, "BTNAddSettingBackgroundColor");
            SetBackgroundFromSettings(BTNFileName, "BTNFileNameBackgroundColor");
            SetBackgroundFromSettings(BTNPlaySound, "BTNPlaySoundBackground");

            SetBorderFromSettings(BTNFileName, "ErrorBorderColor");
            SetBorderFromSettings(TXTDuration, "ErrorBorderColor");
            SetBorderFromSettings(TXTGroup, "ErrorBorderColor");

            /*
             * <SolidColorBrush x:Key="SliderSelectionBackground" Color="Green" />
                <SolidColorBrush x:Key="SliderSelectionBorder" Color="Green" />
                <SolidColorBrush x:Key="SliderThumbBackground" Color="Green" />
                <SolidColorBrush x:Key="SliderThumbBackgroundDisabled" Color="Green" />
                <SolidColorBrush x:Key="SliderThumbBackgroundDragging" Color="Green" />
                <SolidColorBrush x:Key="SliderThumbBackgroundHover" Color="Green" />
                <SolidColorBrush x:Key="SliderThumbBorder" Color="Green" />
                <SolidColorBrush x:Key="SliderThumbBorderDisabled" Color="Green" />
                <SolidColorBrush x:Key="SliderThumbBorderDragging" Color="Green" />
                <SolidColorBrush x:Key="SliderThumbBorderHover" Color="Green" />
             * 
             */
             
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

        private void SetBorderFromSettings(Control control, string colorName)
        {

            Brush tmpBrush;
            try
            {
                tmpBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_uISettings[colorName].ToString()));
                control.BorderBrush = tmpBrush;
            }
            catch (Exception ex)
            {
                Loggings.Logger.Log(Loggings.LogType.UI, String.Format("Error getting Color from UISettings; Color:{0}, ErrorMessage:{1}", colorName, ex.Message));
            }
        }

        private EventHandler<Event> EventRadioButtonClicked;
        private UIElement PickerOption(Event @event)
        {
            var horistack = new StackPanel();
            horistack.Margin = new Thickness(5, 5, 5, 5);
            horistack.Orientation = Orientation.Horizontal;
            var lbl = new Label();
            lbl.Content = @event.ToString();
            lbl.VerticalAlignment = VerticalAlignment.Center;
            lbl.Width = 150;
            SetBackgroundFromSettings(lbl, "LBLQueueBackgroundColor");
            lbl.MouseDown += (sender, e) =>
            {
                _event = @event;
                EventRadioButtonClicked.Invoke(sender, @event);
            };

            EventRadioButtonClicked += (sender, selectedEvent) =>
            {
                //radio.IsChecked = selectedEvent == @event;
                if (selectedEvent == @event)
                {
                    SetBackgroundFromSettings(lbl, "LBLQueueSelectedBackgroundColor");
                }
                else
                {
                    SetBackgroundFromSettings(lbl, "LBLQueueBackgroundColor");
                }
            };
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
            var success = true;
            if (!File.Exists(_fileName))
            {
                BTNFileName.BorderThickness = new Thickness(2);
                success = false;
            }
            else
            {
                BTNFileName.BorderThickness = new Thickness(0);
            }
            ExtractInt(TXTDuration, out playLength, ref success);
            ExtractInt(TXTGroup, out group, ref success);
            return success;
        }

        private void ExtractInt(TextBox textBox, out int value, ref bool success)
        {
            if (!int.TryParse(TXTDuration.Text, out value))
            {
                textBox.BorderThickness = new Thickness(2);
                success = false;
            }
            else
            {
                textBox.BorderThickness = new Thickness(0);
            }
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
