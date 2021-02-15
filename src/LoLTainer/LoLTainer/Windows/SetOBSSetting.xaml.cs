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
using LoLTainer.Models;
using Newtonsoft.Json.Linq;

namespace LoLTainer.Windows
{
    /// <summary>
    /// Interaktionslogik für SetOBSSettings.xaml
    /// </summary>
    public partial class SetOBSSetting : Window, Interfaces.IActionWindow
    {
        private Services.PropertyBundleTranslator.OBSPropertyBundle _oBSPropertyBundle;
        private Action<PropertyBundle> _action;

        private Misc.OBSActionType _oBSActionType;
        private JToken _uISettings;
        private bool _manualClose = true;

        public SetOBSSetting()
        {
            InitializeComponent();
            ActionTypeChanged += OnActionTypeChanged;
            DrawUISettings();
            DrawSelectActionType();
            this.Closing += OnWindowClose;
        }
        private void DrawUISettings()
        {
            var filePath = System.IO.Path.Combine(AppContext.BaseDirectory, "Settings", "UISettings.json");

            var jsontext = System.IO.File.ReadAllText(filePath);

            var uISettings = JObject.Parse(jsontext);

            _uISettings = uISettings["SetOBSSetting"];

            SetBackgroundFromSettings(this, "BackgroundColor");
            SetBackgroundFromSettings(BTNSave, "BTNSaveBackgroundColor");


            LBLSceneName.Visibility = Visibility.Collapsed;
            TXTSceneName.Visibility = Visibility.Collapsed;
            LBLSourceName.Visibility = Visibility.Collapsed;
            TXTSourceName.Visibility = Visibility.Collapsed;
            LBLFilterName.Visibility = Visibility.Collapsed;
            TXTFilterName.Visibility = Visibility.Collapsed;
            LBLActionDuration.Visibility = Visibility.Collapsed;
            TXTActionDuration.Visibility = Visibility.Collapsed;
            LBLToggleOnly.Visibility = Visibility.Collapsed;
            BTNToggleOnly.Visibility = Visibility.Collapsed;
            LBLTargetValue.Visibility = Visibility.Collapsed;
            BTNTargetValue.Visibility = Visibility.Collapsed;
        }

