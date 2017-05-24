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
        ObservableCollection<OperationVM> ImportFromFile(string filename, string trashold);

    }

    public class OperationService : IOperationService
    {
        public ObservableCollection<OperationVM> ImportFromFile(string filename, string trashold)
        {
            ObservableCollection<OperationVM> collection = new ObservableCollection<OperationVM>();

            DataImporter importer = new DataImporter();

            var list = importer.Import(SupportedImport.mBankCsv, "mBank.csv", new ImportConfiguration());
            foreach (var o in list)
            {
                collection.Add(o);
            }

            return collection;
        }

        public void Save(List<OperationVM> list)
        {

        }
    }
}
