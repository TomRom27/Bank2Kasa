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
            string s = "";
            for (byte i = 1; i <= bytes[0]; i++)
                s = s+ WULatinStringHelper.Latin2Byte2Uth16Char(bytes[i]);
            return s;
        }

        public static void SetStringToBytes(string s, ref byte[] bytes)
        {
            bytes[0] = (byte)Math.Min((byte)s.Length, (byte)bytes.Length - 1);
            for (byte i = 0; i <= bytes[0] - 1; i++)
                bytes[i + 1] = WULatinStringHelper.Utf16Char2Latin2Byte(s[i]);
        }

        public static byte Utf16Char2Latin2Byte(char c)
        {
            switch (c)
            {
                case 'Ą': return 161;
                case 'Ć': return 198;
                case 'Ę': return 202;
                case 'Ł': return 163;
                case 'Ń': return 209;
                case 'Ó': return 211;
                case 'Ś': return 166;
                case 'Ź': return 172;
                case 'Ż': return 175;
                case 'ą': return 177;
                case 'ć': return 230;
                case 'ę': return 234;
                case 'ł': return 179;
                case 'ń': return 241;
                case 'ó': return 243;
                case 'ś': return 182;
                case 'ź': return 188;
                case 'ż': return 191;
                default: return Encoding.ASCII.GetBytes(new char[] { c })[0];
            }

        }

        public static char Latin2Byte2Uth16Char(byte b)
        {
            switch (b)
            {
                case 161: return 'Ą';
                case 198: return 'Ć';
                case 202: return 'Ę';
                case 163: return 'Ł';
                case 209: return 'Ń';
                case 211: return 'Ó';
                case 166: return 'Ś';
                case 172: return 'Ź';
                case 175: return 'Ż';
                case 177: return 'ą';
                case 230: return 'ć';
                case 234: return 'ę';
                case 179: return 'ł';
                case 241: return 'ń';
                case 243: return 'ó';
                case 182: return 'ś';
                case 188: return 'ź';
                case 191: return 'ż';
                default: return Encoding.ASCII.GetChars(new byte[] { b })[0];
            }
        }

    }
}
