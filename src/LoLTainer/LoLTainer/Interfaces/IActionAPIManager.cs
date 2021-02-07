using LoLTainer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.Interfaces
{
    interface IActionAPIManager
    {
        Dictionary<string, Type> PropertyList
        {
            get;
        }

        IActionWindow GetActionWindow(Action<PropertyBundle> finishedEditingAction, PropertyBundle propertyBundle);

        bool IsValidPropertyBundle(PropertyBundle propertyBundle);

        void PerformAction(PropertyBundle propertyBundle, EventTriggeredEventArgs eventTriggeredEventArgs = null);

        bool Connected
        {
            get;
        }

    }
}
