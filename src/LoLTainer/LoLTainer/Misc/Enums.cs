using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.Misc
{
    public enum ConnectionStatus
    {
        Disconnected = 0,
        [Description("Trying to connect")]
        TryingToConnect = 100,
        Connected = 200
    }

    /// <summary>
    /// Events that are supported in this application.
    /// Implementation for support can be found in <see cref="API.InGameEventMapper"/> or <see cref="API.LCUEventMapper"/>.
    /// </summary>
    public enum Event
    {
        /* !IMPORTANT!
         * When adding a new event here ensure support in API.InGameEventMapper or API.LCUEventMapper
         */
        PlayerAnyKill = 10,
        PlayerSingleKill = 11,
        PlayerDoubleKill = 12,
        PlayerTripleKill = 13,
        PlayerQuodraKill = 14,
        PlayerPentaKill = 15,
        PlayerFirstBlood = 16,
        //TeamKill = 21,
        //TeamDoubleKill = 22,
        //TeamTripleKill = 23,
        //TeamQuodraKill = 24,
        //TeamPentaKill = 25,
        //PlayerTurretDestroyed = 31,
        //PlayerInhibitorDestroyed = 32,
        //TeamTurretDestroyed = 41,
        //TeamInhibitorDestroyed = 42,
        [Description("TeamNexusDestroyed (Loose)")] TeamNexusDestroyed = 45,
        [Description("EnemyTeamNexusDestroyed (Win)")] EnemyTeamNexusDestroyed = 46,
        AnyNexusDestroyed = 47,
        PlayerDragonKill = 51,
        PlayerBaronKill = 52,
        //PlayerDragonKillAssist = 53,
        //PlayerBaronKillAssist = 54,
        PlayerDragonSteal = 55,
        PlayerBaronSteal = 56,
        //TeamDragonSteal = 57,
        //TeamBaronSteal = 58

        StartGame = 900,
        /*
         * --LCU--
         */
        EnterChampSelect = 1010,
        EnterGame = 1020,
        EndGame = 1030,
        EnterMatchmaking = 1040,
        EnterLobby = 1050
    }

    public enum ActionManager
    {
        SoundPlayer = 0,
        OBS = 50
    }

    public enum FilterType
    {
        NoFilter = 0,
        Champions = 10,
        Queue = 20
    }

    public enum OBSActionType
    {
        Scene = 10,
        ItemVisibility = 20,
        FilterVisibility = 30
    }

    public enum PlayMode
    {
        /// <summary>
        /// The sound will wait until the current sound from the group is finished
        /// </summary>
        WaitPlaying,
        /// <summary>
        /// The sound will stop the current sound from the group and start immediately
        /// </summary>
        StopPlaying,
        /// <summary>
        /// Before starting to play all other sounds will be shut down
        /// </summary>
        StopAllPlaying
    }
}
