using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using WUHelper;

namespace WUKasa
{
    [Serializable]
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

        public OprRecord(object o)
        {
            //Kwota = new WUForsa(0);
            //Przyjeto = new WUForsa(0);
            //Wydano = new WUForsa(0);
            //Stan = new WUForsa(0);
            //Data = new WUShortDate(WUShortDateHelper.MinDate);
            //Typ = new WULatinString(TypKodSLen);
            TypArray = new byte[TypKodSLen + 1];
        }

        //public WUShortDate Data;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = TypKodSLen + 1)]
        public byte[] TypArray;
        //public WULatinString Typ;

        //public WUForsa Kwota;
        //public WUForsa Przyjeto;
        //public WUForsa Wydano;
        //public WUForsa Stan;


        /*
                 data  : date;
                       typ   :string[TypKodSLen];
                       kod   :string[OprKodSLen];
                       nazwa1,
                       nazwa2:string[OprNazwaSLen];
                       miasto:string[OprMiastoSLen];
                       ulica :string[OprUlicaSLen];
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
