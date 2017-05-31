using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BTreeFileUtil;
using WUHelper;

namespace WUKasa
{
    public class OperationType : IBTreeRecord
    {
        private OprTypRecord oprTypRecord;
        private PolishConverter polishConverter;

        public OperationType()
        {
            oprTypRecord = new OprTypRecord(1);
            polishConverter = new MazoviaConverter();
        }

        public OperationType(byte[] record)
        {
            oprTypRecord = StructHelper.BytesToStruct<OprTypRecord>(ref record);
            polishConverter = new MazoviaConverter();
        }

        #region properties

        public bool isDeleted
        {
            get { return oprTypRecord.Deleted != 0; }
        }

        public string OperationTypeCode
        {
            get { return polishConverter.GetStringFromBytes(oprTypRecord.TypArray); }
            set { polishConverter.SetStringToBytes(value, ref oprTypRecord.TypArray); }
        }

        public string Description
        {
            get { return polishConverter.GetStringFromBytes(oprTypRecord.OpisArray); }
            set { polishConverter.SetStringToBytes(value, ref oprTypRecord.OpisArray); }
        }

        public string Account
        {
            get { return polishConverter.GetStringFromBytes(oprTypRecord.KontoArray); }
            set { polishConverter.SetStringToBytes(value, ref oprTypRecord.KontoArray); }
        }

        public bool IsIncome
        {
            get { return Convert.ToBoolean(oprTypRecord.NaPlusArray[0]); }
            set { oprTypRecord.NaPlusArray[0] = Convert.ToByte(value); }
        }

        #endregion
        #region IBTreeRecord interface
        public byte[] GetBytes()
        {
            return StructHelper.StructToBytes<OprTypRecord>(oprTypRecord);
        }

        public void SetFromBytes(byte[] bytes)
        {
            byte[] data = bytes.ToArray();

            oprTypRecord = StructHelper.BytesToStruct<OprTypRecord>(ref data);
        }

        public int GetSize()
        {
            return System.Runtime.InteropServices.Marshal.SizeOf(typeof(OprTypRecord));
        }
        #endregion
    }
}
