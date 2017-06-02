using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bank2Kasa.ViewModel;

namespace Bank2Kasa.Service.Messages
{
    public class DeleteOperation
    {
        public DeleteOperation(OperationVM operation)
        {
            Operation = operation;
        }
        public OperationVM Operation { get; private set; }
    }
}
