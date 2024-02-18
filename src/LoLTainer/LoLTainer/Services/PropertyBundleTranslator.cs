using LoLTainer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.Services.PropertyBundleTranslator
{
    #region SoundPlayerTranslations

    public class SoundPlayerPropertyBundle
    {
        public const string SOUNDPLAYER_FILENAMES = "FileNames";
        public const string SOUNDPLAYER_PLAYLENGTH = "PlayLength";
        public const string SOUNDPLAYER_PLAYMODE = "PlayMode";
        public const string SOUNDPLAYER_SOUNDPLAYERGROUP = "SoundPlayerGroup";
        public const string SOUNDPLAYER_STARTTIME = "StartTime";
        public const string SOUNDPLAYER_VOLUME = "Volume";

        public SoundPlayerPropertyBundle(PropertyBundle propertyBundle)
        {
            _propertyBundle = propertyBundle;
        }

        private PropertyBundle _propertyBundle;
        public PropertyBundle PropertyBundle => _propertyBundle;
        public List<string> FileNames
        {
            get => _propertyBundle.Properties[SOUNDPLAYER_FILENAMES] as List<string>;
            set
            {
                _propertyBundle.Properties[SOUNDPLAYER_FILENAMES] = value;
            }
        }
        public string SoundPlayerGroup
        {
            get => _propertyBundle.Properties[SOUNDPLAYER_SOUNDPLAYERGROUP] as string;
            set
            {
                _propertyBundle.Properties[SOUNDPLAYER_SOUNDPLAYERGROUP] = value;
            }
        }
        public TimeSpan? PlayLength
        {
            get => _propertyBundle.Properties[SOUNDPLAYER_PLAYLENGTH] as TimeSpan?;
            set
            {
                _propertyBundle.Properties[SOUNDPLAYER_PLAYLENGTH] = value;
            }
        }
        public TimeSpan? StartTime
        {
            get => _propertyBundle.Properties[SOUNDPLAYER_STARTTIME] as TimeSpan?;
            set
            {
                _propertyBundle.Properties[SOUNDPLAYER_STARTTIME] = value;
            }
        }
        public Misc.PlayMode PlayMode
        {
            get => (Misc.PlayMode)_propertyBundle.Properties[SOUNDPLAYER_PLAYMODE];
            set
            {
                _propertyBundle.Properties[SOUNDPLAYER_PLAYMODE] = value;
            }
        }
        public float Volume
        {
            get => (float)_propertyBundle.Properties[SOUNDPLAYER_VOLUME];
            set
            {
                _propertyBundle.Properties[SOUNDPLAYER_VOLUME] = value;
            }
        }
    }

    #endregion

    #region OBSTranslation

    public class OBSPropertyBundle
    {
        public const string ACTION_TYPE = "ActionType";
        public const string SCENE_NAME = "SceneName";
        public const string ITEM_NAME = "ItemName";
        public const string FILTER_NAME = "FilterName";
        public const string ACTION_DURATION = "ActionDuration";
        public const string VISIBILITY_TOGGLE_ONLY = "ToggleOnly";
        public const string VISIBILITY_TARGET_VALUE = "TargetValue";

        public OBSPropertyBundle(PropertyBundle propertyBundle)
        {
            _propertyBundle = propertyBundle;
        }

        private PropertyBundle _propertyBundle;
        public PropertyBundle PropertyBundle => _propertyBundle;

        public Misc.OBSActionType ActionType
        {
            get => (Misc.OBSActionType)_propertyBundle.Properties[ACTION_TYPE];
            set
            {
                _propertyBundle.Properties[ACTION_TYPE] = value;
            }
        }
        public string SceneName
        {
            get => _propertyBundle.Properties[SCENE_NAME] as string;
            set
            {
                _propertyBundle.Properties[SCENE_NAME] = value;
            }
        }
        public string ItemName
        {
            get => _propertyBundle.Properties[ITEM_NAME] as string;
            set
            {
                _propertyBundle.Properties[ITEM_NAME] = value;
            }
        }
        public string FilterName
        {
            get => _propertyBundle.Properties[FILTER_NAME] as string;
            set
            {
                _propertyBundle.Properties[FILTER_NAME] = value;
            }
        }

        public TimeSpan? ActionDuration
        {
            get => _propertyBundle.Properties[ACTION_DURATION] as TimeSpan?;
            set
            {
                _propertyBundle.Properties[ACTION_DURATION] = value;
            }
        }

        public bool VisibilityToggleOnly
        {
            get => (bool)_propertyBundle.Properties[VISIBILITY_TOGGLE_ONLY];
            set
            {
                _propertyBundle.Properties[VISIBILITY_TOGGLE_ONLY] = value;
            }
        }
        public bool VisibilityTargetValue
        {
            get => (bool)_propertyBundle.Properties[VISIBILITY_TARGET_VALUE];
            set
            {
                _propertyBundle.Properties[VISIBILITY_TARGET_VALUE] = value;
            }
        }
    }

    #endregion
}

