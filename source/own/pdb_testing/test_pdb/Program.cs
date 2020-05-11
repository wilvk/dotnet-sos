using System;
using System.IO;
using Microsoft.DiaSymReader.PortablePdb;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace test_pdb
{
    class Program
    {
        static void Main(string[] args)
        {
            var symBinder = new SymBinder();

            string peFilePath = Path.GetFullPath("../../artefacts/test_debug.dll");
            string pdbFilePath = Path.GetFullPath("../../artefacts/test_debug.pdb");
            var file = File.OpenRead(peFilePath);

            var peReader = new PEReader(file);

            Console.WriteLine("PER: " + peReader.PEHeaders.CoffHeader.Machine.ToString());
            var generationData = new GenerationData(peReader, peReader.GetMetadataReader(), peReader);
            Console.WriteLine("GD: " + generationData.MetadataReader.AssemblyFiles.ToString());

            Console.WriteLine("Hello World!");
        }
    }
}
