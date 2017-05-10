using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUHelper
{
    [Serializable]
    public struct Forsa
    {
        private const int Denominator = 1000;

        public Forsa(decimal amount)
        {
            wuInternal = 0;
        }

        public decimal Amount
        {
            get { return Convert.ToDecimal(wuInternal / Denominator);  }
            set
            {
                wuInternal = Convert.ToInt32(value * Denominator);
            }
        }

        private int wuInternal;
    }
}
