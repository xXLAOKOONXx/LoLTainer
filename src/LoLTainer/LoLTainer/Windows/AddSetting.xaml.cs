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
        public AddSetting(Action<Setting> action, IEnumerable<Event> usedEvents)
        {
            _action = action;
            InitializeComponent();
            DrawUISettings();
            DrawPickList(usedEvents);
        }

        public void DrawPickList(IEnumerable<Event> usedEvents)
        {
            var freshEvents = new List<Event>();
            var events = Enum.GetValues(typeof(Event));
            foreach(Event item in events)
            {
                if (!usedEvents.Contains(item))
                {
                    freshEvents.Add(item);
                }
            }
            if(freshEvents.Count == 0)
            {

                var msgbx = MessageBox.Show("Check your settings list, you already have every available Event in your list.");
                _manualClose = false;
                this.Close();

                return;
            }
            foreach(var item in freshEvents)
            {
                this.EventPicker.Children.Add(PickerOption(item));
            }
            _event = freshEvents[0];
            RadioButtonClicked.Invoke(null, _event);
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
        private EventHandler<Event> RadioButtonClicked;
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
                RadioButtonClicked.Invoke(sender, @event);
            };
            RadioButtonClicked += (sender, selectedEvent) => { radio.IsChecked = selectedEvent == @event; };

            var lbl = new Label();
            lbl.Content = @event.ToString();
            lbl.VerticalAlignment = VerticalAlignment.Center;
            lbl.Width = 150;
            lbl.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_uISettings["LBLQueueBackgroundColor"].ToString()));
            lbl.MouseDown += (sender, e) =>
            {
                _event = @event;
                RadioButtonClicked.Invoke(sender, @event);
            };

            horistack.Children.Add(radio);
            horistack.Children.Add(lbl);

            return horistack;
        }

        private bool AllValid()
        {
            if (!File.Exists(_fileName))
            {
                return false;
            }
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
            if (AllValid())
            {
                var set = new Setting(_event, _fileName);
                set.SoundPlayerGroup = _soundPlayerGroup;
                _action(set);
                this.Close();
            }
            else
            {
                System.Windows.MessageBox.Show("One or more entries are not valid.");
            }
        }
    }
}
