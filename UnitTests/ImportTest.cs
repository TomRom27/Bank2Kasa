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
        public void CanImportMBankCsv()
        {
            // arrange

            List<ImportedOperation> list = new List<ImportedOperation>();
            var extractor = new CsvExportProvider();

            extractor.OperationImported += delegate (object sender, ImportedOperation args)
            {
                list.Add(args);
            };

            // act
            extractor.Import("mbank.csv","1");
            // asset
            Assert.AreEqual(7, list.Count);
        }
    }
}
