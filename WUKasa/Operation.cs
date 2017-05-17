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

        //public string OpertionType
        //{
        //    get { return WULatinStringHelper.GetStringFromBytes(oprRecord.TypArray); }
        //    set { WULatinStringHelper.SetStringToBytes(value, ref oprRecord.TypArray); }
        //}

        //public decimal Amount
        //{
        //    get { return oprRecord.Kwota.Value; }
        //    set { oprRecord.Kwota.Value = value; }
        //}

        #endregion

        #region IBTreeRecord interface
        public byte[] GetBytes()
        {
            return StructHelper.StructToBytes<OprRecord>(oprRecord);
        }
        #endregion
    }
}
