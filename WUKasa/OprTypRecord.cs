using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;


#region type aliases
using TpLong = System.Int32;
using TpWord = System.UInt16; // todo
#endregion

using WUHelper;

namespace WUKasa
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 240)] //todo
    public struct OprTypRecord
    {
        public const byte TypKodSLen = 2;
        public const int OprKodSLen = 5;

        public OprTypRecord(object o)
        {
            Deleted = 0;
            TypArray = new byte[TypKodSLen + 1];
            KodArray = new byte[OprKodSLen + 1];
        }

        public TpLong Deleted;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TypKodSLen + 1)]
        public byte[] TypArray;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = OprKodSLen + 1)]
        public byte[] KodArray;
        //todo
    }
}
