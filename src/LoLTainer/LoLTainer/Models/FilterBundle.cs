using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLTainer.Models
{
    public class FilterBundle : BaseBundle
    {
        private Misc.FilterType _actionManager;
        public Misc.FilterType ActionManager
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
