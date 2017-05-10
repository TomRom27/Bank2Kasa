using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WUHelper;

namespace WUKasa
{
    [Serializable]
    public struct OprRecord
    {
        public OprRecord(object o)
        {
            Kwota = new WUForsa(0);
            Przyjeto = new WUForsa(0);
            Wydano = new WUForsa(0);
            Stan = new WUForsa(0);
        }

        public WUForsa Kwota;
        public WUForsa Przyjeto;
        public WUForsa Wydano;
        public WUForsa Stan;


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
    */
    }
}
