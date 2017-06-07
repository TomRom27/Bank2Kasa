using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using WUKasa;
using WUKasa.Config;

namespace mBankData
{
    public class CsvExportProvider
    {
        public string MonthToken = "{Mc}";
        public string YearToken = "{Yr}";
        public string BankDescriptionToken = "{BankDescription}";
        public string BankTitleToken = "{BankTitle}";

        private ImportConfigurationSection importConfig;

        public CsvExportProvider()
        {
            importConfig = GetImportConfiguration();
        }

        public List<ImportedOperation> Import(string filename, string trashold, bool aggregateDay)
        {
            int currentMax = 1;
            List<ImportedOperation> list = new List<ImportedOperation>();

            System.Diagnostics.Trace.WriteLine("Number of configuration import rules: " + importConfig.ImportRules.Count.ToString());
            int lineNo = 0;
            using (var file = new StreamReader(filename, Encoding.GetEncoding(1250)))
            {
                while (!file.EndOfStream)
                {
                    lineNo++;
                    var mBOperation = ParseCsvLine(file.ReadLine());
                    if (mBOperation != null)
                    {
                        ImportedOperation importedOperation = TranslateMBankOperation(mBOperation, trashold);
                        if (importedOperation != null)
                        {
                            importedOperation.OperationOrigin = new CsvExportOrigin(lineNo);

                            // balancing operation in most cases must be added before the actual operation
                            var balancingOperation = CreateBalancingOperation(importedOperation);
                            if (balancingOperation != null)
                            {
                                if (balancingOperation.IsIncome)
                                {
                                    list.Add(balancingOperation);
                                    balancingOperation.Max = currentMax++;
                                    list.Add(importedOperation);
                                    importedOperation.Max = currentMax++;
                                }
                                else
                                {
                                    list.Add(importedOperation);
                                    importedOperation.Max = currentMax++;
                                    list.Add(balancingOperation);
                                    balancingOperation.Max = currentMax++;
                                }
                            }
                            else
                            {
                                list.Add(importedOperation);
                                importedOperation.Max = currentMax++;
                            }
                        }
                    }
                }
            }
            if (aggregateDay)
                AggregateDayIncomeTransferOperations(list);

            return list;

        }

        #region Import private methods

        private ImportedOperation TranslateMBankOperation(mBankOperation mBOperation, string trashold)
        {
            ImportedOperation impOperation = new ImportedOperation();

            foreach (var configRule in importConfig.ImportRules)
            {
                if (TranslateByConfigRule((ImportRule)configRule, mBOperation, trashold, impOperation))
                    return impOperation;
            }

            // process the rest, which is not covered by rules
            return TranslateGeneralExpense(mBOperation, trashold);
        }

        private bool TranslateByConfigRule(ImportRule configRule, mBankOperation mBOperation, string trashold, ImportedOperation opr)
        {
            bool ruleMatches = true;

            if (!String.IsNullOrEmpty(configRule.BankAccountRegEx))
                ruleMatches = ruleMatches && RegMatches(configRule.BankAccountRegEx, mBOperation.AccountNumber);
            if (!String.IsNullOrEmpty(configRule.BankDescriptionRegEx))
                ruleMatches = ruleMatches && RegMatches(configRule.BankDescriptionRegEx, mBOperation.OperationDescription);
            if (!String.IsNullOrEmpty(configRule.BankTitleRegEx))
                ruleMatches = ruleMatches && RegMatches(configRule.BankTitleRegEx, mBOperation.Title);
            if (!String.IsNullOrEmpty(configRule.SenderReceiverRegEx))
                ruleMatches = ruleMatches && RegMatches(configRule.SenderReceiverRegEx, mBOperation.SenderReceiver);
            if (configRule.BankAmount.HasValue)
                ruleMatches = ruleMatches && configRule.BankAmount.Value.Equals(mBOperation.Amount);

            if (!ruleMatches)
                return false;

            if (configRule.ExtractDateFromTitle.HasValue && configRule.ExtractDateFromTitle.Value)
                opr.Date = ExtractDateFromOperationTitle(mBOperation);
            else
                opr.Date = mBOperation.OperationDate;
            opr.Name1 = S2Cammel(mBOperation.SenderReceiver);
            if (opr.Name1.Length < mBOperation.SenderReceiver.Length)
                opr.Name2 = S2Cammel(mBOperation.SenderReceiver.Substring(opr.Name1.Length));

            opr.BankOperationType = S2Cammel(mBOperation.OperationDescription);
            opr.FullDescription = S2Cammel(mBOperation.Title);

            opr.OperationType = configRule.OperationTyp;
            opr.Description = ReplaceKnownToken(configRule.Description, mBOperation);
            opr.IsIncome = configRule.IsIncome;

            opr.Action = (ActionToDo)configRule.ActionCode;
            if (opr.IsIncome)
            {
                opr.Amount = Math.Abs(mBOperation.Amount);
                opr.MoneyIn = opr.Amount;
            }
            else
            {
                opr.Amount = Math.Abs(mBOperation.Amount);
                opr.MoneyOut = opr.Amount;
            }
            opr.Account = Operation.FormAccount(opr.OperationType, trashold);


            return true;
        }

