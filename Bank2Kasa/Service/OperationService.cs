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
        void Save(List<OperationVM> list);
        ObservableCollection<OperationVM> ImportFromFile(SupportedImport importType, string filename, string trashold);
        OperationListSettings LoadSettings();
        void SaveSettings(OperationListSettings settings);
        string GetOperationTypeName(string operationTypeCode);
        void SetKasaFolder(string folder);
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

        private void EnsureOperationTypes()
        {
            if (_OperationTypes == null)
            {
                WUKasa.OperationTypeStore store = new OperationTypeStore(_KasaFolder);
                _OperationTypes = store.GetAll();
            }
        }


        public ObservableCollection<OperationVM> ImportFromFile(SupportedImport importType, string dataFilename, string trashold)
        {


            switch (importType)
            {
                case SupportedImport.mBankCsv:
                    {
                        return ImportFromMBankCsv(dataFilename, trashold);
                    }
                default: return new ObservableCollection<OperationVM>();
            }
        }

        private ObservableCollection<OperationVM> ImportFromMBankCsv(string dataFilename, string trashold)
        {
            ObservableCollection<OperationVM> list = new ObservableCollection<OperationVM>();
            var importer = new mBankData.CsvExportProvider();

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
