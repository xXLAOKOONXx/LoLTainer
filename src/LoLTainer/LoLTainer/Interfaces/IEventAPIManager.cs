using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.Interfaces
{
    public interface IEventAPIManager
    {
        IEnumerable<Misc.Event> GetSupportedEvents();

        EventHandler<Models.EventTriggeredEventArgs> EventHandler { get; set; }

        void Connect();

        void DisConnect();

        void RestartConnection();

        void SetActiveEvents(IEnumerable<Misc.Event> events);

        bool Connected
        {
            get;
        }
    }
}
