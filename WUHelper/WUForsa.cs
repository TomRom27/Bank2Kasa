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
        private const int Denominator = 100;

        public WUForsa(decimal amount)
        {
            wuInternal = 0;
            Value = amount;
        }

        public decimal Value
        {
            get { return Convert.ToDecimal(wuInternal / (1.0*Denominator));  }
            set
            {
                wuInternal = Convert.ToInt64(value * Denominator);
            }
        }

        [FieldOffset(0)]
        private Int64 wuInternal;
    }
}
