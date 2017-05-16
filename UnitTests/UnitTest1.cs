using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WUKasa;
namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {

            string filename = "test.data";
            OprRecord record = new OprRecord(1);
            //record.Kwota.Amount = 100;
            //record.Przyjeto.Amount = 100;
            //record.Wydano.Amount = 200;
            //record.Stan.Amount = 200;
            //record.Typ.Value = "02";
            record.TypArray[0] = 1;
            record.TypArray[1] = 48;
            //record.Data.Value = new DateTime(2018, 1, 1);


            byte[] bytes;

            bytes = Struct2Bytes(record);

            using (var file = File.OpenWrite(filename))
            {
                file.Write(bytes, 0, bytes.Length);
            }
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
