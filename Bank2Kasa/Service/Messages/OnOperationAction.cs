using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bank2Kasa.ViewModel;

namespace Bank2Kasa.Service.Messages
{
    public class OnOperationAction
    {
        public OnOperationAction(OperationVM operation)
        {
            Operation = operation;
        }
        public OperationVM Operation { get; private set; }
    }

    public class DeleteOperation : OnOperationAction
    {
        public DeleteOperation(OperationVM operation) : base(operation)
        {
        }
    }
    public class CopyOperation : OnOperationAction
    {
        public CopyOperation(OperationVM operation) : base(operation)
        {
        }
    }
}
