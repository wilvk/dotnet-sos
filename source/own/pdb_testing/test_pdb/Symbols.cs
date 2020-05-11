using System;
using System.Reflection.PortableExecutable;
using System.IO;

namespace test_pdb
{
    public static class Pdb
    {


// From Microsoft.Metadata.Visualizer lib
//         public void Visualize(int generation = -1)
//        {
//            _reader = (generation >= 0) ? _readers[generation] : _readers[_readers.Count - 1];
//
//            WriteModule();
//            WriteTypeRef();
//            WriteTypeDef();
//            WriteField();
//            WriteMethod();
//            WriteParam();
//            WriteMemberRef();
//            WriteConstant();
//            WriteCustomAttribute();
//            WriteDeclSecurity();
//            WriteStandAloneSig();
//            WriteEvent();
//            WriteProperty();
//            WriteMethodImpl();
//            WriteModuleRef();
//            WriteTypeSpec();
//            WriteEnCLog();
//            WriteEnCMap();
//            WriteAssembly();
//            WriteAssemblyRef();
//            WriteFile();
//            WriteExportedType();
//            WriteManifestResource();
//            WriteGenericParam();
//            WriteMethodSpec();
//            WriteGenericParamConstraint();
//
//             debug tables:
//            WriteDocument();
//            WriteMethodDebugInformation();
//            WriteLocalScope();
//            WriteLocalVariable();
//            WriteLocalConstant();
//            WriteImportScope();
//            WriteCustomDebugInformation();
//
//             heaps:
//            WriteUserStrings();
//            WriteStrings();
//            WriteBlobs();
//            WriteGuids();
//        }

        public static void VisHeaders(PEReader peReader)
        {
        }

        public static void VisMemberRefs(PEReader peReader)
        {
        }

        public static void VisualizeDebugDirectory(PEReader peReader)
        {
            var entries = peReader.ReadDebugDirectory();

            if (entries.Length == 0)
            {
                return;
            }

            Console.WriteLine("Debug Directory:");
            foreach (var entry in entries)
            {
                Console.WriteLine($"  {entry.Type} stamp=0x{entry.Stamp:X8}, version=(0x{entry.MajorVersion:X4}, 0x{entry.MinorVersion:X4}), size={entry.DataSize}");

                try
                {
                    switch (entry.Type)
                    {
                        case DebugDirectoryEntryType.CodeView:
                            var codeView = peReader.ReadCodeViewDebugDirectoryData(entry);
                            Console.WriteLine($"    path='{codeView.Path}', guid={{{codeView.Guid}}}, age={codeView.Age}");
                            break;
                    }
                }
                catch (BadImageFormatException)
                {
                    Console.WriteLine("<bad data>");
                }
            }
            Console.WriteLine();
        }
    }
}
