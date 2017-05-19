using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BTreeFileUtil
{
    public class BTreeFile<T> : IDisposable where T : IBTreeRecord, new()
    {
        private string filename;
        private int dataSize;
        private byte[] dataBuffer;
        private FileStream dataFile;
        private BTreeFileHeader header;
        private bool isOpen;
        private bool disposed;

        public BTreeFile(string filename )
        {
            this.filename = filename;
            dataSize = new T().GetSize();
            dataBuffer = new byte[dataSize];
            isOpen = false;
            disposed = false;
            header = new BTreeFileHeader();
        }

        public int TotalRecordNumber
        {
            get { return header.TotalRecordNumber; }
        }

        public void Open()
        {
            dataFile = File.Open(filename, FileMode.Open, FileAccess.ReadWrite);
            ReadHeader();
            isOpen = true;
        }

        public void Add(T o)
        {

        }

        public T Get(int recPos)
        {
            if (recPos > header.TotalRecordNumber)
                throw new ArgumentOutOfRangeException($"Expected record position {recPos} is bigger then allowed {header.TotalRecordNumber}");

            dataFile.Seek(recPos * dataSize, SeekOrigin.Begin);
            dataFile.Read(dataBuffer, 0, dataSize);

            T o = new T();
            o.SetFromBytes(dataBuffer);

            return o;
        }

        public void Close()
        {
            if (isOpen)
            {
                WriteHeader();
                dataFile.Close();
                isOpen = false;
            }
        }

        private void WriteHeader()
        {
            throw new NotImplementedException();
        }

        private void ReadHeader()
        {
            byte[] headerBytes = new byte[Marshal.SizeOf(header)];
            dataFile.Seek(0, SeekOrigin.Begin);
            dataFile.Read(headerBytes, 0, headerBytes.Length);
            header = StructHelper.BytesToStruct<BTreeFileHeader>(ref headerBytes);
        }

        #region IDisposable related
        public void Dispose()
        {
            Close();
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
                    // todo
                }
                // Indicate that the instance has been disposed.
                disposed = true;
            }
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BTreeFileHeader
    {
        public int FirstFree;
        public int DeletedRecordsNumber;
        public int RecordsNumber;
        public int RecordLength;
        public int I5;
        bool AllowDup;

        public int TotalRecordNumber
        {
            get { return DeletedRecordsNumber + RecordsNumber; }
        }
    }
}
