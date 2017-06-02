using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Command;

using Microsoft.Practices.ServiceLocation;

using WUKasa;
using Bank2Kasa.Service;
using Bank2Kasa.Service.Messages;


namespace Bank2Kasa.ViewModel
{
    public class OperationVM : GalaSoft.MvvmLight.ViewModelBase
    {
        private Operation operation;
        private IOperationService operationService;

        public OperationVM()
        {
            this.operation = new Operation();
            IsIgnore = false;
            IsEditMode = false;
            CreateCommands();
            operationService = ServiceLocator.Current.GetInstance<IOperationService>();
        }

        [GalaSoft.MvvmLight.Ioc.PreferredConstructor]
        public OperationVM(Operation operation) : this()
        {
            this.operation = operation;
        }

        #region Commands

        public RelayCommand ToggleAction { get; set; }
        public RelayCommand StartEdit { get; set; }
        public RelayCommand EndEdit { get; set; }

        public RelayCommand DeleteSelf{ get; set; }

        #endregion

        #region private methods

        private void CreateCommands()
        {
            ToggleAction = new RelayCommand(ToggleCurrentAction);
            StartEdit = new RelayCommand(() => Edit(true));
            EndEdit = new RelayCommand(() => Edit(false));
            DeleteSelf = new RelayCommand(Delete);
        }

        private void Delete()
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<DeleteOperation>(new DeleteOperation(this));
        }

        private void ToggleCurrentAction()
        {
            Action = Action.Next();
        }

        private void Edit(bool start)
        {
            IsEditMode = !IsEditMode;
        }
        #endregion

        private bool isEditMode;
        public bool IsEditMode
        {
            get
            {
                return isEditMode;
            }
            set
            {
                isEditMode = value;
                RaisePropertyChanged(nameof(IsEditMode));
                RaisePropertyChanged(nameof(IsNotEditMode));
            }
        }
        public bool IsNotEditMode
        {
            get
            {
                return !isEditMode;
            }
        }

        private ActionToDo action { get; set; }
        public ActionToDo Action
        {
            get
            {
                if (operation is ImportedOperation)
                    return ((ImportedOperation)operation).Action;
                else
                    return (ActionToDo)100; // vaue which for sure is above the upper bound of the enum
            }
            set
            {
                if (operation is ImportedOperation)
                {
                    ((ImportedOperation)operation).Action = value;
                    RaisePropertyChanged(nameof(Action));
                    RaisePropertyChanged(nameof(ActionString));
                }
            }
        }

        public string ActionString
        {
            get
            {
                switch (Action)
                {
                    case ActionToDo.Add2KasaAndRemoveFromImport: return "Dodaj do Kasy, usuń z importu";
                    case ActionToDo.AnnotateInKasa: return "Oznacz istniejący w Kasie";
                    case ActionToDo.RemoveFromImport: return "Tylko usuń z importu";
                    default: return "Nic nie rób";
                }
            }
        }

        public bool IsIgnore { get; set; }

        public DateTime Date
        {
            get
            {
                return operation.Date;
            }
            set
            {
                operation.Date = value;
                RaisePropertyChanged(nameof(Date));
            }
        }

        public string OperationType
        {
            get
            {
                return operation.OperationType;
            }
            set
            {
                if (!operation.OperationType.Equals(value))
                    {
                    operation.OperationType = value;
                    RaisePropertyChanged(nameof(OperationType));
                    RaisePropertyChanged(nameof(OperationTypeName));
                    IsIncome = operationService.GetOperationIncome(OperationType);
                }
            }
        }

        public string OperationTypeName
        {
            get
            {
                return operationService.GetOperationTypeName(operation.OperationType);
            }

        }

        public string Description
        {
            get
            {
                return operation.Description;
            }
            set
            {
                operation.Description = value;
                RaisePropertyChanged(nameof(Description));
            }
        }

        public string BankOperationDescription
        {
            get
            {
                if (operation is ImportedOperation)
                    return ((ImportedOperation)operation).BankOperationType;
                else
                    return "";
            }
            set
            {
                if (operation is ImportedOperation)
                {
                    ((ImportedOperation)operation).BankOperationType = value;
                    RaisePropertyChanged(nameof(BankOperationDescription));
                }
            }
        }

        public string FullDescription
        {
            get
            {
                if (operation is ImportedOperation)
                    return ((ImportedOperation)operation).FullDescription;
                else
                    return "";
            }
            set
            {
                if (operation is ImportedOperation)
                {
                    ((ImportedOperation)operation).FullDescription = value;
                    RaisePropertyChanged(nameof(FullDescription));
                }
            }
        }


        public bool IsIncome
        {
            get
            {
                return operation.IsIncome;
            }
            set
            {
                operation.IsIncome = value;
                RaisePropertyChanged(nameof(IsIncome));
                RaisePropertyChanged(nameof(AmountIn));
                RaisePropertyChanged(nameof(AmountOut));
            }
        }

        public decimal Amount
        {
            get
            {
                return operation.Amount;
            }
            set
            {
                operation.Amount = value;
                RaisePropertyChanged(nameof(Amount));
                RaisePropertyChanged(nameof(AmountIn));
                RaisePropertyChanged(nameof(AmountOut));
                if (IsIncome)
                {
                    MoneyIn = value;
                }
                else
                {
                    MoneyOut = value;
                }
            }
        }

        public decimal AmountIn
        {
            get
            {
                if (IsIncome)
                    return Amount;
                else
                    return 0;
            }
        }

        public decimal AmountOut
        {
            get
            {
                if (IsIncome)
                    return 0;
                else
                    return Amount;
            }
        }


        public decimal MoneyIn
        {
            get
            {
                return operation.MoneyIn;
            }
            set
            {
                operation.MoneyIn = value;
                RaisePropertyChanged(nameof(MoneyIn));
            }
        }

        public decimal MoneyOut
        {
            get
            {
                return operation.MoneyOut;
            }
            set
            {
                operation.MoneyOut = value;
                RaisePropertyChanged(nameof(MoneyOut));
            }
        }
    }
}
