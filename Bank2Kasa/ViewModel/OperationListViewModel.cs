using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MvvmLight.Extensions;

using Bank2Kasa.Service;
using GalaSoft.MvvmLight.Command;

namespace Bank2Kasa.ViewModel
{
    public class OperationListViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private IOperationService operationService;
        private IDialogService dialogService;

        public OperationListViewModel()
        {
            Operations = new ObservableCollection<OperationVM>();

            //if (GalaSoft.MvvmLight.ViewModelBase.IsInDesignModeStatic)
            //{
            Operations.Add(new OperationVM(new WUKasa.Operation() { Amount = 100, Description = "Operacja przychodowa", Date = DateTime.Today }));

            //}

            CreateCommands();
        }

        [GalaSoft.MvvmLight.Ioc.PreferredConstructor]
        public OperationListViewModel(IOperationService oprService, IDialogService dialogService) : this()
        {
            this.operationService = oprService;
            this.dialogService = dialogService;
            LoadSettings();
        }

        ObservableCollection<OperationVM> _Operations;
        public ObservableCollection<OperationVM> Operations
        {
            get { return _Operations; }
            set
            {
                _Operations = value;
                RaisePropertyChanged(nameof(Operations));
            }
        }

        private OperationListSettings _Settings;
        public OperationListSettings Settings
        {
            get { return _Settings; }
            set
            {
                _Settings = value;
                RaisePropertyChanged(nameof(Settings));
            }
        }

        #region Commands

        public RelayCommand Save { get; set; }
        public RelayCommand Import { get; set; }
        public RelayCommand SelectKasa { get; set; }
        public RelayCommand SelectImport { get; set; }

        #endregion

        #region private methods

        private void CreateCommands()
        {
            Save = new RelayCommand(SaveData);
            Import = new RelayCommand(ImportData);
            SelectKasa = new RelayCommand(SelectKasaFolder);
            SelectImport = new RelayCommand(SelectImportFile);
        }

        private void LoadSettings()
        {
            Settings = operationService.LoadSettings();
        }

        private void SaveSettings()
        {
            operationService.SaveSettings(Settings);
        }

        private void SelectKasaFolder()
        {
            var newFolder = dialogService.SelectFolder("Wybierz katalog z plikiem Kasy", Settings.KasaFolder);
            if (!String.IsNullOrEmpty(newFolder))
                Settings.KasaFolder = newFolder;
        }

        private void SelectImportFile()
        {
            var newFile = dialogService.SelectFile("Wybierz plik do import", "*.*");
            if (!String.IsNullOrEmpty(newFile))
                Settings.ImportFile = newFile;
        }

        private void ImportData()
        {
            // todo
            Operations = operationService.ImportFromFile(SupportedImport.mBankCsv, "mbank.csv", "1");

            SaveSettings();
        }

        private void SaveData()
        {
            // todo
            SaveSettings();
        }
        #endregion
    }
}
