using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WUKasa;

namespace Bank2Kasa.ViewModel
{
    public class OperationVM : GalaSoft.MvvmLight.ViewModelBase
    {
        private Operation operation;

        public OperationVM()
        {
            this.operation = new Operation();
        }

        [GalaSoft.MvvmLight.Ioc.PreferredConstructor]
        public OperationVM(Operation operation)
        {
            this.operation = operation;
            IsIgnore = false;
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
    }
}
