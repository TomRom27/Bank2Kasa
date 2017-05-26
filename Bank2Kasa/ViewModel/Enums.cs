using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank2Kasa.ViewModel
{
    public enum SupportedImport
    {
        mBankCsv
    }

    public enum ActionToDo
    {
        Add2KasaAndRemoveFromImport, RemoveFromImport, AnnotateInKasa
    }

    public static class ActionToDoExtension
    {
        public static ActionToDo Next(this ActionToDo a)
        {
            return (ActionToDo)((((int)a) + 1) % Enum.GetValues(typeof(ActionToDo)).Length);
        }
    }
}
