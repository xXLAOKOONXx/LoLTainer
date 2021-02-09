using LoLTainer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;
using LoLTainer.Misc;
using LoLTainer.Models;

namespace LoLTainer.SoundPlayer
{
    /// <summary>
    /// Implementation of <see cref="ISoundPlayer"/> using Nugt Package NAudio.
    /// Used Version: 1.10.0
    /// </summary>
    public class NAudioPlayer : ActionAPIManagers.BaseActionAPIManager, ISoundPlayer
    {
        /// <summary>
        /// Amount of time in ms to wait after reasking whether an audio device is finished playing
        /// </summary>
        private const int _delayTicks = 100;

        private Dictionary<string, WaveOutEvent> _playerIds = new Dictionary<string, WaveOutEvent>();

        public override Dictionary<string, Type> PropertyList => throw new NotImplementedException();

        #region constructors
        public NAudioPlayer() : base()
        {
            Connected = true;
        }
        #endregion

        #region ISoundPlayer EndPoints
        public async Task ClearCaches()
        {
            await TerminateAllSounds();
            var keys = _playerIds.Keys.ToArray();
            foreach (var key in keys)
            {
                _playerIds.Remove(key);
            }
        }

        public async Task PlaySound(string playerId, string fileName, TimeSpan? startTime = null, TimeSpan? playLength = null, float volume = -1, PlayMode playMode = PlayMode.WaitPlaying)
        {

            var audioFile = new AudioFileReader(fileName);
            var outputDevice = GetPlayer(playerId);
            if (volume >= 0)
            {
                if (volume > 100)
                {
                    volume = 100;
                }
                outputDevice.Volume = 0.01f * volume;
            }
            switch (playMode)
            {
                case PlayMode.WaitPlaying:
                    Loggings.Logger.Log(Loggings.LogType.Sound, "Waiting for Player " + playerId + " to finish; Initial State: " + outputDevice.PlaybackState.ToString(), base.Id);
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        await Task.Delay(_delayTicks);
                    }
                    Loggings.Logger.Log(Loggings.LogType.Sound, "Waiting for Player " + playerId + " ended", base.Id);

                    break;
                case PlayMode.StopPlaying:
                    outputDevice.Stop();
                    break;
                case PlayMode.StopAllPlaying:
                    await TerminateAllSounds();
                    break;
            }

            Loggings.Logger.Log(Loggings.LogType.Sound, "Playing Sound in Player " + playerId + "", base.Id);
            if(startTime != null && startTime.Value.TotalSeconds >= 0)
            {
                try
                {
                    audioFile.CurrentTime = startTime.Value;
                } catch(Exception ex)
                {
                    Loggings.Logger.Log(Loggings.LogType.Sound, String.Format("Error setting startingtime. File:'{0}' StartTime:'{1}' Error:'{2}'",
                        fileName, startTime.ToString(), ex.Message));
                }
            }
            outputDevice.Init(audioFile);
            outputDevice.Play();
            if (playLength != null && playLength.Value.TotalSeconds >= 0)
            {
                if (audioFile.TotalTime > playLength)
                {
                    StopSoundDelayed(playerId, playLength.Value);
                }
            }
        }

        public async Task StopSound(string playerId)
        {
            if (!_playerIds.ContainsKey(playerId))
            {
                return;
            }
            GetPlayer(playerId).Stop();
        }
        public async Task TerminateAllSounds()
        {
            foreach (var v in _playerIds)
            {
                v.Value.Stop();
            }
        }
        #endregion

        private async Task StopSoundDelayed(string playerId, TimeSpan delay)
        {
            await Task.Delay(delay);
            await StopSound(playerId);
        }

        private WaveOutEvent GetPlayer(string playerId)
        {
            if (!_playerIds.ContainsKey(playerId))
            {
                var player = new WaveOutEvent();
                _playerIds.Add(playerId, player);
            }
            return _playerIds[playerId];
        }

        public override IActionWindow GetActionWindow()
        {
            var window = new Windows.SetSoundSettings(this);

            return window;
        }

        public async Task PlaySound(Services.PropertyBundleTranslator.SoundPlayerPropertyBundle propertyBundle) =>
            await PlaySound(
                playerId: propertyBundle.SoundPlayerGroup,
                fileName: propertyBundle.FileName,
                playLength: propertyBundle.PlayLength,
                playMode: propertyBundle.PlayMode,
                volume: propertyBundle.Volume,
                startTime: propertyBundle.StartTime
                );

        public override async void PerformAction(PropertyBundle propertyBundle, EventTriggeredEventArgs eventTriggeredEventArgs = null)
        {
            var bundle = new Services.PropertyBundleTranslator.SoundPlayerPropertyBundle(propertyBundle);
            await this.PlaySound(bundle);
        }

        public override void Connect()
        {
            
        }

        public override bool IsValidPropertyBundle(PropertyBundle propertyBundle)
        {
            try
            {
                var soundBundle = new Services.PropertyBundleTranslator.SoundPlayerPropertyBundle(propertyBundle);
                var x1 = soundBundle.FileName;
                var x2 = soundBundle.Volume;
                var x3 = soundBundle.PlayLength;
                var x4 = soundBundle.PlayMode;
                var x5 = soundBundle.SoundPlayerGroup;
                var x6 = soundBundle.StartTime;

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
