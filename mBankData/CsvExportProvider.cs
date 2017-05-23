using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WUKasa;

namespace mBankData
{
    public class CsvExportProvider
    {
        public event EventHandler<ImportedOperation> OperationImported;
        private ImportConfiguration cfg;

        public CsvExportProvider(ImportConfiguration cfg)
        {
            this.cfg = cfg;
        }
        public void Import(string filename)
        {
            //this.filename = filename;
        }

        public void RemovedImported(List<Object> operationOrigins)
        {
            // todo
        }
    }
}
