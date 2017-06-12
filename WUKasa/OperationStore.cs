using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BTreeFileUtil;

namespace WUKasa
{
    public class OperationStore : IDisposable
    {
        public const string FileNameTemplate = "OPR{0}.DAT";
        public const string IndexExt = "IX";
        private int currentMax;
        private BTreeFile<Operation> btreeFile;
        private bool disposed;

        public OperationStore(int year, string path)
        {
            currentMax = 0;
            btreeFile = new BTreeFile<Operation>(System.IO.Path.Combine(path, String.Format(FileNameTemplate, year)));
        }

        public int Count
        {
            get { return btreeFile.RecordsNumber; }
        }

        public int CountWithDeleted
        {
            get { return btreeFile.TotalRecordNumber; }
        }


        public void Add(Operation operation)
        {
            EnsureMax();
            currentMax++;
            operation.Max = currentMax;
            btreeFile.Add(operation);

        }

        public void Put(Operation operation, int pos)
        {
            btreeFile.Put(operation, pos);
        }

        public void ForEach(Action<Operation, int> action)
        {
            for (int i = 1; i <= btreeFile.TotalRecordNumber; i++)
            {
                Operation opr = btreeFile.Get(i);
                action(opr, i);
            }
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

        private void EnsureMax2()
        {
            if (currentMax == 0)
            {
                ForEach((opr, i) =>
                {
                    if (!opr.isDeleted)
                        currentMax = Math.Max(currentMax, opr.Max);
                });
            }
        }


        #region IDisposable related
        public void Dispose()
        {
            btreeFile.Dispose();
            Dispose(true);

            // Use SupressFinalize in case a subclass 
            // of this type implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    btreeFile.Dispose();
                }
                // Indicate that the instance has been disposed.
                disposed = true;
            }
        }
        #endregion
    }
}
