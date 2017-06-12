﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BTreeFileUtil;
using WUHelper;

namespace WUKasa
{
    public class Operation : IBTreeRecord
    {
        #region
        public const string OperationOutGeneral = "00";
        public const string OperationOutHousehold = "04";
        public const string OperationOutToBank = "26";
        public const string OperationOutHouseholdPrio = "34";
        public const string OperationInGeneral = "10";
        public const string OperationInTransfer = "15";
        public const string OperationInCard = "13";
        public const string OperationInCashout = "12";
        private const string AccountTemplate = "500-0{0}-{1}";
        public const string AnnotatedPrefix = ">";
        #endregion

        private OprRecord oprRecord;
        private PolishConverter polishConverter;

        public Operation()
        {
            oprRecord = new OprRecord(1);
            polishConverter = new MazoviaConverter();
            Max = GetMaxFromTime();
        }

        public Operation(byte[] record)
        {
            oprRecord = StructHelper.BytesToStruct<OprRecord>(ref record);
            polishConverter = new MazoviaConverter();
            Max = GetMaxFromTime();
        }

        public static string FormAccount(string code, string trashold)
        {
            return String.Format(AccountTemplate, code, trashold);
        }
        #region properties
        public bool IsIncome { get; set; }

        public bool isDeleted
        {
            get { return oprRecord.Deleted != 0; }
        }

        public DateTime Date
        {
            get { return oprRecord.Data.Value; }
            set { oprRecord.Data.Value = value; }
        }

        public string OperationType
        {
            get { return polishConverter.GetStringFromBytes(oprRecord.TypArray); }
            set { polishConverter.SetStringToBytes(value, ref oprRecord.TypArray); }
        }

        public string OperationCode
        {
            get { return polishConverter.GetStringFromBytes(oprRecord.KodArray); }
            set { polishConverter.SetStringToBytes(value, ref oprRecord.KodArray); }
        }
        public string Name1
        {
            get { return polishConverter.GetStringFromBytes(oprRecord.Nazwa1Array); }
            set { polishConverter.SetStringToBytes(value, ref oprRecord.Nazwa1Array); }
        }

        public string Name2
        {
            get { return polishConverter.GetStringFromBytes(oprRecord.Nazwa2Array); }
            set { polishConverter.SetStringToBytes(value, ref oprRecord.Nazwa2Array); }
        }

        public string City
        {
            get { return polishConverter.GetStringFromBytes(oprRecord.MiastoArray); }
            set { polishConverter.SetStringToBytes(value, ref oprRecord.MiastoArray); }
        }

        public string Street
        {
            get { return polishConverter.GetStringFromBytes(oprRecord.UlicaArray); }
            set { polishConverter.SetStringToBytes(value, ref oprRecord.UlicaArray); }
        }

        public string FinanceCode
        {
            get { return polishConverter.GetStringFromBytes(oprRecord.RozrArray); }
            set { polishConverter.SetStringToBytes(value, ref oprRecord.RozrArray); }
        }

        public string Description
        {
            get { return polishConverter.GetStringFromBytes(oprRecord.OpisArray); }
            set { polishConverter.SetStringToBytes(value, ref oprRecord.OpisArray); }
        }

        public decimal Amount
        {
            get { return oprRecord.Kwota.Value; }
            set { oprRecord.Kwota.Value = value; }
        }

        public decimal MoneyIn
        {
            get { return oprRecord.Przyjeto.Value; }
            set { oprRecord.Przyjeto.Value = value; }
        }
        public decimal MoneyOut
        {
            get { return oprRecord.Wydano.Value; }
            set { oprRecord.Wydano.Value = value; }
        }
        public decimal Balance
        {
            get { return oprRecord.Stan.Value; }
            set { oprRecord.Stan.Value = value; }
        }

        public string Account
        {
            get { return polishConverter.GetStringFromBytes(oprRecord.KontoArray); }
            set { polishConverter.SetStringToBytes(value, ref oprRecord.KontoArray); }
        }

        public int PrintNumber
        {
            get { return oprRecord.NrDrk; }
            set { oprRecord.NrDrk = Convert.ToUInt16(value); }
        }
        public int Max
        {
            get { return oprRecord.Max; }
            set { oprRecord.Max = value; }
        }
        #endregion

        public static int GetMaxFromTime()
        {
            return ((DateTime.Now - new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day))).Milliseconds;
        }

        #region IBTreeRecord interface
        public byte[] GetBytes()
        {
            return StructHelper.StructToBytes<OprRecord>(oprRecord);
        }

        public void SetFromBytes(byte[] bytes)
        {
            byte[] data = bytes.ToArray();

            oprRecord = StructHelper.BytesToStruct<OprRecord>(ref data);
        }

        public int GetSize()
        {
            return System.Runtime.InteropServices.Marshal.SizeOf(typeof(OprRecord));
        }
        #endregion
    }
}
