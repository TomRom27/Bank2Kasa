using System;

using System.Runtime.InteropServices;
using System.Text;

namespace WUHelper
{
    [StructLayout(LayoutKind.Explicit)]
    public struct WULatinString
    {

        public WULatinString(byte maxLen)
        {
            wuInternal = new byte[maxLen+1];
            Value = "";
        }

        public string Value
        {
            get
            {
                // todo - coversion
                return Encoding.ASCII.GetString(wuInternal,1,wuInternal[0]);
            }
            set
            {
                wuInternal[0] = (byte)Math.Min( (byte)value.Length, (byte)wuInternal.Length-1);
                for (byte i = 0; i <= wuInternal[0]-1; i++)
                    wuInternal[i+1] = WULatinStringHelper.Utf16Char2Latin2Byte(value[i]);

            }
        }

        [FieldOffset(0)]
        private readonly byte[] wuInternal;
    }

    public static class WULatinStringHelper
    {
        public static string GetStringFromBytes(byte[] bytes)
        {
            // assumption - bytes is Pascal string like array i.e. zero-ed element is the actual length
            // todo - coversion
            return Encoding.ASCII.GetString(bytes, 1, bytes[0]);
        }

        public static void SetStringToBytes(string s, ref byte[] bytes)
        {
            bytes[0] = (byte)Math.Min((byte)s.Length, (byte)bytes.Length - 1);
            for (byte i = 0; i <= bytes[0] - 1; i++)
                bytes[i + 1] = WULatinStringHelper.Utf16Char2Latin2Byte(s[i]);
        }

        public static byte Utf16Char2Latin2Byte(char c)
        {
            // todo conversion from Unicode to Latin2
            return Encoding.ASCII.GetBytes(new char[] { c })[0];
        }

        public static string Latin2Byte2Uth16Char(byte b)
        {
            return Encoding.ASCII.GetString(new byte[] { b });
        }

    }
}
