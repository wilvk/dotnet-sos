using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.IO;
using System.Reflection.Metadata;

namespace test_pdb
{
    public static class Symbols
    {
        public static MetadataReader GetMetadataReaderFromFile(string path)
        {
            var file = File.OpenRead(path);
            var mdProvider = MetadataReaderProvider.FromMetadataStream(file);
            return mdProvider.GetMetadataReader();
        }

        public static string GetMetadataVersion(MetadataReader reader)
        {
            return reader.MetadataVersion;
        }

        public static int GetEntrypointAddress(MetadataReader reader)
        {
            return Helpers.Address(reader, reader.DebugMetadataHeader.EntryPoint);
        }

        public static ModuleDefinition GetModule(MetadataReader reader)
        {
          return reader.GetModuleDefinition();
        }

        public static List<TypeReferenceHandle> GetTypeReferenceHandles(MetadataReader reader)
        {
            return reader.TypeReferences.ToList();
        }

        public static List<TypeReference> GetTypeReferences(MetadataReader reader)
        {
            var typeReferences = new List<TypeReference>();
            var typeReferenceHandles = GetTypeReferenceHandles(reader);

            foreach(var typeReferenceHandle in typeReferenceHandles)
            {
              var typeReference = reader.GetTypeReference(typeReferenceHandle);
              typeReferences.Add(typeReference);
            }

            return typeReferences;
        }

        public static List<TypeDefinitionHandle> GetTypeDefinitionHandles(MetadataReader reader)
        {
            return reader.TypeDefinitions.ToList();
        }

        public static List<TypeDefinition> GetTypeDefinitions(MetadataReader reader)
        {
            var typeDefinitions = new List<TypeDefinition>();
            var typeDefinitionHandles = GetTypeDefinitionHandles(reader);

            foreach(var typeDefinitionHandle in typeDefinitionHandles)
            {
              var typeDefinition = reader.GetTypeDefinition(typeDefinitionHandle);
              typeDefinitions.Add(typeDefinition);
            }

            return typeDefinitions;
        }

        public static List<TypeLayout> GetTypeLayouts(MetadataReader reader)
        {
            var typeLayouts = new List<TypeLayout>();
            var typeDefinitions = GetTypeDefinitions(reader);

            foreach(var typeDefinition in typeDefinitions)
            {
              typeLayouts.Add(typeDefinition.GetLayout());
            }

            return typeLayouts;
        }

        public static List<FieldDefinitionHandle> GetFieldDefinitionHandles(MetadataReader reader)
        {
            return reader.FieldDefinitions.ToList();
        }

        public static List<FieldDefinition> GetFieldDefinitions(MetadataReader reader)
        {
            var fieldDefinitions = new List<FieldDefinition>();
            var fieldDefinitionHandles = GetFieldDefinitionHandles(reader);

            foreach(var fieldDefinitionHandle in fieldDefinitionHandles)
            {
              var fieldDefinition = reader.GetFieldDefinition(fieldDefinitionHandle);
              fieldDefinitions.Add(fieldDefinition);
            }

            return fieldDefinitions;
        }


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
