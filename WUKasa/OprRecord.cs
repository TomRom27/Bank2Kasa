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
    [StructLayout(LayoutKind.Sequential, Pack =1, Size = 240)]
    public struct OprRecord
    {
        public const byte TypKodSLen = 2;
        public const int OprKodSLen = 5;
        public const int OprNazwaSLen = 30;
        public const int OprMiastoSLen = 35;
        public const int OprUlicaSLen = 25;
        public const int OprRozrSLen = 10;
        public const int OprOpisSLen = 35;
        public const int OprRSLen = 9;
        public const int KontoLen = 15;

        public OprRecord(object o)
        { 
            unchecked
            {
                Deleted = 0;

                Data = new WUShortDate(WUShortDateHelper.MinDate);
                TypArray = new byte[TypKodSLen + 1];
                KodArray = new byte[OprKodSLen + 1];
                Nazwa1Array = new byte[OprNazwaSLen + 1];
                Nazwa2Array = new byte[OprNazwaSLen + 1];
                MiastoArray = new byte[OprMiastoSLen + 1];
                UlicaArray = new byte[OprUlicaSLen + 1];
                RozrArray = new byte[OprRozrSLen + 1];
                OpisArray = new byte[OprOpisSLen + 1];
                Kwota = new WUForsa(0);
                Przyjeto = new WUForsa(0);
                Wydano = new WUForsa(0);
                Stan = new WUForsa(0);
                KontoArray = new byte[KontoLen + 1];
                NrDrk = 0;
                Max = 0;
            }
        }

        public TpLong Deleted;
        public WUShortDate Data;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TypKodSLen + 1)]
        public byte[] TypArray;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = OprKodSLen + 1)]
        public byte[] KodArray;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = OprNazwaSLen + 1)]
        public byte[] Nazwa1Array;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = OprNazwaSLen + 1)]
        public byte[] Nazwa2Array;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = OprMiastoSLen + 1)]
        public byte[] MiastoArray;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = OprUlicaSLen + 1)]
        public byte[] UlicaArray;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = OprRozrSLen + 1)]
        public byte[] RozrArray;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = OprOpisSLen + 1)]
        public byte[] OpisArray;
        public WUForsa Kwota;
        public WUForsa Przyjeto;
        public WUForsa Wydano;
        public WUForsa Stan;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = KontoLen + 1)]
        public byte[] KontoArray;
        public TpWord NrDrk;
        public TpLong Max;


        /*
                 data  : date; + 
                       typ   :string[TypKodSLen]; +
                       kod   :string[OprKodSLen]; +
                       nazwa1, +
                       nazwa2:string[OprNazwaSLen]; +
                       miasto:string[OprMiastoSLen]; +
                       ulica :string[OprUlicaSLen]; +
                       rozr  :string[OprRozrSLen];
                       opis  :string[OprOpisSLen];
                       kwota ,
                       przyj ,
                       wydan ,
                       stan  :forsa;
                       konto :string[KontoLen];
                       nrdrk :word; 
    max   :longint; { uzywane do ustalania porzadku }

        240 = 236+4???
    */
    }
}
