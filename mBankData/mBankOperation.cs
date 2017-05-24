using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mBankData
{
    internal class mBankOperation
    {
        public DateTime OperationDate { get; set; }
        public DateTime AccountingDate { get; set; }
        public string OperationDescription { get; set; }
        public string Title { get; set; }
        public string SenderReceiver { get; set; }
        public string AccountNumber { get; set; }
        public decimal Ammount { get; set; }
        public decimal Balance { get; set; }
    }
}
