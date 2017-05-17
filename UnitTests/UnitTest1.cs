using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WUHelper;
using WUKasa;
using BTreeFileUtil;

namespace UnitTests
{


    //[StructLayout(LayoutKind.Explicit, Size = 7)]
    //public struct TestStruct
    //{
    //    [FieldOffset(0)]
    //    public System.Int32 del;

    //    [FieldOffset(4)]
    //    public System.UInt16 dt;
    //}

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            byte[] bytes;

            //TestStruct str = new TestStruct();
            //str.del = 0;
            //str.dt = 257;

            //bytes = StructHelper.StructToBytes(str);

            //WUShortDate sd = new WUShortDate(new DateTime(2018, 1, 1));
            //bytes = StructHelper.StructToBytes(sd);

            OprRecord rec = new OprRecord();
            rec.Data.Value = new DateTime(2018,1,1);
            rec.Kwota.Value = 100000000000000;

            bytes = StructHelper.StructToBytes(rec);

            string filename = "test.data";

            Operation opr = new Operation();
            //opr.OpertionType = "12";
            //opr.Date = new DateTime(2018, 1, 1);

            //opr.Amount = 100000000000000;


            //bytes = opr.GetBytes();// Struct2Bytes(record);

            using (var file = File.OpenWrite(filename))
            {
                file.Write(bytes, 0, bytes.Length);
            }
        }


        [TestMethod]
        public void ReadHeader()
        {
            string filename = "opr2018.dat";

            byte[] headerBytes = new byte[21];

            using (var file = File.OpenRead(filename))
            {
                file.Read(headerBytes, 0, 21);
            }
            BTreeFileHeader header = StructHelper.BytesToStruct<BTreeFileHeader>(ref headerBytes);
        }

        [TestMethod]
        public void Read1Rec()
        {
            string filename = "opr2018.dat";

            byte[] recBytes = new byte[240];

            using (var file = File.OpenRead(filename))
            {
                file.Seek(240, SeekOrigin.Begin);
                file.Read(recBytes, 0, 240);
            }
            OprRecord oprRec = StructHelper.BytesToStruct<OprRecord>(ref recBytes);

            Operation opr = new Operation(recBytes);
        }
        /*

                private void ByteToStruct(byte[] Buffer, ref TMenuDataStruct Struct)
                {
                    IntPtr pCurrentPosition;
                    GCHandle Handle = GCHandle.Alloc(Buffer, GCHandleType.Pinned);
                    pCurrentPosition = Handle.AddrOfPinnedObject();
                    Struct = Convert.ChangeType(Marshal.PtrToStructure(pCurrent Position,
                    TMenuDataStruct), TMenuDataStruct);
                    Handle.Free();
                }
                */
        /*
         */

        public byte[] Struct2Bytes(OprRecord startXML)
        {
            int sizestartXML = System.Runtime.InteropServices.Marshal.SizeOf(startXML);//Get size of struct data
            byte[] startXML_buf = new byte[sizestartXML];//declare byte array and initialize its size
            IntPtr ptr = System.Runtime.InteropServices.Marshal.AllocHGlobal(sizestartXML);//pointer to byte array

            System.Runtime.InteropServices.Marshal.StructureToPtr(startXML, ptr, true);
            System.Runtime.InteropServices.Marshal.Copy(ptr, startXML_buf, 0, sizestartXML);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(ptr);

            return startXML_buf;
        }

    }
}
