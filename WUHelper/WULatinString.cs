using System;

using System.Runtime.InteropServices;
using System.Text;

namespace WUHelper
{
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
            switch (c)
            {
                case 'Ą': return 0;
                case 'Ć': return 0;
                case 'Ę': return 0;
                case 'Ł': return 0;
                case 'Ń': return 0;
                case 'Ó': return 0;
                case 'Ś': return 0;
                case 'Ź': return 0;
                case 'Ż': return 0;
                case 'ą': return 0;
                case 'ć': return 0;
                case 'ę': return 0;
                case 'ł': return 0;
                case 'ń': return 0;
                case 'ó': return 0;
                case 'ś': return 0;
                case 'ź': return 0;
                case 'ż': return 0;
                default: return Encoding.ASCII.GetBytes(new char[] { c })[0];
            }

        }

        public static char Latin2Byte2Uth16Char(byte b)
        {
            switch (b)
            {
                case 1: return 'Ą';
                case 2: return 'Ć';
                case 3: return 'Ę';
                case 4: return 'Ł';
                case 5: return 'Ń';
                case 6: return 'Ó';
                case 7: return 'Ś';
                case 8: return 'Ź';
                case 9: return 'Ż';
                case 10: return 'ą';
                case 11: return 'ć';
                case 12: return 'ę';
                case 13: return 'ł';
                case 14: return 'ń';
                case 15: return 'ó';
                case 16: return 'ś';
                case 17: return 'ź';
                case 18: return 'ż';
                default: return Encoding.ASCII.GetChars(new byte[] { b })[0];
            }
        }

    }
}
