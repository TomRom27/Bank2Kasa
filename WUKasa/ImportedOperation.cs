using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUKasa
{

    public enum ActionToDo
    {
        Add2KasaAndRemoveFromImport = 0, RemoveFromImport = 1, AnnotateInKasa = 2, Add2Kasa = 3
    }

    public static class ActionToDoExtension
    {
        public static ActionToDo Next(this ActionToDo a)
        {
            return (ActionToDo)((((int)a) + 1) % Enum.GetValues(typeof(ActionToDo)).Length);
        }
    }


    public class ImportedOperation : Operation
    {
        public object OperationOrigin { get; set; }

        public string BankOperationType { get; set; }

        public string FullDescription { get; set; }

        public ActionToDo Action { get; set; }
    }
}
