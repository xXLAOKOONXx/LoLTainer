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
        public const string SOUNDPLAYER_FILENAME = "FileName";
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
        public string FileName
        {
            get => _propertyBundle.Properties[SOUNDPLAYER_FILENAME] as string;
            set
            {
                _propertyBundle.Properties[SOUNDPLAYER_FILENAME] = value;
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
}

