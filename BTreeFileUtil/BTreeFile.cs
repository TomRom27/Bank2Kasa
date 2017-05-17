using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace BTreeFileUtil
{
    public class BTreeFile : IDisposable
    {
        private string filename;
        private int recSize;
        private bool useDeleteTag;
        private bool isOpen;
        private bool disposed;
        private byte[] buffer;


        public BTreeFile(string filename, Type recStructType , bool useDeleteTag)
        {
            this.filename = filename;
            //todo this.recSize = recSize;
            this.useDeleteTag = useDeleteTag;

            isOpen = false;
            disposed = false;
        }

        public void Open()
        {

        }

        public void Add(object o)
        {

        }

        public object Get(long pos)
        {
            return null; // todo
        }

        public void Close()
        {
            if (isOpen)
            {
                // todo
            }
        }

        private long GetStreamPos(int recNumber)
        {
            return 0; // todo
        }

        #region IDisposable related
        public void Dispose()
        {
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

    public struct BTreeFileHeader
    {
        public int FirstFree;
        public int NumFree;
        public int NumRec;
        public int RecLen;
        public int I5;
        bool AllowDup;
    }

    public class DeleteTagRecord
    {
        int NextDeletedPos { get; set; }
        object Data;
    }
}
