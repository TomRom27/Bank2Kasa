using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [StructLayout(LayoutKind.Sequential, Pack=1, Size = 17)]
    public struct TestStruct
    {
        //[FieldOffset(0)]
        public System.Int32 Del; // 4

        //[FieldOffset(4)]
        public System.UInt16 Dt; // 2

        //[FieldOffset(6)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6 + 1)]
        public byte[] StrStart; // actual length is 6+1 = 7

        //[FieldOffset(13)]
        public System.Int32 Max; //4
    }

    [TestClass]
    public class StructTests
    {

        [TestMethod]
        public void TestBytes()
        {
            byte[] bytes;

            TestStruct st = new TestStruct();

            st.Del = 1;
            st.Dt = 257;
            st.StrStart = new byte[] { 1, 2, 3, 4, 5, 8, 0 };
            st.Max = 2;

            bytes = BTreeFileUtil.StructHelper.StructToBytes(st);

        }


        private void AssignBytes(byte[] content, ref byte start)
        {

        }
    }

}
