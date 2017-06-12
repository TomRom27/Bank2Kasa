using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUKasa
{
    public class OperationCache
    {
        private List<StoredOperation> _StoredOperations;

        public OperationCache()
        {
            _StoredOperations = new List<StoredOperation>();
        }

        public void Add(Operation o, int position)
        {
            _StoredOperations.Add(new StoredOperation() { Operation = o, Position = position });
        }

        public StoredOperation FindByDCAFirstPosition(Operation o)
        {
            var found = _StoredOperations.Find((so) => so.Operation.Date.Equals(o.Date) &&
                                so.Operation.OperationCode.Equals(o.OperationCode) &&
                                so.Operation.Account.Equals(o.Account));

            return found;
        }
    }
}
