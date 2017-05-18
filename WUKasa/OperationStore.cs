using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BTreeFileUtil;

namespace WUKasa
{
    public class OperationStore
    {
        private int currentMax;
        private BTreeFile btreeFile;

        public OperationStore(int year)
        {
            currentMax = -1;
            //btreeFile = new BTreeFile()
        }

        public void Add(Operation operation)
        {
            EnsureMax();
            operation.Max = currentMax;
            btreeFile.Add(operation);
            currentMax++;
        }

        private void EnsureMax()
        {
            // todo
        }
    }
}
