using LoLTainer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;
using LoLTainer.Misc;

namespace LoLTainer.SoundPlayer
{
    /// <summary>
    /// Implementation of <see cref="ISoundPlayer"/> using Nugt Package NAudio.
    /// Used Version: 1.10.0
    /// </summary>
    public class NAudioPlayer : ISoundPlayer
    {
        // Used to wait for other files to finish
        private const int _delayTicks = 100;

        private Dictionary<int, WaveOutEvent> _playerIds = new Dictionary<int, WaveOutEvent>();
        #region ISoundPlayer EndPoints
        public async Task ClearCaches()
        {
            await TerminateAllSounds();
            var keys = _playerIds.Keys.ToArray();
            foreach(var key in keys)
            {
                _playerIds.Remove(key);
            }
        }

        public async Task PlaySound(int playerId, string fileName, int playLengthInSec, int volume = -1, PlayMode playMode = PlayMode.WaitPlaying)
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
                    Loggings.Logger.Log(Loggings.LogType.Sound, "Waiting for Player " + playerId + " to finish; Initial State: " + outputDevice.PlaybackState.ToString());
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        await Task.Delay(_delayTicks);
                    }
                    Loggings.Logger.Log(Loggings.LogType.Sound, "Waiting for Player " + playerId + " ended");

                    break;
                case PlayMode.StopPlaying:
                    outputDevice.Stop();
                    break;
                case PlayMode.StopAllPlaying:
                    await TerminateAllSounds();
                    break;
            }

            Loggings.Logger.Log(Loggings.LogType.Sound, "Playing Sound in Player " + playerId + "");
            outputDevice.Init(audioFile);
            outputDevice.Play();
            if (playLengthInSec >= 0)
            {
                if (audioFile.TotalTime > TimeSpan.FromSeconds(playLengthInSec))
                {
                    StopSoundDelayed(playerId, TimeSpan.FromSeconds(playLengthInSec));
                }
            }
        }

        public async Task StopSound(int playerId)
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

        private async Task StopSoundDelayed(int playerId, TimeSpan delay)
        {
            await Task.Delay(delay);
            await StopSound(playerId);
        }

        private WaveOutEvent GetPlayer(int playerId)
        {
            if (!_playerIds.ContainsKey(playerId))
            {
                var player = new WaveOutEvent();
                _playerIds.Add(playerId, player);
            }
            return _playerIds[playerId];
        }



    }
}
