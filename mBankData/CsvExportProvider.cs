using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

using WUKasa;

namespace mBankData
{
    public class CsvExportProvider
    {
        public event EventHandler<ImportedOperation> OperationImported;
        private ImportConfiguration cfg;

        public CsvExportProvider(ImportConfiguration cfg)
        {
            this.cfg = cfg;
        }
        public void Import(string filename, string trashold)
        {
            int lineNo = 0;
            using (var file = new StreamReader(filename))
            {
                while (!file.EndOfStream)
                {
                    lineNo++;
                    var mBOperation = ParseCsvLine(file.ReadLine());
                    if (mBOperation != null)
                    {
                        ImportedOperation importedOperation = TranslateMBankOperation(mBOperation, trashold);
                        if (importedOperation != null)
                            RaiseOperationImported(importedOperation, lineNo);
                    }
                }
            }
        }

        #region Import private methods
        private void RaiseOperationImported(ImportedOperation importedOperation, int lineNo)
        {
            if (this.OperationImported != null)
            {
                importedOperation.OperationOrigin = new CsvExportOrigin(lineNo);

                this.OperationImported(this, importedOperation);
            }
        }

        private ImportedOperation TranslateMBankOperation(mBankOperation mBOperation, string trashold)
        {
            ImportedOperation impOperation = new ImportedOperation();

            switch (mBOperation.OperationDescription)
            {
                case (mBankConsts.TransferOut):
                    {
                        return TranslateTransferOut(mBOperation, trashold);
                    }
                default: return TranslateGeneralExpense(mBOperation, trashold);
                    /*
                             public static string InternalTransfer = "PRZELEW WEWNĘTRZNY WYCHODZĄCY";
        public static string MTransfer = "PRZELEW MTRANSFER WYCHODZACY";
        public static string TransferOut = "PRZELEW ZEWNĘTRZNY WYCHODZĄCY";
        public static string TransferIn = "PRZELEW ZEWNĘTRZNY PRZYCHODZĄCY";
        public static string TaxTransfer = "PRZELEW PRZYSZŁY PODATKOWY";
        public static string Cardpay = "ZAKUP PRZY UŻYCIU KARTY";
        public static string Cashout = "WYPŁATA W BANKOMACIE";
        public static string CardFee = "OPŁATA ZA KARTĘ";   
                     */
            }
        }

        private ImportedOperation TranslateGeneralExpense(mBankOperation mBOperation, string trashold)
        {
            ImportedOperation opr = new ImportedOperation();
            opr.Date = mBOperation.OperationDate;
            opr.Description = mBOperation.Title;
            opr.Name1 = mBOperation.SenderReceiver;

            if (mBOperation.Ammount < 0)
            {
                opr.OperationType = Operation.OperationOutGeneral;
                opr.Amount = -1 * mBOperation.Ammount;
                opr.MoneyOut = opr.Amount;
            }
            else
            {
                opr.OperationType = Operation.OperationInGeneral;
                opr.Amount = mBOperation.Ammount;
                opr.MoneyIn = opr.Amount;
            }
            opr.Account = Operation.FormAccount(opr.OperationType, trashold);

            return opr;
        }

        private ImportedOperation TranslateTransferOut(mBankOperation mBOperation, string trashold)
        {
            return TranslateGeneralExpense(mBOperation, trashold);

            //ImportedOperation opr = new ImportedOperation();

            //return opr;
        }

        private mBankOperation ParseCsvLine(string line)
        {
            var mBankFormatProvider = new CultureInfo(mBankConsts.FormatCulture);
            line = line.Trim();
            if (String.IsNullOrEmpty(line))
                return null;

            string[] items = line.Split(new string[] { mBankConsts.CsvSeparator }, StringSplitOptions.None);

            if (items.Length != mBankConsts.ExpectedColumnNumber)
                return null;
            DateTime dt;
            if (!DateTime.TryParse(items[0], mBankFormatProvider, DateTimeStyles.AssumeLocal, out dt))
                return null;

            mBankOperation opr = new mBankOperation();
            opr.OperationDate = DateTime.Parse(items[0], mBankFormatProvider);
            opr.AccountingDate = DateTime.Parse(items[1], mBankFormatProvider);
            opr.OperationDescription = items[2];
            opr.Title = items[3];
            opr.SenderReceiver = items[4];
            opr.AccountNumber = items[5];
            opr.Ammount = Decimal.Parse(items[6], mBankFormatProvider);
            opr.Balance = Decimal.Parse(items[7], mBankFormatProvider);

            return opr;
            /*
# Data operacji;#Data księgowania;#Opis operacji;#Tytuł;#Nadawca/Odbiorca;#Numer konta;#Kwota;#Saldo po operacji;
            */
        }
        #endregion

        public void RemovedImported(List<Object> operationOrigins)
        {
            // todo
        }



        /*
# Data operacji;#Data księgowania;#Opis operacji;#Tytuł;#Nadawca/Odbiorca;#Numer konta;#Kwota;#Saldo po operacji;
*/

    }
}
