using System;
using System.Linq;
using System.IO;
using Microsoft.DiaSymReader.PortablePdb;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace test_pdb
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = Path.GetFullPath("../../artefacts/test_debug.pdb");
            var reader = Symbols.GetMetadataReaderFromFile(path);
            VisualizeHeaders(reader);
        }

        static void VisualizeHeaders(MetadataReader reader)
        {
            Console.WriteLine($"MetadataVersion: {reader.MetadataVersion}");

            if (reader.DebugMetadataHeader != null)
            {
                Console.WriteLine("Id: " + BitConverter.ToString(reader.DebugMetadataHeader.Id.ToArray()));

                if (!reader.DebugMetadataHeader.EntryPoint.IsNil)
                {
                    Console.WriteLine($"Entrypoint: {Helpers.Address(reader, reader.DebugMetadataHeader.EntryPoint)}");
                    Console.WriteLine($"Table:      {Helpers.Table(reader.DebugMetadataHeader.EntryPoint)}");
                }
            }
        }

    }
}