        private ImportedOperation CreateBalancingOperation(ImportedOperation operation)
        {
            if (operation.Action == ActionToDo.Add2KasaAndRemoveFromImport)
            {
                ImportedOperation balancingOperation = new ImportedOperation();
                balancingOperation.Action = ActionToDo.Add2Kasa;
                balancingOperation.Date = operation.Date;
                balancingOperation.Amount = operation.Amount;
                balancingOperation.IsIncome = !operation.IsIncome;
                if (!operation.IsIncome)
                {
                    balancingOperation.OperationType = Operation.OperationInTransfer;
                    balancingOperation.Description = "Płatność przelewem";
                    balancingOperation.MoneyIn = operation.MoneyOut;
                }
                else
                {
                    balancingOperation.OperationType = Operation.OperationOutToBank;
                    balancingOperation.Description = "Wpłata do banku";
                    balancingOperation.MoneyOut = operation.MoneyIn;
                }

                return balancingOperation;
            }
            else
                return null;
        }

        private ImportedOperation TranslateGeneralExpense(mBankOperation mBOperation, string trashold)
        {
            ImportedOperation opr = new ImportedOperation();
            opr.Date = mBOperation.OperationDate;
            opr.Description = S2Cammel(mBOperation.Title);
            opr.Name1 = S2Cammel(mBOperation.SenderReceiver);
            if (opr.Name1.Length < mBOperation.SenderReceiver.Length)
                opr.Name2 = mBOperation.SenderReceiver.Substring(opr.Name1.Length);

            opr.BankOperationType = S2Cammel(mBOperation.OperationDescription);
            opr.FullDescription = S2Cammel(mBOperation.Title);

            if (mBOperation.Amount < 0)
            {
                opr.OperationType = Operation.OperationOutHousehold;
                opr.Amount = -1 * mBOperation.Amount;
                opr.MoneyOut = opr.Amount;
                opr.IsIncome = false;
            }
            else
            {
                opr.OperationType = Operation.OperationInGeneral;
                opr.Amount = mBOperation.Amount;
                opr.MoneyIn = opr.Amount;
                opr.IsIncome = true;
            }
            opr.Account = Operation.FormAccount(opr.OperationType, trashold);

            return opr;
        }


        private void AggregateDayIncomeTransferOperations(List<ImportedOperation> list)
        {
            ImportedOperation daySumm = new ImportedOperation();
            List<ImportedOperation> toDelete = new List<ImportedOperation>();

            // first sort in the wnated order i.e. by date
            list.Sort((o1, o2) => o1.Date.CompareTo(o2.Date) == 0 ? o1.Max.CompareTo(o2.Max) : o1.Date.CompareTo(o2.Date)); // by date and if equal - by Max            

            foreach (var o in list)
            {
                if (o.OperationType.Equals(WUKasa.Operation.OperationInTransfer))
                {
                    if (CanAggregate(daySumm, o))
                    {
                        daySumm.Amount = daySumm.Amount + o.Amount;
                        daySumm.MoneyIn = daySumm.MoneyIn + o.MoneyIn;
                        daySumm.MoneyOut = daySumm.MoneyOut + o.MoneyOut;
                        toDelete.Add(o);
                    }
                    else
                    {
                        daySumm = o;
                    }
                }
            }
            foreach (var d in toDelete)
                list.Remove(d);
        }

        private bool CanAggregate(ImportedOperation aggr, ImportedOperation opr)
        {
            return aggr.Date.Equals(opr.Date) && aggr.Description.Equals(opr.Description);
        }

        private mBankOperation ParseCsvLine(string line)
        {
            char[] obsoleteDelimiters = new char[] { '"', '\'' };

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
            opr.Title = items[3].TrimStart(obsoleteDelimiters).TrimEnd(obsoleteDelimiters);
            opr.SenderReceiver = items[4].TrimStart(obsoleteDelimiters).TrimEnd(obsoleteDelimiters);
            opr.AccountNumber = items[5].TrimStart(obsoleteDelimiters).TrimEnd(obsoleteDelimiters);
            opr.Amount = Decimal.Parse(items[6], mBankFormatProvider);
            opr.Balance = Decimal.Parse(items[7], mBankFormatProvider);

            return opr;
            /*
# Data operacji;#Data księgowania;#Opis operacji;#Tytuł;#Nadawca/Odbiorca;#Numer konta;#Kwota;#Saldo po operacji;
            */
        }

        private string ReplaceKnownToken(string input, mBankOperation mBankOpr)
        {
            input = input.Replace(MonthToken, mBankOpr.OperationDate.Month.ToString());
            input = input.Replace(YearToken, mBankOpr.OperationDate.Year.ToString());
            input = input.Replace(BankDescriptionToken, mBankOpr.OperationDescription);
            input = input.Replace(BankTitleToken, mBankOpr.Title);

            return input;
        }

        private DateTime ExtractDateFromOperationTitle(mBankOperation mBOperation)
        {
            const string x = "DATA TRANSAKCJI: ";
            int pos = mBOperation.Title.LastIndexOf(x);
            if (pos >= 0)
            {
                try
                {
                    return DateTime.Parse(mBOperation.Title.Substring(pos + x.Length), new CultureInfo(mBankConsts.FormatCulture));
                }
                catch
                {

                }
            }
            return mBOperation.AccountingDate;
        }


        private bool RegMatches(string pattern, string input)
        {
            return Regex.Match(input, pattern).Success;
        }
        #endregion

        public void RemovedImported(List<Object> operationOrigins)
        {
            // todo
        }

        private string S2Cammel(string s)
        {
            if (String.IsNullOrEmpty(s))
                return s;

            return s.Substring(0, 1).ToUpper() + s.Substring(1).ToLower();
        }

        private ImportConfigurationSection GetImportConfiguration()
        {
            try
            {

                return ConfigurationManager.GetSection("ImportConfigurationSection") as ImportConfigurationSection;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Failed to read ImportConfigurationSection: " + ex.Message);
                return new ImportConfigurationSection();
            }
        }

    }
}
