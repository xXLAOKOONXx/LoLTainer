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
        private Interfaces.ISettingsManager _settingsManager;
        private Interfaces.IAPIManager _aPIManager;

        public MainWindow()
        {
            InitializeComponent();
            _settingsManager = new SettingsManager.SettingsManager();
            _aPIManager = new API.APIManager(_settingsManager);
            DrawUISettings();
            DrawList();
        }

        private void DrawUISettings()
        {
            var filePath = System.IO.Path.Combine(AppContext.BaseDirectory, "Settings", "UISettings.json");

            var jsontext = System.IO.File.ReadAllText(filePath);

            var uISettings = JObject.Parse(jsontext);

            _uISettings = uISettings["MainWindow"];

            this.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_uISettings["BackgroundColor"].ToString()));

            this.BTNAddMapping.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_uISettings["BTNAddQueueBackgroundColor"].ToString()));
        }

        private void DrawList()
        {
            SettingsPanel.Children.Clear();

            foreach (var item in _settingsManager.GetAllSettings())
            {
                SettingsPanel.Children.Add(GetUIElement(item));
            }
        }

        private UIElement GetUIElement(Setting setting)
        {
            var magicWrap = new StackPanel();
            var head = new Button();
            head.Content = setting.Event;
            magicWrap.Children.Add(head);

            var body = new Grid();
            body.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_uISettings["QueueBodyBackgroundColor"].ToString()));
            body.ColumnDefinitions.Add(new ColumnDefinition());
            body.ColumnDefinitions.Add(new ColumnDefinition());
            body.ColumnDefinitions.Add(new ColumnDefinition());

            var lbl1 = new Label();
            var bnd = new Binding("FileName");
            bnd.Source = setting;
            lbl1.SetBinding(Label.ContentProperty, bnd);
            Grid.SetColumn(lbl1, 0);
            body.Children.Add(lbl1);


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

        private void RemoveSetting(Setting setting)
        {
            _settingsManager.RemoveSetting(setting);
            DrawList();
        }
        private void AddSetting()
        {
            var adder = new Windows.AddSetting(SettingAdd,_settingsManager.GetAllSettings().Select(set => set.Event));

            adder.Show();
        }

        private void SettingAdd(Setting setting)
        {
            _settingsManager.AddSetting(setting);
            DrawList();
        }

        private void ChangeSetting(Setting setting)
        {
            
        }

        private void BTNAddMapping_Click(object sender, RoutedEventArgs e)
        {
            AddSetting();
        }
    }
}
