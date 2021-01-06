using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoLTainer.Interfaces;
using System.Windows.Media;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

namespace LoLTainer.SoundPlayer
{
    /// <summary>
    /// MediaPlayer with <see cref="Interfaces.ISoundPlayer"/> implemented.
    /// Uses the System.Media.MediaPlayer to play sound.
    /// supports using different playerIds to play different sound in paralell.
    /// </summary>
    public class MediaPlayer : ISoundPlayer
    {
        private Dictionary<int, System.Windows.Media.MediaPlayer> mediaPlayers = new Dictionary<int, System.Windows.Media.MediaPlayer>();

        public MediaPlayer()
        {

        }

        #region ISoundPlayer EndPoints

        public async Task ClearCaches()
        {
            await TerminateAllSounds();
            mediaPlayers = new Dictionary<int, System.Windows.Media.MediaPlayer>();
        }
        public async Task PlaySound(int playerId, string fileName, int playLengthInSec, int volume = -1)
        {
            if (volume > 100)
                volume = 100;
            else if (volume < 0)
                volume = -1;
            playerTaken?.Invoke(this, playerId);
            bool myPlayer = true;
            playerTaken += (sender, id) =>
            {
                if (myPlayer && id == playerId)
                {
                    myPlayer = false;
                }
            };
            var player = GetMediaPlayer(playerId);
            var uri = new Uri(fileName);
            if (!myPlayer) return;
            if (myPlayer && volume != -1)
            {
                player.Volume = volume * 0.01;
            }
            if (!myPlayer) return;
            try
            {
                // TODO Fix Dispatcher Issues 
                Console.WriteLine("Asking Dispatcher " + player.Dispatcher.Thread.ManagedThreadId);
                Loggings.Logger.Log(Loggings.LogType.Sound, "Asking Dispatcher to play File '" + fileName + "' using Player " + playerId);

                var del = new Action(() =>
                {
                    Loggings.Logger.Log(Loggings.LogType.Sound, "DispatcherQueue for File '" + fileName + "' using Player " + playerId + "started");
                    Console.WriteLine("Dispatcher answered");
                    player.Close();
                    player.Open(uri);
                    if (!myPlayer) return;
                    player.Play();
                    Thread.Sleep(1000 * playLengthInSec);
                    if (!myPlayer) return;
                    player.Stop();
                    player.Close();
                    Console.WriteLine("Sound should be off");
                    Loggings.Logger.Log(Loggings.LogType.Sound, "DispatcherQueue for File '" + fileName + "' using Player " + playerId + "ended");
                    //player.Dispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Normal);
                });

                player.Dispatcher.Invoke(del);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task StopSound(int playerId)
        {
            playerTaken?.Invoke(this, playerId);
            var player = GetMediaPlayer(playerId);

            player.Stop();
            player.Close();
        }

        public async Task TerminateAllSounds()
        {
            foreach (var player in mediaPlayers)
            {
                playerTaken?.Invoke(this, player.Key);
                player.Value.Stop();
                player.Value.Close();
            }
        }
        #endregion

        private System.Windows.Media.MediaPlayer GetMediaPlayer(int id)
        {
            if (!mediaPlayers.ContainsKey(id))
            {
                Loggings.Logger.Log(Loggings.LogType.Sound, "Crated player with id " + id);
                mediaPlayers.Add(id, new System.Windows.Media.MediaPlayer());
            }
            return mediaPlayers[id];
        }

        private EventHandler<int> playerTaken;
    }
}
