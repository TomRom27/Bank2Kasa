using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUKasa
{
    public class OperationCache
    {
        public List<StoredOperation> _StoredOperations;

        public OperationCache()
        {
            _StoredOperations = new List<StoredOperation>();
        }

        public void Add(Operation o, int position)
        {
            if (!o.isDeleted)
            {
                // todo temp
                //if ((o.Date.Month == 4) && (o.OperationType == "13"))
                _StoredOperations.Add(new StoredOperation() { Operation = o, Position = position });
            }
        }

        public StoredOperation FindByDCAFirstPosition(Operation o)
        {

            var found = _StoredOperations.Find((so) => so.Operation.Date.Equals(o.Date) &&
                                so.Operation.OperationType.Equals(o.OperationType) &&
                                so.Operation.Amount.Equals(o.Amount));

            return found;
        }
    }
}
