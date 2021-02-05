using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.Models
{
    [Serializable]
    public class PropertyBundle : BaseBundle
    {
        public PropertyBundle()
        {

        }

        private FilterBundle _filterBundle;
        public FilterBundle FilterBundle
        {
            get => _filterBundle;
            set
            {
                _filterBundle = value;
                NotifyPropertyChanged();
            }
        }

        private Misc.ActionManager _actionManager;
        public Misc.ActionManager ActionManager
        {
            get => _actionManager;
            set
            {
                _actionManager = value;
                NotifyPropertyChanged();
            }
        }
    }
}
