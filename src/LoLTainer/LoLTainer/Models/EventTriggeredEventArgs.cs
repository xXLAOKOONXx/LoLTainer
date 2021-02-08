using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoLTainer.Misc;

namespace LoLTainer.Models
{
    public class EventTriggeredEventArgs : BaseBundle
    {
        public EventTriggeredEventArgs()
        {

        }

        public EventTriggeredEventArgs(Event @event, Dictionary<string, object> EventArgs = null)
        {
            this._event = @event;
            this.EventArgs = EventArgs;
        }

        private Misc.Event _event;

        public Event Event
        {
            get => _event;
            set
            {
                _event = value;
            }
        }

        public Dictionary<string, object> EventArgs
        {
            get => Properties;
            set => Properties = value;
        }
    }
}