        private void DrawSelectActionType()
        {
            STKActionType.Children.Clear();
            foreach (Misc.OBSActionType item in Enum.GetValues(typeof(Misc.OBSActionType)))
            {
                STKActionType.Children.Add(DrawActionType(item));
            }
        }
        private EventHandler ActionTypeChanged;
        private UIElement DrawActionType(Misc.OBSActionType oBSActionType)
        {
            var button = new Button();
            button.Margin = new Thickness(5);
            button.Padding = new Thickness(2);
            button.Content = oBSActionType.ToString();
            SetBackgroundFromSettings(button, "BTNActionTypeBackgroundColor");
            button.Click += (s, o) =>
            {
                _oBSActionType = oBSActionType;
                ActionTypeChanged?.Invoke(s, o);
            };
            ActionTypeChanged += (s, o) =>
            {
                if (_oBSActionType == oBSActionType)
                {
                    SetBackgroundFromSettings(button, "BTNActionTypeSelectedBackgroundColor");
                }
                else
                {
                    SetBackgroundFromSettings(button, "BTNActionTypeBackgroundColor");
                }
            };
            return button;
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

        public void Open(Action<PropertyBundle> finishedEditingAction, PropertyBundle propertyBundle)
        {
            _action = finishedEditingAction;
            if (propertyBundle == null)
            {
                var tmpbundle = new PropertyBundle();
                tmpbundle.ActionManager = Misc.ActionManager.OBS;
                _oBSPropertyBundle = new Services.PropertyBundleTranslator.OBSPropertyBundle(tmpbundle);
                TXTActionDuration.Text = "0";
            }
            else
            {
                _oBSPropertyBundle = new Services.PropertyBundleTranslator.OBSPropertyBundle(propertyBundle);
                TXTActionName.Text = _oBSPropertyBundle.PropertyBundle.ActionName;
                _oBSActionType = _oBSPropertyBundle.ActionType;
                try
                {
                    var tmp = _oBSPropertyBundle.VisibilityToggleOnly;
                    _toggleOnly = tmp;
                }
                catch (Exception)
                {

                }
                try
                {
                    var tmp = _oBSPropertyBundle.VisibilityTargetValue;
                    _targetValue = tmp;
                }
                catch (Exception)
                {

                }
                try
                {
                    var tmp = _oBSPropertyBundle.SceneName;
                    TXTSceneName.Text = tmp;
                }
                catch (Exception)
                {

                }
                try
                {
                    var tmp = _oBSPropertyBundle.ItemName;
                    TXTSourceName.Text = tmp;
                }
                catch (Exception)
                {

                }
                try
                {
                    var tmp = _oBSPropertyBundle.FilterName;
                    TXTFilterName.Text = tmp;
                }
                catch (Exception)
                {

                }

                try
                {
                    var tmp = _oBSPropertyBundle.ActionDuration;
                    if (tmp.HasValue)
                    {
                        TXTActionDuration.Text = tmp.Value.TotalSeconds.ToString();
                    }
                    else
                    {
                        TXTActionDuration.Text = "0";
                    }
                }
                catch (Exception)
                {

                }
                HighlightSelection();
                OnActionTypeChanged();

            }
            OnActionTypeChanged();
            this.Show();
        }

        private void HighlightSelection()
        {
            foreach (var item in STKActionType.Children)
            {
                if (item.GetType() == typeof(Button))
                {
                    var btn = (Button)item;
                    if (btn.Content.ToString() == _oBSActionType.ToString())
                    {
                        SetBackgroundFromSettings(btn, "BTNActionTypeSelectedBackgroundColor");
                    }
                }
            }
        }

        private void OnActionTypeChanged(object sender = null, object args = null)
        {
            switch (_oBSActionType)
            {
                case Misc.OBSActionType.Scene:
                    EnableSceneFields();
                    break;
                case Misc.OBSActionType.ItemVisibility:
                    EnableItemFields();
                    break;
                case Misc.OBSActionType.FilterVisibility:
                    EnableFilterFields();
                    break;
                default:
                    break;
            }
        }

        private void EnableSceneFields()
        {
            LBLSceneName.Visibility = Visibility.Visible;
            TXTSceneName.Visibility = Visibility.Visible;
            LBLSourceName.Visibility = Visibility.Collapsed;
            TXTSourceName.Visibility = Visibility.Collapsed;
            LBLFilterName.Visibility = Visibility.Collapsed;
            TXTFilterName.Visibility = Visibility.Collapsed;
            LBLActionDuration.Visibility = Visibility.Visible;
            TXTActionDuration.Visibility = Visibility.Visible;
            LBLToggleOnly.Visibility = Visibility.Collapsed;
            BTNToggleOnly.Visibility = Visibility.Collapsed;
            LBLTargetValue.Visibility = Visibility.Collapsed;
            BTNTargetValue.Visibility = Visibility.Collapsed;
        }
        private void EnableItemFields()
        {
            LBLSceneName.Visibility = Visibility.Visible;
            TXTSceneName.Visibility = Visibility.Visible;
            LBLSourceName.Visibility = Visibility.Visible;
            TXTSourceName.Visibility = Visibility.Visible;
            LBLFilterName.Visibility = Visibility.Collapsed;
            TXTFilterName.Visibility = Visibility.Collapsed;
            LBLActionDuration.Visibility = Visibility.Visible;
            TXTActionDuration.Visibility = Visibility.Visible;
            LBLToggleOnly.Visibility = Visibility.Visible;
            BTNToggleOnly.Visibility = Visibility.Visible;
            DrawToggleOnlyBasedUI();
        }
        private void EnableFilterFields()
        {
            LBLSceneName.Visibility = Visibility.Collapsed;
            TXTSceneName.Visibility = Visibility.Collapsed;
            LBLSourceName.Visibility = Visibility.Visible;
            TXTSourceName.Visibility = Visibility.Visible;
            LBLFilterName.Visibility = Visibility.Visible;
            TXTFilterName.Visibility = Visibility.Visible;
            LBLActionDuration.Visibility = Visibility.Visible;
            TXTActionDuration.Visibility = Visibility.Visible;
            LBLToggleOnly.Visibility = Visibility.Visible;
            BTNToggleOnly.Visibility = Visibility.Visible;
            DrawToggleOnlyBasedUI();
        }

        private void OnWindowClose(object s, object o)
        {
            if (_manualClose)
            {
                _action(null);
            }
        }

        private void BTNSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateEntries(out double duration))
            {
                _manualClose = false;

                _oBSPropertyBundle.PropertyBundle.ActionName = TXTActionName.Text;

                _oBSPropertyBundle.ActionType = _oBSActionType;

                if (TXTSceneName.IsVisible)
                {
                    _oBSPropertyBundle.SceneName = TXTSceneName.Text;
                }
                if (TXTSourceName.IsVisible)
                {
                    _oBSPropertyBundle.ItemName = TXTSourceName.Text;
                }
                if (TXTFilterName.IsVisible)
                {
                    _oBSPropertyBundle.FilterName = TXTFilterName.Text;
                }
                if (TXTActionDuration.IsVisible)
                {
                    if (duration > 0)
                    {
                        _oBSPropertyBundle.ActionDuration = TimeSpan.FromSeconds(duration);
                    }
                    else
                    {
                        _oBSPropertyBundle.ActionDuration = null;
                    }
                }
                if (BTNToggleOnly.IsVisible)
                {
                    _oBSPropertyBundle.VisibilityToggleOnly = _toggleOnly;
                }
                if (BTNTargetValue.IsVisible)
                {
                    _oBSPropertyBundle.VisibilityTargetValue = _targetValue;
                }

                _action(_oBSPropertyBundle.PropertyBundle);

                Close();
            }
        }

