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
            var stream = Readers.GetFileStreamFromFilePath(path);
            var isPe = Readers.IsPEStream(stream);
            var isMd = Readers.IsManagedMetadata(stream);
            var reader = Readers.GetMetadataReaderFromFileStream(stream);
            Console.WriteLine("PE: " + isPe.ToString());
            Console.WriteLine("MD: " + isMd.ToString());

            var documents = Symbols.GetDocuments(reader);

            Console.WriteLine("Doc count: " + documents.Count.ToString());

            foreach(var doc in documents)
            {
                var documentName = Helpers.DocumentName(reader, doc.Name);
                var documentNumber = Symbols.GetDocumentNumberByName(reader, documentName);
                Console.WriteLine("  #:    "  + documentNumber.ToString() + ", " + documentName);;
            }

            var debugInfoAndSequences = Symbols.GetMethodDebugInformationHandlesAndSequencePoints(reader);

            foreach(var (md, sql) in debugInfoAndSequences)
            {
                var methodDebugInformation = Symbols.GetMethodDebugInformation(reader, md);
                if(!methodDebugInformation.Document.IsNil)
                {
                    var document = Symbols.GetDocumentFromDocumentHandle(reader, methodDebugInformation.Document);
                    var docName = Helpers.DocumentName(reader, document.Name);

                    Console.WriteLine("DocName: " + docName);
                    var methodNumber = Symbols.GetMethodRowNumberFromMethodDebugInformationHandle(reader, md);
                    Console.WriteLine(" Method#: " + methodNumber.ToString());

                    foreach(var sp in sql)
                    {
                        Console.WriteLine("  Start Line:   " + sp.StartLine.ToString());
                        Console.WriteLine("  End   Line:   " + sp.EndLine.ToString());
                        Console.WriteLine("  Start Column: " + sp.StartColumn.ToString());
                        Console.WriteLine("  End   Column: " + sp.EndColumn.ToString());
                        Console.WriteLine("  IL offset:    " + sp.Offset.ToString());
                        Console.WriteLine("  IS Hidden: :  " + sp.IsHidden.ToString());
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }
            }
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
