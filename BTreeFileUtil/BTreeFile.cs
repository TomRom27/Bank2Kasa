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
        public const uint NoFreeTag = 0xFFFFFFFF;
        private string filename;
        private int dataSize;
        private byte[] dataBuffer;
        private FileStream dataFile;
        private BTreeFileHeader header;
        private RecordHeader recHeader;
        private byte[] recHeaderBuffer;
        private int recHeaderSize;
        private bool isOpen;
        private bool disposed;

        public BTreeFile(string filename)
        {
            this.filename = filename;
            dataSize = new T().GetSize();
            dataBuffer = new byte[dataSize];
            isOpen = false;
            disposed = false;
            header = new BTreeFileHeader();
            recHeaderSize = 4; // todo 4 -> const
            recHeaderBuffer = new byte[recHeaderSize];
        }

        public int TotalRecordNumber
        {
            get
            {
                if (!isOpen)
                    Open();

                return header.TotalRecordsNumber;
            }
        }

        public int RecordsNumber
        {
            get
            {
                if (!isOpen)
                    Open();

                return header.RecordsNumber;
            }
        }

        private void Open()
        {
            dataFile = File.Open(filename, FileMode.Open, FileAccess.ReadWrite);
            ReadHeader();
            isOpen = true;
        }

        public void Add(T rec)
        {
            if (!isOpen)
                Open();

            if (header.DeletedRecordsNumber > 0)
                PutToDeleted(rec);
            else
                AddAsNew(rec);
        }

        private void AddAsNew(T o)
        {
            dataFile.Seek((header.RecordsNumber + 1) * dataSize, SeekOrigin.Begin);
            header.TotalRecordsNumber++;
            PutAtCurrentPos(o);
        }

        private void PutToDeleted(T o)
        {
            if ((header.FirstFree <= 0) || (header.FirstFree == NoFreeTag))
                throw new InvalidOperationException("Attempt to put to deleted while there is no deleted records in the file");

            uint freePos = header.FirstFree;
            // first we need to update the deleted structure 
            dataFile.Seek(freePos * dataSize, SeekOrigin.Begin);
            dataFile.Read(recHeaderBuffer, 0, recHeaderBuffer.Length);
            recHeader = StructHelper.BytesToStruct<RecordHeader>(ref recHeaderBuffer);
            header.FirstFree = recHeader.NextFree;
            header.DeletedRecordsNumber--;

            // now time to write actual data
            dataFile.Seek(freePos * dataSize, SeekOrigin.Begin);
            PutAtCurrentPos(o);
        }

        private void PutAtCurrentPos(T o)
        {
            dataFile.Write(o.GetBytes(), 0, dataSize);
            WriteHeader();
        }

        public T Get(int recPos)
        {
            if (!isOpen)
                Open();

            if (recPos > header.TotalRecordsNumber)
                throw new ArgumentOutOfRangeException($"Expected record position {recPos} is bigger then allowed {header.RecordsNumber}");

            dataFile.Seek(recPos * dataSize, SeekOrigin.Begin);
            dataFile.Read(dataBuffer, 0, dataSize);

            T o = new T();
            o.SetFromBytes(dataBuffer);

            return o;
        }

        public void Put(T o, int recPos)
        {
            if (!isOpen)
                Open();

            if (recPos > header.TotalRecordsNumber)
                throw new ArgumentOutOfRangeException($"Expected record position {recPos} is bigger then allowed {header.RecordsNumber}");

            dataFile.Seek(recPos * dataSize, SeekOrigin.Begin);
            dataFile.Write(o.GetBytes(), 0, dataSize);
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
            byte[] headerBytes = StructHelper.StructToBytes(header);
            dataFile.Seek(0, SeekOrigin.Begin);
            dataFile.Write(headerBytes, 0, headerBytes.Length);
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
                    Close();
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
        public uint FirstFree;
        public int DeletedRecordsNumber;
        public int TotalRecordsNumber;
        public int RecordLength;
        public int I5;
        bool AllowDup;

        public int RecordsNumber
        {
            get { return TotalRecordsNumber - DeletedRecordsNumber; }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RecordHeader
    {
        public uint NextFree;
    }
}
