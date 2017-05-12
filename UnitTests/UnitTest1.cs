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
            OprRecord record = new OprRecord();
            record.Kwota = new WUHelper.WUForsa(100);
            record.Stan = new WUHelper.WUForsa(200);

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, record.Kwota);

            using (var file = File.OpenWrite(filename))
            {
                var writer = new BinaryFormatter();
                writer.Serialize(file, record); // Writes the entire list.
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
         int sizestartXML = Marshal.SizeOf(startXML);//Get size of struct data
            byte[] startXML_buf = new byte[sizestartXML];//declare byte array and initialize its size
            IntPtr ptr = Marshal.AllocHGlobal(sizestartXML);//pointer to byte array
 
            Marshal.StructureToPtr(startXML, ptr, true);
            Marshal.Copy(ptr, startXML_buf, 0, sizestartXML);
            Marshal.FreeHGlobal(ptr);
         */

        /*
         struct StartReadXML
        {
            public int CmdID;//3
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
            public string CmdName;//ReadXML
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 50)]
            public string Description;//Data to be sent
        };


and
Hide   Copy Code

StartReadXML startXML=new StartReadXML();
            startXML.CmdID = 3;
            //var charCmdName = "s".ToCharArray();            
            startXML.CmdName = "Sree";
            startXML.Description = "test";
          
         */
    }
}
