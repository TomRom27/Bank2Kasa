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
            BackupDatFile = true;
            BackupImportFile = false;
            RemoveIxFile = true;
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

        private string _KasaFolder1;
        public string KasaFolder1
        {
            get
            {
                return _KasaFolder1;
            }
            set
            {
                if ((_KasaFolder1 == null) || (!_KasaFolder1.Equals(value)))
                {
                    _KasaFolder1 = value;
                    RaisePropertyChanged(nameof(KasaFolder1));
                    RaisePropertyChanged(nameof(KasaFile1));
                    Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Service.IOperationService>().SetKasaFolder(value);
                }
            }
        }


        public string KasaFile1
        {
            get
            {
                return System.IO.Path.Combine(KasaFolder1, String.Format(WUKasa.OperationStore.FileNameTemplate, Year));
            }
        }

        private string _Trashold1;
        public string Trashold1
        {
            get
            {
                return _Trashold1;
            }
            set
            {
                _Trashold1 = value;
                RaisePropertyChanged(nameof(Trashold1));
            }
        }

        private string _KasaFolder2;
        public string KasaFolder2
        {
            get
            {
                return _KasaFolder2;
            }
            set
            {
                if ((_KasaFolder2 == null) || (!_KasaFolder2.Equals(value)))
                {
                    _KasaFolder2 = value;
                    RaisePropertyChanged(nameof(KasaFolder2));
                    RaisePropertyChanged(nameof(KasaFile2));
                    Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Service.IOperationService>().SetKasaFolder(value);
                }
            }
        }


        public string KasaFile2
        {
            get
            {
                return System.IO.Path.Combine(KasaFolder2, String.Format(WUKasa.OperationStore.FileNameTemplate, Year));
            }
        }

        private string _Trashold2;
        public string Trashold2
        {
            get
            {
                return _Trashold2;
            }
            set
            {
                _Trashold2 = value;
                RaisePropertyChanged(nameof(Trashold2));
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
                RaisePropertyChanged(nameof(KasaFile1));
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

        private bool _BackupImportFile;
        public bool BackupImportFile
        {
            get
            {
                return _BackupImportFile;
            }
            set
            {
                _BackupImportFile = value;
                RaisePropertyChanged(nameof(BackupImportFile));
            }
        }


        private bool _BackupDatFile;
        public bool BackupDatFile
        {
            get
            {
                return _BackupDatFile;
            }
            set
            {
                _BackupDatFile = value;
                RaisePropertyChanged(nameof(BackupDatFile));
            }
        }


        private bool _RemoveIxFile;
        public bool RemoveIxFile
        {
            get
            {
                return _RemoveIxFile;
            }
            set
            {
                _RemoveIxFile = value;
                RaisePropertyChanged(nameof(RemoveIxFile));
            }
        }
    }
}
