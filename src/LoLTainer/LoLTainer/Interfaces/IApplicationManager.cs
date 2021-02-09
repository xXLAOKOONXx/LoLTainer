using LoLTainer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.Interfaces
{
    public interface IApplicationManager
    {
        IEnumerable<IActionAPIManager> ActionAPIManagers { get; }
        IEnumerable<IEventAPIManager> EventAPIManagers { get; }
        EventActionSetting EventActionSetting { get; }
        IEnumerable<Misc.Event> AllAvailableEvents { get; }
        void OpenNewSetting(string fileName);
        IActionAPIManager GetActionAPIManager(Misc.ActionManager actionManager);
        IEnumerable<Misc.ActionManager> GetAvailableActionManagers();
        void SaveChanges();
    }
}
