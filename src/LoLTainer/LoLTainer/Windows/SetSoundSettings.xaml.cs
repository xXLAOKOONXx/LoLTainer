using LoLTainer.API;
using LoLTainer.Interfaces;
using LoLTainer.Misc;
using LoLTainer.Models;
using LoLTainer.Services.PropertyBundleTranslator;
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
        //private string _fileName = "";
        private Action<PropertyBundle> _action;
        private bool _playingSound = false;
        private PlayMode _playMode = PlayMode.StopPlaying;
        private ISoundPlayer _soundPlayer;
        private Services.PropertyBundleTranslator.SoundPlayerPropertyBundle _propertyBundle;
        private List<string> _fileNames = new List<string>();
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

                _uISettings = uISettings["SetSoundSetting"];
            }
            catch (Exception ex)
            {
                Loggings.Logger.Log(Loggings.LogType.UI, "Failed loading AddQueue UISettings from File, Error Message: " + ex.Message);
            }

            SetBackgroundFromSettings(this, "BackgroundColor");
            SetBackgroundFromSettings(BTNSave, "BTNSaveBackgroundColor");
            SetBackgroundFromSettings(BTNFileName, "BTNFileNameBackgroundColor");
            SetBackgroundFromSettings(BTNPlaySound, "BTNPlaySoundBackground");

            SetBorderFromSettings(BTNFileName, "ErrorBorderColor");
            SetBorderFromSettings(TXTDuration, "ErrorBorderColor");
            SetBorderFromSettings(TXTGroup, "ErrorBorderColor");
        }

        private void DrawFilesList()
        {
            STKFileNames.Children.Clear();
            foreach (var fileName in _fileNames)
            {
                STKFileNames.Children.Add(DrawFileName(fileName));
            }
        }

        private UIElement DrawFileName(string fileName)
        {
            var label = new Label();
            label.MouseEnter += (s, o) =>
            {
                SetBackgroundFromSettings(label, "LBLFileNameMouseOverBackgroundColor");
            };
            label.MouseLeave += (s, o) =>
            {
                SetBackgroundFromSettings(label, "LBLFileNameBackgroundColor");
            };
            label.MouseDown += (s, a) =>
            {
                _fileNames.Remove(fileName);
                DrawFilesList();
            };
            var tinyText = fileName.Split('\\').Last();
            label.Content = tinyText;
            SetBackgroundFromSettings(label, "LBLFileNameBackgroundColor");
            label.Margin = new Thickness(5);

            return label;
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

        private bool AllValid(out int playLength, out int startTime)
        {
            var success = true;
            /*
            if (!File.Exists(_fileName))
            {
                BTNFileName.BorderThickness = new Thickness(2);
                success = false;
            }
            else
            {
                BTNFileName.BorderThickness = new Thickness(0);
            }
            */
            ExtractInt(TXTDuration, out playLength, ref success);
            ExtractInt(TXTStart, out startTime, ref success);
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
                _fileNames.Add(dialog.FileName);
                DrawFilesList();
            }
        }

        private void BTNAddSetting_Click(object sender, RoutedEventArgs e)
        {
            if (AllValid(out int playLengthInSec, out int startTimeInSec))
            {
                _propertyBundle.FileNames = _fileNames;
                _propertyBundle.SoundPlayerGroup = TXTGroup.Text;
                _propertyBundle.PlayMode = _playMode;
                _propertyBundle.Volume = (int)Math.Round(SLDVolume.Value, 0);
                _propertyBundle.PlayLength = TimeSpan.FromSeconds(playLengthInSec);
                _propertyBundle.StartTime = TimeSpan.FromSeconds(startTimeInSec);
                _propertyBundle.PropertyBundle.ActionName = TXTActionName.Text;

                _action(_propertyBundle.PropertyBundle);
                _manualClose = false;
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
            if (AllValid(out int playLengthInSec, out int startTimeInSec))
            {
                var bundle = new Services.PropertyBundleTranslator.SoundPlayerPropertyBundle(new PropertyBundle());

                _playingSound = true;
                bundle.FileNames = _fileNames;
                bundle.SoundPlayerGroup = TXTGroup.Text;
                bundle.PlayMode = _playMode;
                bundle.Volume = (int)Math.Round(SLDVolume.Value, 0);
                bundle.PlayLength = TimeSpan.FromSeconds(playLengthInSec);
                bundle.StartTime = TimeSpan.FromSeconds(startTimeInSec);

                var t = new Task(async () =>
                {
                    await _soundPlayer.PlaySound(bundle);
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
            _action = finishedEditingAction;

            if (propertyBundle == null)
            {
                propertyBundle = new PropertyBundle();
                propertyBundle.ActionManager = ActionManager.SoundPlayer;
                _propertyBundle = new SoundPlayerPropertyBundle(propertyBundle);
            }
            else
            {
                _propertyBundle = new SoundPlayerPropertyBundle(propertyBundle);
                _fileNames = _propertyBundle.FileNames.ToList();
                TXTActionName.Text = _propertyBundle.PropertyBundle.ActionName;
                TXTDuration.Text = _propertyBundle.PlayLength.Value.TotalSeconds.ToString();
                TXTGroup.Text = _propertyBundle.SoundPlayerGroup;
                TXTStart.Text = _propertyBundle.StartTime.Value.TotalSeconds.ToString();
                SLDVolume.Value = _propertyBundle.Volume;
                LBLFileName.Content = _propertyBundle.FileNames;
            }

            this.Closed += (s, o) =>
            {
                if (_manualClose)
                {
                    finishedEditingAction(null);
                }
            };

            this.Show();
        }
    }
}
