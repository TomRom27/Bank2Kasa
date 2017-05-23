using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WUKasa;

namespace Bank2Kasa.ViewModel
{
    public class OperationVM
    {
        private Operation operation;

        public OperationVM(Operation operation)
        {
            this.operation = operation;
            IsIgnore = false;
        }

        public bool IsIgnore { get; set; }
    }
}
