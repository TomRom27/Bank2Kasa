using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WUHelper
{

    public static class WUShortDateHelper
    {
        public static DateTime MinDate = new DateTime(1600, 1, 1);
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct WUShortDate
    {
        public WUShortDate(DateTime dt)
        {
            wuInternal = 0;
            Value = dt;
        }
        public DateTime Value
        {
            get { return WUShortDateHelper.MinDate.AddDays(wuInternal); }
            set
            {
                wuInternal = (value - WUShortDateHelper.MinDate).Days;
            }
        }

        [FieldOffset(0)]
        private int wuInternal;
    }
}
