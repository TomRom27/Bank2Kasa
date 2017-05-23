using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WUKasa;
using Bank2Kasa.ViewModel;

namespace Bank2Kasa.Service
{
    public class DataImporter
    {
        public List<OperationVM> Import(SupportedImport importType, string dataFilename, ImportConfiguration cfg)
        {
            switch (importType)
            {
                case SupportedImport.mBankCsv:
                    {
                        return ImportFromMBankCsv(dataFilename, cfg);
                    }
                default: return new List<OperationVM>();
            }
        }

        private List<OperationVM> ImportFromMBankCsv(string dataFilename, ImportConfiguration cfg)
        {
            List<OperationVM> list = new List<OperationVM>();
            var extractor = new mBankData.CsvExportProvider(cfg);

            extractor.OperationImported += delegate (object sender, ImportedOperation args)
            {
                list.Add(new OperationVM(args));
            };

            return list;
        }
    }
}
