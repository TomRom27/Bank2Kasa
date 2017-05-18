using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BTreeFileUtil;
using WUHelper;

namespace WUKasa
{
    public class Operation : IBTreeRecord
    {
        private OprRecord oprRecord;

        public Operation()
        {
            oprRecord = new OprRecord(1);
        }

        public Operation(byte[] record)
        {
            oprRecord = StructHelper.BytesToStruct<OprRecord>(ref record);
        }

        #region properties
        public DateTime Date
        {
            get { return oprRecord.Data.Value; }
            set { oprRecord.Data.Value = value; }
        }

        public string OperationType
        {
            get { return WULatinStringHelper.GetStringFromBytes(oprRecord.TypArray); }
            set { WULatinStringHelper.SetStringToBytes(value, ref oprRecord.TypArray); }
        }

        public string OperationCode
        {
            get { return WULatinStringHelper.GetStringFromBytes(oprRecord.KodArray); }
            set { WULatinStringHelper.SetStringToBytes(value, ref oprRecord.KodArray); }
        }
        public string Name1
        {
            get { return WULatinStringHelper.GetStringFromBytes(oprRecord.Nazwa1Array); }
            set { WULatinStringHelper.SetStringToBytes(value, ref oprRecord.Nazwa1Array); }
        }

        public string Name2
        {
            get { return WULatinStringHelper.GetStringFromBytes(oprRecord.Nazwa2Array); }
            set { WULatinStringHelper.SetStringToBytes(value, ref oprRecord.Nazwa2Array); }
        }

        public string City
        {
            get { return WULatinStringHelper.GetStringFromBytes(oprRecord.MiastoArray); }
            set { WULatinStringHelper.SetStringToBytes(value, ref oprRecord.MiastoArray); }
        }

        public string Street
        {
            get { return WULatinStringHelper.GetStringFromBytes(oprRecord.UlicaArray); }
            set { WULatinStringHelper.SetStringToBytes(value, ref oprRecord.UlicaArray); }
        }

        public string FinanceCode
        {
            get { return WULatinStringHelper.GetStringFromBytes(oprRecord.RozrArray); }
            set { WULatinStringHelper.SetStringToBytes(value, ref oprRecord.RozrArray); }
        }

        public string Description
        {
            get { return WULatinStringHelper.GetStringFromBytes(oprRecord.OpisArray); }
            set { WULatinStringHelper.SetStringToBytes(value, ref oprRecord.OpisArray); }
        }

        public decimal Amount
        {
            get { return oprRecord.Kwota.Value; }
            set { oprRecord.Kwota.Value = value; }
        }

        public decimal MoneyIn
        {
            get { return oprRecord.Przyjeto.Value; }
            set { oprRecord.Przyjeto.Value = value; }
        }
        public decimal MoneyOut
        {
            get { return oprRecord.Wydano.Value; }
            set { oprRecord.Wydano.Value = value; }
        }
        public decimal Balance
        {
            get { return oprRecord.Stan.Value; }
            set { oprRecord.Stan.Value = value; }
        }

        public string Account
        {
            get { return WULatinStringHelper.GetStringFromBytes(oprRecord.KontoArray); }
            set { WULatinStringHelper.SetStringToBytes(value, ref oprRecord.KontoArray); }
        }

        public int PrintNumber
        {
            get { return oprRecord.NrDrk; }
            set { oprRecord.NrDrk = Convert.ToUInt16(value); }
        }
        public int Max
        {
            get { return oprRecord.Max; }
            set { oprRecord.Max = value; }
        }
        #endregion

        #region IBTreeRecord interface
        public byte[] GetBytes()
        {
            return StructHelper.StructToBytes<OprRecord>(oprRecord);
        }
        #endregion
    }
}
