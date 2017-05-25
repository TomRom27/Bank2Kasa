using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bank2Kasa.ViewModel;
using WUKasa;

namespace Bank2Kasa.Service
{

    public interface IOperationService
    {
        void Save(List<OperationVM> list);
        ObservableCollection<OperationVM> ImportFromFile(SupportedImport importType, string filename, string trashold);

    }

    public class OperationService : IOperationService
    {
        public ObservableCollection<OperationVM> ImportFromFile(SupportedImport importType, string dataFilename, string trashold)
        {
            switch (importType)
            {
                case SupportedImport.mBankCsv:
                    {
                        return ImportFromMBankCsv(dataFilename, new ImportConfiguration() , trashold);
                    }
                default: return new ObservableCollection<OperationVM>();
            }
        }

        private ObservableCollection<OperationVM> ImportFromMBankCsv(string dataFilename, ImportConfiguration cfg, string trashold)
        {
            ObservableCollection<OperationVM> list = new ObservableCollection<OperationVM>();
            var importer = new mBankData.CsvExportProvider(cfg);

            importer.OperationImported += delegate (object sender, ImportedOperation args)
            {
                list.Add(new OperationVM(args));
            };

            importer.Import(dataFilename, trashold);
            return list;
        }

        public void Save(List<OperationVM> list)
        {

        }
    }
}
