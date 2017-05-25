using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUKasa
{
    public class ImportedOperation : Operation
    {
        public object OperationOrigin { get; set; }

        public string BankOperationType { get; set; }
        public string FullDescription { get; set; }
    }
}
