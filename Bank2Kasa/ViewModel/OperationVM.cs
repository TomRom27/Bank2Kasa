using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight.Command;

using WUKasa;

namespace Bank2Kasa.ViewModel
{
    public class OperationVM : GalaSoft.MvvmLight.ViewModelBase
    {
        private Operation operation;

        public OperationVM()
        {
            this.operation = new Operation();
            IsIgnore = false;
            IsEditMode = false;
            CreateCommands();
        }

        [GalaSoft.MvvmLight.Ioc.PreferredConstructor]
        public OperationVM(Operation operation) : this()
        {
            this.operation = operation;

            Action = ActionToDo.Add2KasaAndRemoveFromImport;
        }

        #region Commands

        public RelayCommand ToggleAction { get; set; }
        public RelayCommand StartEdit { get; set; }
        public RelayCommand EndEdit { get; set; }

        #endregion

        #region private methods

        private void CreateCommands()
        {
            ToggleAction = new RelayCommand(ToggleCurrentAction);
            StartEdit = new RelayCommand(() =>  Edit(true));
            EndEdit = new RelayCommand(() => Edit(false));
        }


        private void ToggleCurrentAction()
        {
            Action = Action.Next();
        }

        private void Edit(bool start)
        {
            IsEditMode = start;
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
                return action;
            }
            set
            {
                action = value;
                RaisePropertyChanged(nameof(Action));
                RaisePropertyChanged(nameof(ActionString));
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
                operation.OperationType = value;
                RaisePropertyChanged(nameof(OperationType));
                RaisePropertyChanged(nameof(OperationTypeName));
            }
        }

        public string OperationTypeName
        {
            get
            {
                return "todo nazwa dla: " + operation.OperationType; // todo
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
    }
}
