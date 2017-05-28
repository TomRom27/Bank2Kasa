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

        public OperationType()
        {
            oprTypRecord = new OprTypRecord(1);
        }

        public OperationType(byte[] record)
        {
            oprTypRecord = StructHelper.BytesToStruct<OprTypRecord>(ref record);
        }

        #region properties

        public bool isDeleted
        {
            get { return oprTypRecord.Deleted != 0; }
        }

        // todo
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
