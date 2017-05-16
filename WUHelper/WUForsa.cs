using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WUHelper
{
    [StructLayout(LayoutKind.Explicit)]
    public struct WUForsa
    {
        [NonSerialized]
        private const int Denominator = 1000;

        public WUForsa(decimal amount)
        {
            wuInternal = 0;
            Amount = amount;
        }


        public decimal Amount
        {
            get { return Convert.ToDecimal(wuInternal / Denominator);  }
            set
            {
                wuInternal = Convert.ToInt32(value * Denominator);
            }
        }

        [FieldOffset(0)]
        private int wuInternal;
    }
}
