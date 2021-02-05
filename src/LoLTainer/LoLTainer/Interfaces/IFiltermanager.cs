using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.Interfaces
{
    interface IFilterManager
    {
        /// <summary>
        /// Checks whether a specific Filter applies or not.
        /// </summary>
        /// <param name="appInformationProvider">The information provider providing the needed information for the filter</param>
        /// <param name="throwErrors">Defines whether this function throws Exceptions like 'no connection to API' or returns false instead</param>
        /// <returns></returns>
        bool CheckFilter(IAppInformationProvider appInformationProvider, bool throwErrors = false);

        IFilterWindow GetFilterWindow();
    }
}
