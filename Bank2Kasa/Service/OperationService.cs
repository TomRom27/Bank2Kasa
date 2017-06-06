using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Bank2Kasa.ViewModel;
using WUKasa;
using WUKasa.Config;

namespace Bank2Kasa.Service
{

    public interface IOperationService
    {
        void Save(IList<OperationVM> list);
        ObservableCollection<OperationVM> ImportFromFile(SupportedImport importType, string filename, string trashold, bool aggregateDay);
        OperationListSettings LoadSettings();
        void SaveSettings(OperationListSettings settings);
        void SetKasaFolder(string folder);
        string GetOperationTypeName(string operationTypeCode);
        bool GetOperationIncome(string operationTypeCode);
    }

    public class OperationService : IOperationService
    {
        private List<WUKasa.OperationType> _OperationTypes = null;
        private string _KasaFolder;

        public void SetKasaFolder(string folder)
        {
            _KasaFolder = folder;
        }

        public string GetOperationTypeName(string operationTypeCode)
        {
            try
            {
                EnsureOperationTypes();
                var operationType = _OperationTypes.Find((ot) => ot.OperationTypeCode.Equals(operationTypeCode));
                if (operationType != null)
                    return operationType.Description;
                else
                    return $"Nie znaleziono nazwy dla {operationTypeCode}";
            }
            catch (Exception ex)
            {
                return $"Problem z nazwą dla {operationTypeCode}: {ex.Message}";
            }
        }

        public bool GetOperationIncome(string operationTypeCode)
        {
            try
            {
                EnsureOperationTypes();
                var operationType = _OperationTypes.Find((ot) => ot.OperationTypeCode.Equals(operationTypeCode));
                if (operationType != null)
                    return operationType.IsIncome;
                else
                {
                    System.Diagnostics.Trace.WriteLine($"Nie znaleziono {operationTypeCode}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                return true;
                System.Diagnostics.Trace.WriteLine($"Problem z {operationTypeCode}: {ex.Message}");
            }
        }


        private void EnsureOperationTypes()
        {
            if (_OperationTypes == null)
            {
                WUKasa.OperationTypeStore store = new OperationTypeStore(_KasaFolder);
                _OperationTypes = store.GetAll();
            }
        }


        public ObservableCollection<OperationVM> ImportFromFile(SupportedImport importType, string dataFilename, string trashold, bool aggregateDay)
        {
            System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("yyyy.MM.dd HH.mm.ss") + " Import z pliku "+dataFilename);
            switch (importType)
            {
                case SupportedImport.mBankCsv:
                    {
                        System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("yyyy.MM.dd HH.mm.ss") + " Import z pliku - koniec");
                        return ImportFromMBankCsv(dataFilename, trashold, aggregateDay);
                    }
                default: return new ObservableCollection<OperationVM>();
            }
        }

        private ObservableCollection<OperationVM> ImportFromMBankCsv(string dataFilename, string trashold, bool aggregateDay)
        {
            List<ImportedOperation> list;
            var importer = new mBankData.CsvExportProvider();

            list = importer.Import(dataFilename, trashold, aggregateDay);

            return new ObservableCollection<OperationVM>(list.Select(oi => new OperationVM(oi)).ToList());
        }

        public void Save(IList<OperationVM> list)
        {
            // todo
            System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("yyyy.MM.dd HH.mm.ss") + " Zapisuję dane do kasy");
            foreach (var o in list)
            {
                System.Diagnostics.Trace.WriteLine(o.Date.ToString("dd.MM.yyyy") + " " + o.OperationType + " " +
                                                o.Description.PadRight(35) + " " + ((o.IsIncome) ? "+" : "-") + " " +
                                                o.Amount.ToString().PadLeft(10) + " " + o.MoneyIn.ToString().PadLeft(10) + " " + o.MoneyOut.ToString().PadLeft(10) + " " +
                                                o.ActionString.PadRight(32)+" "+o.Max.ToString().PadLeft(5));
            }
            System.Diagnostics.Trace.WriteLine(DateTime.Now.ToString("yyyy.MM.dd HH.mm.ss") + " Zapis do kasy - koniec");
        }

        public OperationListSettings LoadSettings()
        {
            if (!System.IO.File.Exists(SettingsFile))
            {
                System.Diagnostics.Trace.WriteLine("File settigns not found: " + SettingsFile);
                return new OperationListSettings() { ImportFile = @"c:\desktop\my.import.txt", KasaFolder = @"c:\desktop", Trashold = "01", Year = 2017 };
            }

            var content = System.IO.File.ReadAllLines(SettingsFile);
            try
            {
                return SerializationHelper.Deserialize<OperationListSettings>(content.Aggregate((i, j) => (i + j)));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Failed to read settings from " + SettingsFile + "\n" + ex.Message);
                return new OperationListSettings();
            }

        }

        public void SaveSettings(OperationListSettings settings)
        {
            var s = SettingsFile;

            var content = SerializationHelper.Serialize(settings);
            try
            {
                System.IO.File.WriteAllLines(SettingsFile, new string[] { content });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Failed to save settings to " + SettingsFile + "\n" + ex.Message);
            }
        }

        private string SettingsFile
        {
            get
            {
                return System.Reflection.Assembly.GetEntryAssembly().Location + ".settings";
            }
        }
    }
}
