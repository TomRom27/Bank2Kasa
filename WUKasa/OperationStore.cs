﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BTreeFileUtil;

namespace WUKasa
{
    public class OperationStore
    {
        public const string FileNameTemplate = "OPR{0}.DAT";
        private int currentMax;
        private BTreeFile<Operation> btreeFile;

        public OperationStore(int year, string path)
        {
            currentMax = 0;
            btreeFile = new BTreeFile<Operation>(System.IO.Path.Combine(path, String.Format(FileNameTemplate, year)));
        }

        public int Count
        {
            get { return btreeFile.RecordsNumber; }
        }


        public void Add(Operation operation)
        {
            EnsureMax();
            currentMax++;
            operation.Max = currentMax;
            btreeFile.Add(operation);

        }

        private void EnsureMax()
        {
            if (currentMax == 0)
            {
                for (int i = 1; i <= btreeFile.TotalRecordNumber; i++)
                {
                    Operation opr = btreeFile.Get(i);
                    if (!opr.isDeleted)
                        currentMax = Math.Max(currentMax, opr.Max);
                }
            }
        }
    }
}
