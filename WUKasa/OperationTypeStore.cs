using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using BTreeFileUtil;

namespace WUKasa
{
    class OperationTypeStore
    {
        public const string FileName = "$TYP.DAT";
        private BTreeFile<OperationType> btreeFile;

        public OperationTypeStore(string path)
        {
            btreeFile = new BTreeFile<OperationType>(System.IO.Path.Combine(path, FileName));
        }

        public IList<OperationType> GetAll()
        {
            var list = new List<OperationType>();

            btreeFile.Open();

            for (int i = 1; i <= btreeFile.TotalRecordNumber - 1; i++)
            {
                OperationType oprType = btreeFile.Get(i);
                if (!oprType.isDeleted)
                    list.Add(oprType);
            }
            return list;
        }
    }
}

