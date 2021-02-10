using LoLTainer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.Interfaces
{
    public interface IActionAPIManager
    {
        Dictionary<string, Type> PropertyList
        {
            get;
        }

        IActionWindow GetActionWindow();

        bool IsValidPropertyBundle(PropertyBundle propertyBundle);

        void PerformAction(PropertyBundle propertyBundle, EventTriggeredEventArgs eventTriggeredEventArgs = null);
        void Connect();
        void DisConnect();
        void ReConnect();

        bool Connected
        {
            get;
        }

    }
}
