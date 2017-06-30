using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Command;

using MvvmLight.Extensions;

using Bank2Kasa.Service.Messages;
using Bank2Kasa.Service;
using WUKasa;


namespace Bank2Kasa.ViewModel
{
    public class KasaOperationListViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private IOperationService operationService;
        private IDialogService dialogService;

        public KasaOperationListViewModel()
        {
            Operations = new ObservableCollection<OperationVM>();

            //if (GalaSoft.MvvmLight.ViewModelBase.IsInDesignModeStatic)
            //{
            Operations.Add(new OperationVM() { Action = WUKasa.ActionToDo.Add2KasaAndRemoveFromImport, Amount = 100, OperationType = "10", Description = "Operacja przychodowa", Date = DateTime.Today });
            Operations.Add(new OperationVM() { Action = WUKasa.ActionToDo.Add2Kasa, Amount = 100, OperationType = "15", Description = "Płatność przelewem", Date = DateTime.Today });

            //}

            CreateCommands();
            SubscribeToMessages();
            Settings = new OperationListSettings();
            SelectedMonthIndex = 0;
            PopulateMonths();

            IsReading = true;
        }


        [GalaSoft.MvvmLight.Ioc.PreferredConstructor]
        public KasaOperationListViewModel(IOperationService oprService, IDialogService dialogService) : this()
        {
            this.operationService = oprService;
            this.dialogService = dialogService;
            LoadSettings();
        }

        private void CreateCommands()
        {
            ShowKasa1 = new RelayCommand(ShowKasa1Operations);
            ShowKasa2 = new RelayCommand(ShowKasa2Operations);
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



        // Months}" SelectedIndex="{Binding SelectedMonthIndex}
        private int _SelectedMonthIndex;
        public int SelectedMonthIndex
        {
            get { return _SelectedMonthIndex; }
            set
            {
                _SelectedMonthIndex = value;
                RaisePropertyChanged(nameof(SelectedMonthIndex));
            }
        }

        private List<string> _Months = new List<string>();
        public List<string> Months
        {
            get { return _Months; }
            set
            {
                _Months = value;
                RaisePropertyChanged(nameof(Months));
            }
        }


        private bool _IsReading;
        public bool IsReading
        {
            get { return _IsReading; }
            set
            {
                _IsReading = value;
                RaisePropertyChanged(nameof(IsReading));
            }
        }

        public RelayCommand ShowKasa1 { get; set; }
        public RelayCommand ShowKasa2 { get; set; }



        private void PopulateMonths()
        {
            Months.Clear();
            Months.Add("<bez fitrowania>");
            for(int i=1; i<=12;i++)
            {
                Months.Add(new DateTime(1, i, 1).ToString("MMMM"));
            }            
        }

        private void SubscribeToMessages()
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<Service.Messages.SettingsChanged>(this, RefreshSettings);
        }

        private void RefreshSettings(SettingsChanged e)
        {
            if (e.Settings != null)
                Settings = e.Settings;
        }

        private void LoadSettings()
        {
            Settings = operationService.LoadSettings();
        }

        private void ShowKasa1Operations()
        {
            ShowKasaOperations(Settings.Year, Settings.KasaFolder1, SelectedMonthIndex);
        }

        private void ShowKasa2Operations()
        {
            ShowKasaOperations(Settings.Year, Settings.KasaFolder2, SelectedMonthIndex);
        }

        private void ShowKasaOperations(int KasaYear, string KasaFolder, int month)
        {

            Task.Factory
                    /* in fact synchronously - as we use current sync context */
                    .StartNew(() =>
                    {
                        IsReading = true;
                        System.Windows.Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                                new Action(() =>
                                {
                                    using (OperationStore store = new OperationStore(KasaYear, KasaFolder))
                                    {
                                        List<Operation> list = new List<Operation>();
                                        store.ForEach((o, i) => 
                                            {
                                                if ((month==0) ||
                                                     ((o.Date.Month>=month-1) && (o.Date.Month <= month + 1)))
                                                list.Add(o);
                                            }
                                        );
                                        list.Sort((o1, o2) => { return o1.Date.CompareTo(o2.Date); });
                                        Operations.Clear();

                                        Operations = new ObservableCollection<OperationVM>(list.Select((o) => new OperationVM(o)).ToList());
                                    };
                                })
                            );
                        IsReading = false;
                    })
                    /* when completed, display response */
                    .ContinueWith((t) =>
                    {
                        IsReading = false;
                        if (t.Exception != null)
                        {
                            dialogService.ShowError("Coś poszło źle:\n" + t.Exception.InnerException.Message, "Błąd", "OK", null);
                        }
                    });

        }
    }
}