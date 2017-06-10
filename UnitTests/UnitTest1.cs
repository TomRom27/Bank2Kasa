using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WUHelper;
using WUKasa;
using BTreeFileUtil;

namespace UnitTests
{
}


[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void WULatinStringTest()
    {
        string s = "żółwia";
        byte[] bytes = new byte[s.Length + 1];
        WULatinStringHelper.SetStringToBytes(s, ref bytes);
        string s2 = WULatinStringHelper.GetStringFromBytes(bytes);

        Assert.AreEqual(s, s2);
    }

    [TestMethod]
    public void WUForsaTest()
    {
        decimal testValue = 100.1m;
        WUForsa forsa = new WUForsa();

        forsa.Value = testValue;

        Assert.AreEqual(testValue, forsa.Value);

    }
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
    public void CanGetAllOperationTypes()
    {
        OperationTypeStore store = new OperationTypeStore("");

        var list = store.GetAll();

        Assert.AreEqual(38, list.Count);
    }

    [TestMethod]
    public void OperationTypeStruct()
    {
        string filename = "$Typ.Dat";
        byte[] recBytes = new byte[56];
        OprTypRecord rec = new OprTypRecord();

        using (var file = File.OpenRead(filename))
        {
            file.Seek(56, SeekOrigin.Begin);
            file.Read(recBytes, 0, 56);
        }

        rec = StructHelper.BytesToStruct<OprTypRecord>(ref recBytes);
        OperationType opt = new OperationType(recBytes);
    }

    [TestMethod]
    public void CanThreeTwoOperations()
    {
        int year = 2018;
        string path = @"C:\VirtualXP\Tomek\Dropbox\Kasa\TR";

        Operation op1 = new Operation() { Account = "500-010-4", Amount = 100, IsIncome = true, Date = new DateTime(year, 6, 4), OperationType = "10", Description = "Przychód 100 dla żółwia", MoneyIn = 100, Max = 1 };
        Operation op2 = new Operation() { Account = "500-010-4", Amount = 200, IsIncome = false, Date = new DateTime(year, 6, 5), OperationType = "04", Description = "Rozchód 200 dla kogo chcesz", MoneyOut = 200, Max = 2 };
        Operation op3 = new Operation() { Account = "500-010-4", Amount = 230, IsIncome = false, Date = new DateTime(year, 6, 6), OperationType = "04", Description = "Rozchód 230 dla kogo chcesz", MoneyOut = 230, Max = 2 };

        OperationStore store = new OperationStore(year, path);
        int count = store.Count;

        // act
        store.Add(op1);
        store.Add(op2);
        store.Add(op3);

        //assert
        long actualSize = new System.IO.FileInfo(System.IO.Path.Combine(path, $"OPR{year}.DAT")).Length;
        Assert.AreEqual((store.CountWithDeleted + 1) * 240, actualSize);
    }

    [TestMethod]
    public void CanReadAll()
    {
        int year = 2016;
        List<StoredOperation> storedList = new List<StoredOperation>();

        DateTime ds = DateTime.Now;

        OperationStore store = new OperationStore(year, "");
        store.ForEach((o, i) =>
        {
            if (!o.isDeleted)
                storedList.Add(new StoredOperation() { Operation = o, Position = i });
        });
        DateTime ds2 = DateTime.Now;
        System.Diagnostics.Trace.WriteLine("Read " + storedList.Count.ToString() + " in " + (ds2 - ds).TotalMilliseconds.ToString() + " ms");
        ds = DateTime.Now;
        storedList.Sort((o1, o2) => o1.Operation.Date.CompareTo(o2.Operation.Date));
        System.Diagnostics.Trace.WriteLine("Sorted in " + (DateTime.Now - ds).TotalMilliseconds.ToString() + " ms");

        Assert.AreEqual(store.Count, storedList.Count);
    }

    public class StoreCache
    {
        private List<StoredOperation> cachedList;

        public StoreCache()
        {
            cachedList = new List<StoredOperation>();
        }

        public void Add(Operation o, int position)
        {
            if (!o.isDeleted)
                cachedList.Add(new StoredOperation() { Operation = o, Position = position });
        }

        public int GetPosition(Operation o)
        {
            var found = cachedList.Find((co) => co.Operation.Date.Equals(o.Date) &&
                                               co.Operation.OperationType.Equals(o.OperationType) &&
                                               co.Operation.Amount.Equals(o.Amount));
            if (found != null)
                return found.Position;
            else
                return -1;
        }
    }
}
