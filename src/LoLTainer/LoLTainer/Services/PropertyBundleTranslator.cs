using LoLTainer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.Services.PropertyBundleTranslator
{
    #region SoundPlayerTranslations

    public class SoundPlayerPropertyBundle : PropertyBundle
    {
        public const string SOUNDPLAYER_FILENAME = "FileName";
        public const string SOUNDPLAYER_PLAYLENGTH = "PlayLength";
        public const string SOUNDPLAYER_PLAYMODE = "PlayMode";
        public const string SOUNDPLAYER_SOUNDPLAYERGROUP = "SoundPlayerGroup";
        public const string SOUNDPLAYER_STARTTIME = "StartTime";
        public const string SOUNDPLAYER_VOLUME = "Volume";

        public string FileName
        {
            get => base.Properties[SOUNDPLAYER_FILENAME] as string;
            set
            {
                base.Properties[SOUNDPLAYER_FILENAME] = value;
            }
        }
        public string SoundPlayerGroup
        {
            get => base.Properties[SOUNDPLAYER_SOUNDPLAYERGROUP] as string;
            set
            {
                base.Properties[SOUNDPLAYER_SOUNDPLAYERGROUP] = value;
            }
        }
        public TimeSpan? PlayLength
        {
            get => base.Properties[SOUNDPLAYER_PLAYLENGTH] as TimeSpan?;
            set
            {
                base.Properties[SOUNDPLAYER_PLAYLENGTH] = value;
            }
        }
        public TimeSpan? StartTime
        {
            get => base.Properties[SOUNDPLAYER_STARTTIME] as TimeSpan?;
            set
            {
                base.Properties[SOUNDPLAYER_STARTTIME] = value;
            }
        }
        public Misc.PlayMode PlayMode
        {
            get => (Misc.PlayMode)base.Properties[SOUNDPLAYER_PLAYMODE];
            set
            {
                base.Properties[SOUNDPLAYER_PLAYMODE] = value;
            }
        }
        public float Volume
        {
            get => (float)base.Properties[SOUNDPLAYER_VOLUME];
            set
            {
                base.Properties[SOUNDPLAYER_VOLUME] = value;
            }
        }
    }

    #endregion
}

