using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTreeFileUtil
{
    public interface IBTreeRecord
    {
        byte[] GetBytes();
        void SetFromBytes(byte[] bytes);

        int GetSize();
    }
}
