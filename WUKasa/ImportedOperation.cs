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
    }

    public class ImportedOperationEventArgs : EventArgs
    {
        public ImportedOperation Operation;
    }
}
