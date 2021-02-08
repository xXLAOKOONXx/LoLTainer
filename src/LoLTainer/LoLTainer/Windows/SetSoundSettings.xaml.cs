using LoLTainer.API;
using LoLTainer.Interfaces;
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
    public partial class SetSoundSettings : Window, IActionWindow
    {
        private bool _manualClose = true;
        private Event _event;
        private string _fileName = "";
        private Action<PropertyBundle> _action;
        private bool _playingSound = false;
        private PlayMode _playMode = PlayMode.StopPlaying;
        private ISoundPlayer _soundPlayer;
        private Services.PropertyBundleTranslator.SoundPlayerPropertyBundle _propertyBundle;
        public SetSoundSettings(Interfaces.ISoundPlayer soundPlayer)
        {
            InitializeComponent();
            _soundPlayer = soundPlayer;
            DrawUISettings();
            UpdatePlayModeLabels();
        }

        
        private JToken _uISettings;
        private void DrawUISettings()
        {
            try
            {
                var filePath = System.IO.Path.Combine(AppContext.BaseDirectory, "Settings", "UISettings.json");

                var jsontext = System.IO.File.ReadAllText(filePath);

                var uISettings = JObject.Parse(jsontext);

                _uISettings = uISettings["AddSetting"];
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
                    SetBackgroundFromSettings(lbl, "LBLActionSelectedBackgroundColor");
                }
                else
                {
                    SetBackgroundFromSettings(lbl, "LBLActionBackgroundColor");
                }
            };
            horistack.Children.Add(lbl);
            return horistack;
        }

        private void PlayModeWaitClicked(object sender, EventArgs args)
        {
            _playMode = PlayMode.WaitPlaying;
            UpdatePlayModeLabels();
        }
        private void PlayModeStopClicked(object sender, EventArgs args)
        {
            _playMode = PlayMode.StopPlaying;
            UpdatePlayModeLabels();

        }
        private void PlayModeStopAllClicked(object sender, EventArgs args)
        {
            _playMode = PlayMode.StopAllPlaying;
            UpdatePlayModeLabels();

        }

        private void UpdatePlayModeLabels()
        {
            SetLabelBackgroundOnSelected(LBLPlayModeWait, _playMode == PlayMode.WaitPlaying);
            SetLabelBackgroundOnSelected(LBLPlayModeStop, _playMode == PlayMode.StopPlaying);
            SetLabelBackgroundOnSelected(LBLPlayModeStopAll, _playMode == PlayMode.StopAllPlaying);
        }
        private void SetLabelBackgroundOnSelected(Label label, bool selected)
        {
            if (selected)
            {
                SetBackgroundFromSettings(label, "LBLPlayModeSelectedBackgroundColor");
            }
            else
            {
                SetBackgroundFromSettings(label, "LBLPlayModeBackgroundColor");
            }
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
            if (!int.TryParse(textBox.Text, out value))
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


            var filter = "SoundFiles |*.wav;*.mp3|WAV|*.wav|MP3|*.mp3";

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
            if (AllValid(out int playLengthInSec))
            {
                _propertyBundle.Properties[Services.PropertyBundleTranslator.SOUNDPLAYER_FILENAME] = _fileName;
                var set = new Setting(_event, _fileName);
                set.SoundPlayerGroup = TXTGroup.Text;
                set.Volume = (int)Math.Round(SLDVolume.Value, 0);
                set.PlayLength = TimeSpan.FromSeconds(playLengthInSec);
                
                _action(_propertyBundle);
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
            if (AllValid(out int playLengthInSec, out int group))
            {
                _playingSound = true;
                var set = new Setting(_event, _fileName);
                set.SoundPlayerGroup = group;
                set.Volume = (int)Math.Round(SLDVolume.Value, 0);
                set.PlayLength = TimeSpan.FromSeconds(playLengthInSec);
                var t = new Task(async () =>
                {
                    var soundplayer = APIManager.GetActiveManager().SoundPlayer;
                    await soundplayer.PlaySound(set.SoundPlayerGroup, set.FileName, set.StartTime, set.PlayLength, set.Volume, _playMode);
                    _playingSound = false;
                });
                t.Start();
            }
            else
            {
                System.Windows.MessageBox.Show("One or more entries are not valid.");
            }
        }

        public void Open(Action<PropertyBundle> finishedEditingAction, PropertyBundle propertyBundle)
        {
            if(propertyBundle == null)
            {
                propertyBundle = new PropertyBundle();
                propertyBundle.ActionManager = ActionManager.SoundPlayer;
                var x = propertyBundle as Services.PropertyBundleTranslator.SoundPlayerPropertyBundle;
            }
            _propertyBundle = propertyBundle as Services.PropertyBundleTranslator.SoundPlayerPropertyBundle;

            _fileName = _propertyBundle.FileName;
            
            throw new NotImplementedException();
        }
    }
}
