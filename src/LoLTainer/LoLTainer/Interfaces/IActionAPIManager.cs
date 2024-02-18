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
        /// <summary>
        /// Get the Window to edit or create an Action of this Action API Manager
        /// </summary>
        /// <returns></returns>
        IActionWindow GetActionWindow();

        /// <summary>
        /// Checks wether the propertybundle is valid for this action manager
        /// </summary>
        /// <param name="propertyBundle"></param>
        /// <returns></returns>
        bool IsValidPropertyBundle(PropertyBundle propertyBundle);

        /// <summary>
        /// Perform the action
        /// </summary>
        /// <param name="propertyBundle"></param>
        /// <param name="eventTriggeredEventArgs">EventArgs coming from the event that triggered the action</param>
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
