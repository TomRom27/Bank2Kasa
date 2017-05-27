using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank2Kasa.ViewModel
{
    public class OperationListSettings : GalaSoft.MvvmLight.ViewModelBase
    {
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
                _KasaFolder = value;
                RaisePropertyChanged(nameof(KasaFolder));
                RaisePropertyChanged(nameof(KasaFile));
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
                return System.IO.Path.Combine(KasaFolder, String.Format(WUKasa.OperationStore.FileNameTemplate,Year));
            }
        }
    }
}
