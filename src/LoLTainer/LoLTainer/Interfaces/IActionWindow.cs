using LoLTainer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.Interfaces
{
    interface IActionWindow
    {
        void Open(Action<PropertyBundle> finishedEditingAction, PropertyBundle propertyBundle);
    }
}
