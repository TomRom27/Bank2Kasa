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

            if (GalaSoft.MvvmLight.ViewModelBase.IsInDesignModeStatic)
            {
                Operations.Add(new OperationVM() { Action = WUKasa.ActionToDo.Add2KasaAndRemoveFromImport, Amount = 100, OperationType="10", Description = "Operacja przychodowa", Date = DateTime.Today });
                Operations.Add(new OperationVM() { Action = WUKasa.ActionToDo.Add2Kasa, Amount = 100, OperationType = "15", Description = "Płatność przelewem", Date = DateTime.Today });

            }

            CreateCommands();
            SubscribeToMessages();
            Settings = new OperationListSettings();
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

        private bool _IsSaving;
        public bool IsSaving
        {
            get { return _IsSaving; }
            set
            {
                _IsSaving = value;
                RaisePropertyChanged(nameof(IsSaving));
            }
        }

        private string _SavingStatusText;
        public string SavingStatusText
        {
            get { return _SavingStatusText; }
            set
            {
                _SavingStatusText = value;
                RaisePropertyChanged(nameof(SavingStatusText));
            }
        }


        #region Commands

        public RelayCommand Save { get; set; }
        public RelayCommand Import { get; set; }
        public RelayCommand SelectKasa { get; set; }
        public RelayCommand SelectImport { get; set; }
        public RelayCommand ShowSum { get; set; }
        public RelayCommand CancelSaving { get; set; }
        #endregion

        #region private methods

        private void CreateCommands()
        {
            Save = new RelayCommand(SaveData);
            Import = new RelayCommand(ImportData);
            SelectKasa = new RelayCommand(SelectKasaFolder);
            SelectImport = new RelayCommand(SelectImportFile);
            ShowSum = new RelayCommand(ShowOperationSum);
            CancelSaving = new RelayCommand(CancelPendigSave);
        }



        private void SubscribeToMessages()
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<Service.Messages.DeleteOperation>(this, DeleteGivenOperation);
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<Service.Messages.CopyOperation>(this, CopyGivenOperation);
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

        private void ShowOperationSum()
        {
            decimal sAmount, sMoneyIn, sMoneyOut;
            sAmount = sMoneyIn = sMoneyOut = 0;
            // todo
            foreach (var o in Operations)
                o.Add(ref sAmount, ref sMoneyIn, ref sMoneyOut);

            dialogService.ShowMessage($"Suma\t={sAmount,10:N2}\nNa Plus\t={sMoneyIn,10:N2}\nNa Minus\t={sMoneyOut,10:N2}", "Suma");
        }

        private void ImportData()
        {
            dialogService.SetBusy();
            Task.Factory
            /* in fact synchronously - as we use current sync context */
            .StartNew(() =>
            {
                IsImporting = true;
                Operations = operationService.ImportFromFile(SupportedImport.mBankCsv, Settings.ImportFile, Settings.Trashold, Settings.AggregateDay);

            })
            /* when completed, display response */
            .ContinueWith((t) =>
            {
                IsImporting = false;
                if (t.Exception != null)
                {
                    dialogService.ShowError("Coś poszło źle:\n" + t.Exception.InnerException.Message, "Błąd", "OK", null);
                }
                else
                {
                }
            });
            SaveSettings();
            dialogService.SetNormal();
        }

        private void CancelPendigSave()
        {
            // todo - how to control pending saving
            IsSaving = false;
        }

        private void SaveData()
        {
            bool isSuccess = false;
            SaveOperationArgument arg = new SaveOperationArgument()
            {
                ImportFilename = Settings.ImportFile,
                KasaYear = Settings.Year,
                OperationList = Operations,
                ProgressCallback = UpdateSavingProgress,
                BackupImportFile = true, // todo - get from UI
                BackupDatFile = true, // todo - get from UI
                RemoveIxFile = true, // todo - get from UI
                IsCancelled = false
            };

            SaveSettings();

            Task.Factory
            /* in fact synchronously - as we use current sync context */
            .StartNew(() =>
            {
                IsSaving = true;
                operationService.Save(arg);
                IsSaving = false;
                System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, 
                    new Action(() => {
                                int i = 0;
                                while (i <= arg.OperationList.Count - 1)
                                {
                                    if (arg.OperationList[i].CanDelete)
                                        arg.OperationList.RemoveAt(i);
                                    else
                                        i++;
                                }

                            })
                );               

            })
            /* when completed, display response */
            .ContinueWith((t) =>
            {
                IsImporting = false;
                if (t.Exception != null)
                {
                    dialogService.ShowError("Coś poszło źle:\n" + t.Exception.InnerException.Message, "Błąd", "OK", null);
                }
            });


        }

        private void UpdateSavingProgress(string statusText, bool isFinished)
        {
            SavingStatusText = statusText; // todo
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

        private void CopyGivenOperation(Service.Messages.CopyOperation message)
        {

            OperationVM newOperation = message.Operation.Clone();
        
            var currentIndex = Operations.IndexOf(message.Operation);
            if (currentIndex >= 0)
            {
                // the new operation must be added AFTER the one we copied 
                Operations.Insert(currentIndex + 1, newOperation);
            }
        }
        #endregion
    }
}
