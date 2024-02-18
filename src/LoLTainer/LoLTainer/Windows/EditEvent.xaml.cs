using LoLTainer.Misc;
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
using System.Windows.Shapes;

namespace LoLTainer.Windows
{
    /// <summary>
    /// Interaktionslogik für EditEvent.xaml
    /// </summary>
    public partial class EditEvent : Window
    {
        private JToken _uISettings;
        private List<PropertyBundle> _propertyBundles;
        private Misc.Event _event;
        private Interfaces.IApplicationManager _applicationManager;

        public EditEvent(Misc.Event @event, List<PropertyBundle> propertyBundles, Interfaces.IApplicationManager applicationManager)
        {
            InitializeComponent();

            _applicationManager = applicationManager;
            _event = @event;
            _propertyBundles = propertyBundles;
            this.Title = String.Format("Edit Event '{0}'", _event);
            DrawUISettings();
            DrawList();
        }
        private void DrawUISettings()
        {
            var filePath = System.IO.Path.Combine(AppContext.BaseDirectory, "Settings", "UISettings.json");

            var jsontext = System.IO.File.ReadAllText(filePath);

            var uISettings = JObject.Parse(jsontext);

            _uISettings = uISettings["EditEvent"];

            SetBackgroundFromSettings(this, "BackgroundColor");
            SetBackgroundFromSettings(BTNAddAction, "BTNAddActionBackgroundColor");
            BTNAddAction.BorderThickness = new Thickness(0);
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
                Loggings.Logger.Log(Loggings.LogType.UI, String.Format("Exception getting Color {0} for 'EditEvent'; ErrorMessage:{1}", colorName, ex.Message));
            }
        }

        private void DrawList()
        {
            SettingsPanel.Children.Clear();

            foreach (var item in _propertyBundles)
            {
                SettingsPanel.Children.Add(GetUIElement(item));
            }
        }

        private UIElement GetUIElement(PropertyBundle propertyBundle)
        {
            var magicWrap = new StackPanel();
            magicWrap.Margin = new Thickness(0, 0, 0, 1);
            var head = new Button();
            head.Content = String.Format("{0} - {1}", propertyBundle.ActionManager.ToString(), propertyBundle.ActionName);
            magicWrap.Children.Add(head);
            head.BorderThickness = new Thickness(0);

            var body = new Grid();
            try
            {
                body.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_uISettings["ItemBodyBackgroundColor"].ToString()));
            }
            catch (Exception ex)
            {
                Loggings.Logger.Log(Loggings.LogType.UI, String.Format("Exception reading UI Setting 'ItemBodyBackgroundColor' for 'EditEvents'; Errormessage:{0}", ex.Message));
            }

            body.ColumnDefinitions.Add(new ColumnDefinition());
            body.ColumnDefinitions.Add(new ColumnDefinition());
            body.ColumnDefinitions.Add(new ColumnDefinition());

            var lbl1 = new Label();

            var bnd = new Binding("ActionName");
            bnd.Source = propertyBundle;
            lbl1.SetBinding(Label.ContentProperty, bnd);
            lbl1.BorderThickness = new Thickness(0);

            lbl1.HorizontalContentAlignment = HorizontalAlignment.Right;
            lbl1.FlowDirection = FlowDirection.RightToLeft;
            Grid.SetColumn(lbl1, 0);
            Grid.SetColumnSpan(lbl1, 2); // As long as no edit Button available this space might be used in that way.
            body.Children.Add(lbl1);

            // Edit Button
            var btn = new Button();
            btn.BorderThickness = new Thickness(0);
            btn.Content = "Edit";
            Grid.SetColumn(btn, 1);
            btn.Click += (s, e) =>
            {
                EditPropertyBundle(propertyBundle);
            };
            SetBackgroundFromSettings(btn, "BTNChangeItemBackgroundColor");
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
            SetBackgroundFromSettings(head, "BTNHeadBackgroundColor");
            head.FontSize = 20;

            // body.Orientation = Orientation.Horizontal;


            var BTNDelete = new Button();
            BTNDelete.BorderThickness = new Thickness(0);
            BTNDelete.Content = "Delete";
            body.Children.Add(BTNDelete);
            SetBackgroundFromSettings(BTNDelete, "BTNDeleteItemBackgroundColor");
            BTNDelete.Click += (sender, e) =>
            {
                this.RemovePropertyBundle(propertyBundle);
            };

            Grid.SetColumn(BTNDelete, 2);

            return magicWrap;
        }

        private void EditPropertyBundle(PropertyBundle propertyBundle)
        {
            this.IsEnabled = false;
            string actionAPIManagerValue = "";
            try
            {
                actionAPIManagerValue = propertyBundle.ActionManager.ToString();
                var actionWindow = _applicationManager.GetActionAPIManager(propertyBundle.ActionManager).GetActionWindow();
                actionWindow.Open(EditPropertyBundleFinished, propertyBundle);
            }
            catch (Exception ex)
            {
                Loggings.Logger.Log(Loggings.LogType.Settings, string.Format("Exception Opening Action Editor '{0}'; Message: {1}", actionAPIManagerValue, ex.Message));
                MessageBox.Show("Error Opening Action Editor");
                this.IsEnabled = true;
            }
        }

        void RemovePropertyBundle(PropertyBundle propertyBundle)
        {
            _propertyBundles.Remove(propertyBundle);
            _applicationManager.SaveChanges();
            DrawList();
        }

        void EditPropertyBundleFinished(PropertyBundle propertyBundle)
        {
            this.IsEnabled = true;
            _applicationManager.SaveChanges();
            DrawList();
        }

        private void BTNAddAction_Clicked(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;

            var win = new SelectActionManager();
            win.Open(OnActionManagerSelect, _applicationManager);
        }
        void OnActionManagerSelect(ActionManager? actionManager)
        {
            if (actionManager == null)
            {
                this.IsEnabled = true;
                return;
            }
            var actionWindow = _applicationManager.GetActionAPIManager((ActionManager)actionManager).GetActionWindow();
            actionWindow.Open(AddPropertyBundleFinished, null);
        }
        void AddPropertyBundleFinished(PropertyBundle propertyBundle)
        {
            this.IsEnabled = true;
            if (propertyBundle == null)
            {
                return;
            }
            _propertyBundles.Add(propertyBundle);
            _applicationManager.SaveChanges();
            DrawList();
        }
    }
}
