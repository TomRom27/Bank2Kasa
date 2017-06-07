using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank2Kasa.ViewModel
{
    public class OperationListSettings : GalaSoft.MvvmLight.ViewModelBase
    {
        public OperationListSettings()
        {
            Year = 2017;
        }

        private string _ImportFile;
        public string ImportFile
        {
            get
            {
                return _ImportFile;
            }
            set
            {
                _ImportFile = value;
                RaisePropertyChanged(nameof(ImportFile));
            }
        }

        private string _KasaFolder;
        public string KasaFolder
        {
            get
            {
                return _KasaFolder;
            }
            set
            {
                if ((_KasaFolder == null) || (!_KasaFolder.Equals(value)))
                {
                    _KasaFolder = value;
                    RaisePropertyChanged(nameof(KasaFolder));
                    RaisePropertyChanged(nameof(KasaFile));
                    Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Service.IOperationService>().SetKasaFolder(value);
                }
            }
        }

        private int _Year;
        public int Year
        {
            get
            {
                return _Year;
            }
            set
            {
                _Year = value;
                RaisePropertyChanged(nameof(Year));
                RaisePropertyChanged(nameof(KasaFile));
            }
        }

        public string KasaFile
        {
            get
            {
                return System.IO.Path.Combine(KasaFolder, String.Format(WUKasa.OperationStore.FileNameTemplate, Year));
            }
        }

        private string _Trashold;
        public string Trashold
        {
            get
            {
                return _Trashold;
            }
            set
            {
                _Trashold = value;
                RaisePropertyChanged(nameof(Trashold));
            }
        }

        private bool _AggregateDay;
        public bool AggregateDay
        {
            get
            {
                return _AggregateDay;
            }
            set
            {
                _AggregateDay = value;
                RaisePropertyChanged(nameof(AggregateDay));
            }
        }
    }
}
