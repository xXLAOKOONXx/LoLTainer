﻿using LoLTainer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;
using LoLTainer.Misc;
using LoLTainer.Models;
using LoLTainer.Services.PropertyBundleTranslator;

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

        private Dictionary<string, OutputDeviceWrapper> _playerIds = new Dictionary<string, OutputDeviceWrapper>();

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
            switch (playMode)
            {
                case PlayMode.WaitPlaying:
                    outputDevice.WaitForPlayer(_delayTicks);
                    break;
                case PlayMode.StopPlaying:
                    outputDevice.CancelAndStop();
                    break;
                case PlayMode.StopAllPlaying:
                    await TerminateAllSounds();
                    break;
            }
            if (volume >= 0)
            {
                if (volume > 100)
                {
                    volume = 100;
                }
                outputDevice.OutputDevice.Volume = 0.01f * volume;
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
            outputDevice.OutputDevice.Init(audioFile);
            outputDevice.OutputDevice.Play();
            if (playLength != null && playLength.Value.TotalSeconds >= 0)
            {
                if (audioFile.TotalTime > playLength)
                {
                    await StopSoundDelayed(playerId, playLength.Value);
                }
            }
        }

        public async Task StopSound(string playerId)
        {
            if (!_playerIds.ContainsKey(playerId))
            {
                return;
            }
            GetPlayer(playerId).CancelAndStop();
        }
        public async Task TerminateAllSounds()
        {
            foreach (var v in _playerIds)
            {
                v.Value.CancelAndStop();
            }
        }
        #endregion

        private async Task StopSoundDelayed(string playerId, TimeSpan delay)
        {
            await Task.Delay(delay);
            await StopSound(playerId);
        }

        private OutputDeviceWrapper GetPlayer(string playerId)
        {
            if (!_playerIds.ContainsKey(playerId))
            {
                var player = new OutputDeviceWrapper(new WaveOutEvent());
                _playerIds.Add(playerId, player);
            }
            return _playerIds[playerId];
        }

        public override IActionWindow GetActionWindow()
        {
            var window = new Windows.SetSoundSettings(this);

            return window;
        }

        public async Task PlaySound(Services.PropertyBundleTranslator.SoundPlayerPropertyBundle propertyBundle)
        {
            if(propertyBundle.FileNames == null || propertyBundle.FileNames.Count() == 0)
            {
                switch (propertyBundle.PlayMode)
                {
                    case PlayMode.StopAllPlaying:
                        await TerminateAllSounds();
                        return;
                    case PlayMode.StopPlaying:
                        await StopSound(propertyBundle.SoundPlayerGroup);
                        return;
                    default:
                        return;
                }
            }
            var rnd = new Random();
            var index = rnd.Next(propertyBundle.FileNames.Count());
            await PlaySound(
                playerId: propertyBundle.SoundPlayerGroup,
                fileName: propertyBundle.FileNames[index],
                playLength: propertyBundle.PlayLength,
                playMode: propertyBundle.PlayMode,
                volume: propertyBundle.Volume,
                startTime: propertyBundle.StartTime
                );
        }
            

        public override async void PerformAction(PropertyBundle propertyBundle, EventTriggeredEventArgs eventTriggeredEventArgs = null)
        {
            if (!Connected)
            {
                return;
            }
            var bundle = new Services.PropertyBundleTranslator.SoundPlayerPropertyBundle(propertyBundle);
            await this.PlaySound(bundle);
        }

        public override void Connect()
        {
            Connected = true;
        }
        public override void DisConnect()
        {
            TerminateAllSounds().Wait();
            Connected = false;
        }

        public override bool IsValidPropertyBundle(PropertyBundle propertyBundle)
        {
            try
            {
                var soundBundle = new Services.PropertyBundleTranslator.SoundPlayerPropertyBundle(propertyBundle);
                var x1 = soundBundle.FileNames;
                var x2 = soundBundle.Volume;
                var x3 = soundBundle.PlayLength;
                var x4 = soundBundle.PlayMode;
                var x5 = soundBundle.SoundPlayerGroup;
                var x6 = soundBundle.StartTime;

                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }


        public class OutputDeviceWrapper
        {
            WaveOutEvent outputDevice;

            public WaveOutEvent OutputDevice
            {
                get => outputDevice;
            }

            public OutputDeviceWrapper(WaveOutEvent waveOutEvent)
            {
                outputDevice = waveOutEvent;
            }

            public bool WaitForPlayer(int delayTicks)
            {
                _cancel = false;

                Loggings.Logger.Log(Loggings.LogType.Sound, "Waiting for Player " + " to finish; Initial State: " + outputDevice.PlaybackState.ToString());
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Task.Delay(delayTicks).Wait();
                    if (_cancel)
                    {
                        return false;
                    }
                }
                Loggings.Logger.Log(Loggings.LogType.Sound, "Waiting for Player " + " ended");

                return true;
            }
            bool _cancel = false;
            public void CancelWaiting()
            {
                _cancel = true;
            }

            public void CancelAndStop()
            {
                CancelWaiting();
                outputDevice.Stop();
            }
        }
    }
}
