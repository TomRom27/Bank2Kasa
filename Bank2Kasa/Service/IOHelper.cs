using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Bank2Kasa.Service
{
    public static class IOHelper
    {
        public static string RenameToExtension(string filename, string ext)
        {
            string newName;
            int counter = 0;

            newName = Path.ChangeExtension(filename, ext);
            while (true)
            {
                if (File.Exists(newName))
                {
                    counter++;
                    newName = Path.Combine(Path.GetDirectoryName(filename), Path.Combine(Path.GetFileNameWithoutExtension(newName) + $" ({counter})", ext));
                }
                else
                {
                    File.Move(filename, newName);
                    return newName;
                }
            }
        }
    }
}
