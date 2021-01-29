using LoLTainer.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.Interfaces
{
    /// <summary>
    /// Interface for SoundPlayer.
    /// </summary>
    public interface ISoundPlayer
    {
        /// <summary>
        /// Plays the provided <paramref name="fileName"/> for <paramref name="playLengthInSec"/> seconds while using the player <paramref name="playerId"/> 
        /// and therefore maybe overriding currently playing sound.
        /// </summary>
        /// <param name="playerId">ID of the music player</param>
        /// <param name="fileName">full path to sound file</param>
        /// <param name="playLengthInSec">seconds the file should be played</param>
        /// <param name="volume">volume standardized between 0 and 100. To not change the volume use -1.</param>
        /// <returns></returns>
        Task PlaySound(int playerId, string fileName,TimeSpan? startTime, TimeSpan? playLength, float volume = -1, PlayMode playMode = PlayMode.WaitPlaying);
        /// <summary>
        /// Stops the sound assosiated to a specific <paramref name="playerId"/>
        /// </summary>
        /// <param name="playerId">Id of the player</param>
        /// <returns>Task object</returns>
        Task StopSound(int playerId);
        /// <summary>
        /// Terminates all Sound that is played right now and cleares all caches
        /// </summary>
        /// <returns></returns>
        Task TerminateAllSounds();
        /// <summary>
        /// Cleares the cashe of all players
        /// </summary>
        /// <returns></returns>
        Task ClearCaches();
    }
}
