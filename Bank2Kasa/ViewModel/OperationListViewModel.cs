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
            LoadSettings();
        }

        [GalaSoft.MvvmLight.Ioc.PreferredConstructor]
        public OperationListViewModel(IOperationService oprService, IDialogService dialogService) : this()
        {
            this.operationService = oprService;
            this.dialogService = dialogService;
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

        #endregion

        #region private methods

        private void CreateCommands()
        {
            Save = new RelayCommand(SaveData);
            Import = new RelayCommand(ImportData);

        }

        private void LoadSettings()
        {

        }

        private void SaveSettings()
        {
            // todo
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
