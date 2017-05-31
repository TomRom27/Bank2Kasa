using System;

using System.Runtime.InteropServices;
using System.Text;

namespace WUHelper
{
    public interface PolishConverter
    {
        string GetStringFromBytes(byte[] bytes);


        void SetStringToBytes(string s, ref byte[] bytes);
    }

    public static class WULatinStringHelper
    {
        public static string GetStringFromBytes(byte[] bytes)
        {
            // assumption - bytes is Pascal string like array i.e. zero-ed element is the actual length
            string s = "";
            for (byte i = 1; i <= bytes[0]; i++)
                s = s + NationalByte2Utf16Char(bytes[i]);
            return s;
        }

        public static void SetStringToBytes(string s, ref byte[] bytes)
        {
            bytes[0] = (byte)Math.Min((byte)s.Length, (byte)bytes.Length - 1);
            for (byte i = 0; i <= bytes[0] - 1; i++)
                bytes[i + 1] = Utf16Char2NationalByte(s[i]);
        }

        public static byte Utf16Char2NationalByte(char c)
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

        /*
    Ą 	Ć 	Ę 	Ł 	Ń 	Ó 	Ś 	Ź 	Ż 	ą 	ć 	ę 	ł 	ń 	ó 	ś 	ź 	ż
    161 198 202 163 209 211 166 172 175 177 230 234 179 241 243 182 188 191
    #A1 #C6 #CA #A3 #D1 #D3 #A6 #AC #AF #B1 #E6 #EA #B3 #F1 #F3 #B6 #BC #BF
         */

        public static char NationalByte2Utf16Char(byte b)
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

    public class MazoviaConverter: PolishConverter
    {
        public string GetStringFromBytes(byte[] bytes)
        {
            // assumption - bytes is Pascal string like array i.e. zero-ed element is the actual length
            string s = "";
            for (byte i = 1; i <= bytes[0]; i++)
                s = s + NationalByte2Utf16Char(bytes[i]);
            return s;
        }

        public void SetStringToBytes(string s, ref byte[] bytes)
        {
            bytes[0] = (byte)Math.Min((byte)s.Length, (byte)bytes.Length - 1);
            for (byte i = 0; i <= bytes[0] - 1; i++)
                bytes[i + 1] = Utf16Char2NationalByte(s[i]);
        }

        #region conversion methods
        private byte Utf16Char2NationalByte(char c)
        {
            switch (c)
            {
                case 'Ą': return 143;
                case 'Ć': return 149;
                case 'Ę': return 144;
                case 'Ł': return 156;
                case 'Ń': return 165;
                case 'Ó': return 163;
                case 'Ś': return 152;
                case 'Ź': return 160;
                case 'Ż': return 161;
                case 'ą': return 134;
                case 'ć': return 141;
                case 'ę': return 145;
                case 'ł': return 146;
                case 'ń': return 164;
                case 'ó': return 162;
                case 'ś': return 158;
                case 'ź': return 166;
                case 'ż': return 167;
                default: return Encoding.ASCII.GetBytes(new char[] { c })[0];
            }

        }

        /*
    Ą 	 Ć 	 Ę 	 Ł 	Ń 	Ó 	Ś 	Ź 	Ż 	ą 	ć 	ę 	ł 	ń 	ó 	ś 	ź 	ż
    143 149 144 156 165 163 152 160 161 134 141 145 146 164 162 158 166 167
    #8F #95 #90 #9C #A5 #A3 #98 #A0 #A1 #86 #8D #91 #92 #A4 #A2 #9E #A6 #A7

         */

        private char NationalByte2Utf16Char(byte b)
        {
            switch (b)
            {
                case 143: return 'Ą';
                case 149: return 'Ć';
                case 144: return 'Ę';
                case 156: return 'Ł';
                case 165: return 'Ń';
                case 163: return 'Ó';
                case 152: return 'Ś';
                case 160: return 'Ź';
                case 161: return 'Ż';
                case 134: return 'ą';
                case 141: return 'ć';
                case 145: return 'ę';
                case 146: return 'ł';
                case 164: return 'ń';
                case 162: return 'ó';
                case 158: return 'ś';
                case 166: return 'ź';
                case 167: return 'ż';
                default: return Encoding.ASCII.GetChars(new byte[] { b })[0];
            }
        }
        #endregion
    }
}
