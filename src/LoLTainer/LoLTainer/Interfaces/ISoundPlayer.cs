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
        /// 
        /// </summary>
        /// <param name="playerId">ID of the music player</param>
        /// <param name="fileName">full path to sound file</param>
        /// <param name="playLengthInSec">seconds the file should be played</param>
        /// <returns></returns>
        Task PlaySound(int playerId, string fileName, int playLengthInSec);
        Task StopSound(int playerId);

        Task TerminateAllSounds();
        Task ClearCaches();
    }
}
