﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WUHelper;
using WUKasa;
using BTreeFileUtil;

namespace UnitTests
{




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
            rec.Data.Value = new DateTime(2018, 1, 1);
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
            Operation[] oprs = new Operation[4 + 1];

            using (var file = File.OpenRead(filename))
            {

                file.Seek(240, SeekOrigin.Begin);
                file.Read(recBytes, 0, 240);
                oprs[1] = new Operation(recBytes);
                file.Read(recBytes, 0, 240);
                oprs[2] = new Operation(recBytes);
                file.Read(recBytes, 0, 240);
                oprs[3] = new Operation(recBytes);
                file.Read(recBytes, 0, 240);
                oprs[4] = new Operation(recBytes);
            }
            OprRecord oprRec = StructHelper.BytesToStruct<OprRecord>(ref recBytes);

            Operation opr = new Operation(recBytes);
        }


        [TestMethod]
        public void CanAdd1Operation()
        {
            int year = 2018;

            OperationStore store = new OperationStore(year, "");
            Operation operation = new Operation();

            store.Add(operation);
        }
    }
}
