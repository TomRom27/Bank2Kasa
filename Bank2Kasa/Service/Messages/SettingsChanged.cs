using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bank2Kasa.ViewModel;

namespace Bank2Kasa.Service.Messages
{
    public class SettingsChanged
    {
        public SettingsChanged(OperationListSettings settings)
        {
            Settings = settings;
        }
        public OperationListSettings Settings { get; private set; }
    }
}
