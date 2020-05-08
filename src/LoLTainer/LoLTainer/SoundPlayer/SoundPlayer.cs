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

        public async Task PlaySound(int playerId, string fileName, int playLengthInSec)
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

        private static void Play(string playerDeviceName, TimeSpan playDuration)
        {
            string format = @"play {0} from 0 to {1}";
            var call = String.Format(format, playerDeviceName, playDuration.TotalMilliseconds);
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
    }
}
