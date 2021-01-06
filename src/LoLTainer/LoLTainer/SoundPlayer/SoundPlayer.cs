using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.SoundPlayer
{
    /// <summary>
    /// SoundPlayer with <see cref="Interfaces.ISoundPlayer"/> implemented.
    /// Uses the mci funfction of winmm DLL to play sound.
    /// Supports using different playerIds to play different sound in paralell.
    /// </summary>
    public class SoundPlayer : Interfaces.ISoundPlayer
    {
        private List<int> _playerIds = new List<int>();

        #region ISoundPlayer EndPoints
        public async Task ClearCaches()
        {
            var delList = new List<int>();
            foreach (var v in _playerIds)
            {
                if (DeviceRunning(v.ToString()))
                {
                    delList.Add(v);
                    ClearCash(v.ToString());
                }
            }
            _playerIds.RemoveAll(x => delList.Contains(x));
        }

        public async Task PlaySound(int playerId, string fileName, int playLengthInSec, int volume = -1)
        {
            if (_playerIds.Contains(playerId))
            {
                CloseDevice(playerId.ToString());
                _playerIds.Remove(playerId);
            }
            OpenFile(fileName, playerId.ToString());
            Play(playerId.ToString(), TimeSpan.FromSeconds(playLengthInSec));
            _playerIds.Add(playerId);
        }

        public async Task StopSound(int playerId)
        {
            Stop(playerId.ToString());
        }
        public async Task TerminateAllSounds()
        {
            foreach (var v in _playerIds)
            {
                StopSound(v);
                ClearCash(v.ToString());
            }
            _playerIds = new List<int>();
        }
        #endregion


        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

        private static bool DeviceRunning(string playerDeviceName)
        {
            throw new NotImplementedException();
        }

        private static void OpenFile(string filepath, string playerDeviceName)
        {
            string format;
            format = @"open ""{0}"" alias {1}";
            var call = String.Format(format, filepath, playerDeviceName);
            mciSendString(call, null, 0, IntPtr.Zero);

        }

        private static void CloseDevice(string playerDeviceName)
        {
            string format = @"close {0}";
            var call = String.Format(format, playerDeviceName);
            mciSendString(call, null, 0, IntPtr.Zero);

        }

        /// <summary>
        /// Play the device starting by 0.
        /// If the file is long enough the file will be played for <paramref name="playDuration"/>.
        /// If the file is not long enough the file will be played completely
        /// </summary>
        /// <param name="playerDeviceName">name of device</param>
        /// <param name="playDuration">duration to play the device</param>
        private static void Play(string playerDeviceName, TimeSpan playDuration)
        {
            string call;
            if(GetSoundLength(playerDeviceName) >= playDuration)
            {
            string format = @"play {0} from 0 to {1}";
            call = String.Format(format, playerDeviceName, playDuration.TotalMilliseconds);
            }
            else
            {
                string format = @"play {0} from 0";
                call = String.Format(format, playerDeviceName);
            }
            mciSendString(call, null, 0, IntPtr.Zero);
        }

        private static void Stop(string playerDeviceName)
        {
            string format = @"stop {0}";
            var call = String.Format(format, playerDeviceName);
            mciSendString(call, null, 0, IntPtr.Zero);
        }

        private static void ClearCash(string playerDeviceName)
        {
            CloseDevice(playerDeviceName);
        }

        /// <summary>
        /// Function to get the Length of a open device.
        /// </summary>
        /// <param name="playerDeviceName">Name of device</param>
        /// <returns>Duration of the current player Device</returns>
        private static TimeSpan GetSoundLength(string playerDeviceName)
        {
            StringBuilder lengthBuf = new StringBuilder(32);

            var format = "status {0} length";
            var command = String.Format(format, playerDeviceName);

            mciSendString(command, lengthBuf, lengthBuf.Capacity, IntPtr.Zero);

            int length = 0;
            int.TryParse(lengthBuf.ToString(), out length);

            return TimeSpan.FromMilliseconds(length);
        }
    }
}
