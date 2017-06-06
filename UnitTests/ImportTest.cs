using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using WUKasa;
using mBankData;
namespace UnitTests
{
    [TestClass]
    public class ImportTest
    {
        [TestMethod]
        public void CanImportMBankCsv1()
        {
            // arrange

            List<ImportedOperation> list;
            var extractor = new CsvExportProvider();

            // act
            list = extractor.Import("mbank.csv","1", true);
            // asset
            Assert.AreEqual(7, list.Count);
        }

        [TestMethod]
        public void CanImportMBankCsv2()
        {
            // arrange

            List<ImportedOperation> list;
            var extractor = new CsvExportProvider();

            // act
            list = extractor.Import("mbank2.csv", "1", true);
            // asset
            foreach (var o in list)
            {
                System.Diagnostics.Trace.WriteLine(o.Date.ToString("dd.MM.yyyy") + " " + o.OperationType + " " +
                                                o.Description.PadRight(35) + " " + ((o.IsIncome) ? "+" : "-") + " " +
                                                o.Amount.ToString().PadLeft(10) + " " + o.MoneyIn.ToString().PadLeft(10) + " " + o.MoneyOut.ToString().PadLeft(10) + " " +
                                                o.Action.ToString().PadRight(27)+" "+o.Max.ToString().PadLeft(5));
            }

            Assert.AreEqual(18, list.Count);
        }

        [TestMethod]
        public void CanImportMBankCsv3()
        {
            // arrange

            List<ImportedOperation> list;
            var extractor = new CsvExportProvider();

            // act
            list = extractor.Import("multi.csv", "1", true);
            // asset
            foreach (var o in list)
            {
                System.Diagnostics.Trace.WriteLine(o.Date.ToString("dd.MM.yyyy") + " " + o.OperationType + " " +
                                                o.Description.PadRight(35) + " " + ((o.IsIncome) ? "+" : "-") + " " +
                                                o.Amount.ToString().PadLeft(10) + " " + o.MoneyIn.ToString().PadLeft(10) + " " + o.MoneyOut.ToString().PadLeft(10) + " " +
                                                o.Action.ToString().PadRight(27) + " " + o.Max.ToString().PadLeft(5));
            }

            Assert.AreEqual(16, list.Count);
        }

        [TestMethod]
        public void CanImportMBankCsv4()
        {
            // arrange

            List<ImportedOperation> list;
            var extractor = new CsvExportProvider();

            // act
            list = extractor.Import("multi2.csv", "1", true);
            // asset
            foreach (var o in list)
            {
                System.Diagnostics.Trace.WriteLine(o.Date.ToString("dd.MM.yyyy") + " " + o.OperationType + " " +
                                                o.Description.PadRight(35) + " " + ((o.IsIncome) ? "+" : "-") + " " +
                                                o.Amount.ToString().PadLeft(10) + " " + o.MoneyIn.ToString().PadLeft(10) + " " + o.MoneyOut.ToString().PadLeft(10) + " " +
                                                o.Action.ToString().PadRight(27) + " " + o.Max.ToString().PadLeft(5));
            }

            Assert.AreEqual(39, list.Count);
        }
    }
}
