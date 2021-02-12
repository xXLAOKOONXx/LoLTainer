using LoLTainer.Misc;
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
    /// Interaktionslogik für AddEvent.xaml
    /// </summary>
    public partial class AddEvent : Window
    {
        private bool _addEventClose = false;
        private Event _event;
        private EventHandler<Event> EventRadioButtonClicked;
        private JToken _uISettings;
        private Action<Event?> _action;

        public AddEvent(IEnumerable<Event> availableEvents, Action<Event?> completedAction)
        {
            InitializeComponent();
            _action = completedAction;
            DrawUISettings();
            DrawPickList(availableEvents);
            Closing += OnClosingWindow;
        }

        private void OnClosingWindow(object sender, object args)
        {
            if (_addEventClose)
            {
                _action(_event);
            }
            else
            {
                _action(null);
            }
        }

        private void DrawUISettings()
        {
            try
            {
                var filePath = System.IO.Path.Combine(AppContext.BaseDirectory, "Settings", "UISettings.json");

                var jsontext = System.IO.File.ReadAllText(filePath);

                var uISettings = JObject.Parse(jsontext);

                _uISettings = uISettings["AddEvent"];
            }
            catch (Exception ex)
            {
                Loggings.Logger.Log(Loggings.LogType.UI, "Failed loading AddQueue UISettings from File, Error Message: " + ex.Message);
            }

            SetBackgroundFromSettings(this, "BackgroundColor");
            SetBackgroundFromSettings(BTNAddEvent, "BTNAddEventBackgroundColor");
        }

        public void DrawPickList(IEnumerable<Event> availableEvents)
        {
            if (availableEvents.Count() == 0)
            {

                var msgbx = MessageBox.Show("Check your settings list, you already have every available Event in your list.");
                _addEventClose = false;
                this.Close();

                return;
            }
            foreach (var item in availableEvents)
            {
                this.EventPicker.Children.Add(PickerOption(item));
            }
            _event = availableEvents.FirstOrDefault();
            EventRadioButtonClicked.Invoke(null, _event);
        }

        private UIElement PickerOption(Event @event)
        {
            var horistack = new StackPanel();
            horistack.Margin = new Thickness(5, 5, 5, 5);
            horistack.Orientation = Orientation.Horizontal;
            var lbl = new Label();
            lbl.Content = @event.ToString();
            lbl.VerticalAlignment = VerticalAlignment.Center;
            lbl.Width = 150;
            SetBackgroundFromSettings(lbl, "LBLEventBackgroundColor");
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
                    SetBackgroundFromSettings(lbl, "LBLEventSelectedBackgroundColor");
                }
                else
                {
                    SetBackgroundFromSettings(lbl, "LBLEventBackgroundColor");
                }
            };
            horistack.Children.Add(lbl);
            return horistack;
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

        private void BTNAddEvent_Click(object sender, RoutedEventArgs e)
        {
            _addEventClose = true;
            this.Close();
        }
    }
}
