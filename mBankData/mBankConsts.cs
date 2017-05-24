using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mBankData
{
    public static class mBankConsts
    {
        public const string DateFormat = "yyyy-MM-dd";
        public const string CsvSeparator = ";";
        public const int ExpectedColumnNumber = 9; // the 9th is empty
        public const string FormatCulture = "pl-PL";

        public const string InternalTransfer = "PRZELEW WEWNĘTRZNY WYCHODZĄCY";
        public const string MTransfer = "PRZELEW MTRANSFER WYCHODZACY";
        public const string TransferOut = "PRZELEW ZEWNĘTRZNY WYCHODZĄCY";
        public const string TransferIn = "PRZELEW ZEWNĘTRZNY PRZYCHODZĄCY";
        public const string TaxTransfer = "PRZELEW PRZYSZŁY PODATKOWY";
        public static string Cardpay = "ZAKUP PRZY UŻYCIU KARTY";
        public const string Cashout = "WYPŁATA W BANKOMACIE";
        public const string CardFee = "OPŁATA ZA KARTĘ";
    }

    public class CsvExportOrigin
    {
        public CsvExportOrigin(int lineNumber)
        {
            LineNumber = lineNumber;
        }

        public int LineNumber { get; private set; }
    }
}