        private bool ValidateEntries(out double duration)
        {
            bool success = true;
            ValidateNormalTextField(ref success, TXTSceneName);
            ValidateNormalTextField(ref success, TXTFilterName);
            ValidateNormalTextField(ref success, TXTSourceName);
            if (TXTActionDuration.IsVisible)
            {
                if (!double.TryParse(TXTActionDuration.Text, out duration))
                {
                    success = false;
                }
            }
            else
            {
                duration = -1;
            }
            return success;
        }
        private void ValidateNormalTextField(ref bool success, TextBox textBox)
        {
            if (textBox.IsVisible && (textBox.Text == null || textBox.Text == ""))
            {
                success = false;
                SetBorderFromSettings(textBox, "ErrorBorderColor");
            }
            else
            {
                RemoveBorder(textBox);
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
        private void RemoveBorder(Control control)
        {
            control.BorderBrush = null;
        }
        private bool _toggleOnly = false;
        private void BTNToggleOnly_Click(object sender, RoutedEventArgs e)
        {
            _toggleOnly = !_toggleOnly;
            DrawToggleOnlyBasedUI();
        }

        private void DrawToggleOnlyBasedUI()
        {

            if (_toggleOnly)
            {
                SetBackgroundFromSettings(BTNToggleOnly, "BTNOnBackgroundColor");
                BTNToggleOnly.Content = "TRUE";
                BTNTargetValue.Visibility = Visibility.Collapsed;
                LBLTargetValue.Visibility = Visibility.Collapsed;
            }
            else
            {
                SetBackgroundFromSettings(BTNToggleOnly, "BTNOffBackgroundColor");
                BTNToggleOnly.Content = "FALSE";
                BTNTargetValue.Visibility = Visibility.Visible;
                LBLTargetValue.Visibility = Visibility.Visible;
            }
            DrawBTNTargetValue();
        }

        private bool _targetValue = true;
        private void BTNTargetValue_Click(object sender, RoutedEventArgs e)
        {
            _targetValue = !_targetValue;
            DrawBTNTargetValue();
        }
        private void DrawBTNTargetValue()
        {
            if (_targetValue)
            {
                SetBackgroundFromSettings(BTNTargetValue, "BTNOnBackgroundColor");
                BTNTargetValue.Content = "VISIBLE";
            }
            else
            {
                SetBackgroundFromSettings(BTNTargetValue, "BTNOffBackgroundColor");
                BTNTargetValue.Content = "INVISIBLE";
            }
        }
    }
}
