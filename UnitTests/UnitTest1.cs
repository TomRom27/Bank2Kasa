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



            using (var file = File.OpenWrite(filename))
            {
                var writer = new BinaryFormatter();
                writer.Serialize(file, record); // Writes the entire list.
            }
        }
    }
}
