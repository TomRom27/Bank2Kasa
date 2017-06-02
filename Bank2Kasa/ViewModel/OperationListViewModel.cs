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
            SubscribeToMessages();
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

        private bool _IsImporting;
        public bool IsImporting
        {
            get { return _IsImporting; }
            set
            {
                _IsImporting = value;
                RaisePropertyChanged(nameof(IsImporting));
            }
        }

        #region Commands

        public RelayCommand Save { get; set; }
        public RelayCommand Import { get; set; }
        public RelayCommand SelectKasa { get; set; }
        public RelayCommand SelectImport { get; set; }
        //public RelayCommand DeleteOperation{ get; set; }

        #endregion

        #region private methods

        private void CreateCommands()
        {
            Save = new RelayCommand(SaveData);
            Import = new RelayCommand(ImportData);
            SelectKasa = new RelayCommand(SelectKasaFolder);
            SelectImport = new RelayCommand(SelectImportFile);
            //DeleteOperation = new RelayCommand(DeleteGivenOperation);
        }

        private void SubscribeToMessages()
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<Service.Messages.DeleteOperation>(this, DeleteGivenOperation);
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

        //private void ImportData2()
        //{
        //    IsImporting = true;
        //    try
        //    {
        //        Operations = operationService.ImportFromFile(SupportedImport.mBankCsv, Settings.ImportFile, Settings.Trashold);
        //        IsImporting = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        IsImporting = false;
        //        dialogService.ShowError("Coś poszło źle:\n" + ex.Message, "Błąd", "OK", null);
        //    }

        //    SaveSettings();
        //    IsImporting = false;
        //}

        private void ImportData()
        {
            dialogService.SetBusy();
            Task.Factory
            /* in fact synchronously - as we use current sync context */
            .StartNew(() =>
            {
                IsImporting = true;
                Operations = operationService.ImportFromFile(SupportedImport.mBankCsv, Settings.ImportFile, Settings.Trashold);

            })
            /* when completed, display response */
            .ContinueWith((t) =>
            {
                IsImporting = false;
                if (t.Exception != null)
                {
                    dialogService.ShowError("Coś poszło źle:\n" + t.Exception.Message, "Błąd", "OK", null);
                }
                else
                {
                }
            });
            SaveSettings();
            dialogService.SetNormal();
        }

        private void SaveData()
        {
            // todo
            SaveSettings();
        }

        private void DeleteGivenOperation(Service.Messages.DeleteOperation message)
        {
            dialogService.ShowMessage("Usunąć operację " + message.Operation.Description + " ?", "Usuwanie operacji",
                buttonConfirmText: "Tak", buttonCancelText: "Nie",
                                            afterHideCallback: (confirmed) =>
                                            {
                                                if (confirmed && Operations.First((o) => o == message.Operation) != null)
                                                {
                                                    Operations.Remove(message.Operation);
                                                }

                                            });
        }
        #endregion
    }
}
