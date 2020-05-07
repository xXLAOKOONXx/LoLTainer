using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.Misc
{
    /// <summary>
    /// Events that are supported in this application.
    /// Implementation for support can be found in <see cref="API.InGameEventMapper"/>.
    /// </summary>
    public enum Event
    {
        /* !IMPORTANT!
         * When adding a new event here ensure support in API.InGameEventMapper
         */
        PlayerKill = 11,
        PlayerDoubleKill = 12,
        PlayerTripleKill = 13,
        PlayerQuodraKill = 14,
        PlayerPentaKill = 15,
        //TeamKill = 21,
        //TeamDoubleKill = 22,
        //TeamTripleKill = 23,
        //TeamQuodraKill = 24,
        //TeamPentaKill = 25,
        //PlayerTurretDestroyed = 31,
        //PlayerInhibitorDestroyed = 32,
        //TeamTurretDestroyed = 41,
        //TeamInhibitorDestroyed = 42,
        PlayerDragonKill = 51,
        PlayerBaronKill = 52,
        //PlayerDragonKillAssist = 53,
        //PlayerBaronKillAssist = 54,
        //PlayerDragonSteal = 55,
        //PlayerBaronSteal = 56,
        //TeamDragonSteal = 57,
        //TeamBaronSteal = 58
    }
}
