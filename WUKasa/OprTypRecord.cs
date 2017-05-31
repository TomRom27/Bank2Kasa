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
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 56)]
    public struct OprTypRecord
    {
        public const byte TypKodSLen = 2;
        public const byte OpisSLen = 30;
        public const byte KontoSLen = 15;
        public const byte NaPlusLen= 2;

        public OprTypRecord(object o)
        {
            Deleted = 0;
            TypArray = new byte[TypKodSLen + 1];
            OpisArray = new byte[OpisSLen + 1];
            KontoArray = new byte[KontoSLen + 1];
            NaPlusArray = new byte[NaPlusLen];
        }

        public TpLong Deleted;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TypKodSLen + 1)]
        public byte[] TypArray;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = OpisSLen + 1)]
        public byte[] OpisArray;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NaPlusLen)]
        public byte[] NaPlusArray;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = KontoSLen + 1)]
        public byte[] KontoArray;
    }
}
