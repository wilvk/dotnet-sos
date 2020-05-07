using System;
using System.IO;
using Microsoft.DiaSymReader;
using Microsoft.DiaSymReader.PortablePdb;
using System.Reflection;

namespace test_pdb
{
    class Program
    {
        static void Main(string[] args)
        {
            var symBinder = new SymBinder();

            string peFilePath = Path.GetFullPath("../../artefacts/test_debug.dll");
            string pdbFilePath = Path.GetFullPath("../../artefacts/test_debug.pdb");

            string searchPath = null;

            MemoryStream ms = new MemoryStream();
            FileStream file = new FileStream(pdbFilePath, FileMode.Open, FileAccess.Read);
            file.CopyTo(ms);

            ISymUnmanagedReader symReader;

            Console.WriteLine("Hello World!");
        }
    }
}
