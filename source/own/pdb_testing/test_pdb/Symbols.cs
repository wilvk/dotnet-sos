using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Reflection.Metadata;

namespace test_pdb
{
    public static class Symbols
    {

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

        public static TypeDefinition GetTypeDefinition(MetadataReader reader, TypeDefinitionHandle handle)
        {
            return reader.GetTypeDefinition(handle);
        }

        public static List<TypeDefinition> GetTypeDefinitions(MetadataReader reader)
        {
            var typeDefinitions = new List<TypeDefinition>();
            var typeDefinitionHandles = GetTypeDefinitionHandles(reader);

            foreach(var typeDefinitionHandle in typeDefinitionHandles)
            {
              var typeDefinition = GetTypeDefinition(reader, typeDefinitionHandle);
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

        public static List<(TypeDefinition, TypeLayout)> GetTypeDefinitionsLayoutTuple(MetadataReader reader)
        {
            var typeDefinitionLayouts = new List<(TypeDefinition, TypeLayout)>();
            var typeDefinitions = GetTypeDefinitions(reader);

            foreach(var typeDefinition in typeDefinitions)
            {
              typeDefinitionLayouts.Add((typeDefinition, typeDefinition.GetLayout()));
            }

            return typeDefinitionLayouts;
        }

        public static List<FieldDefinitionHandle> GetFieldDefinitionHandles(MetadataReader reader)
        {
            return reader.FieldDefinitions.ToList();
        }

        public static FieldDefinition GetFieldDefinition(MetadataReader reader, FieldDefinitionHandle handle)
        {
            return reader.GetFieldDefinition(handle);
        }

        public static List<FieldDefinition> GetFieldDefinitions(MetadataReader reader)
        {
            var fieldDefinitions = new List<FieldDefinition>();
            var fieldDefinitionHandles = GetFieldDefinitionHandles(reader);

            foreach(var fieldDefinitionHandle in fieldDefinitionHandles)
            {
              var fieldDefinition = GetFieldDefinition(reader, fieldDefinitionHandle);
              fieldDefinitions.Add(fieldDefinition);
            }

            return fieldDefinitions;
        }

        public static List<MethodDefinitionHandle> GetMethodDefinitionHandles(MetadataReader reader)
        {
            return reader.MethodDefinitions.ToList();
        }

        public static List<MethodDefinition> GetMethodDefinitions(MetadataReader reader)
        {
            var methodDefinitions = new List<MethodDefinition>();
            var methodDefinitionHandles = GetMethodDefinitionHandles(reader);

            foreach(var methodDefinitionHandle in methodDefinitionHandles)
            {
                var methodDefinition = reader.GetMethodDefinition(methodDefinitionHandle);
                methodDefinitions.Add(methodDefinition);
            }
            return methodDefinitions;
        }

        public static MethodDefinition GetMethodDefinition(MetadataReader reader, MethodDefinitionHandle handle)
        {
            return reader.GetMethodDefinition(handle);
        }

        public static MethodImport GetMethodImport(MethodDefinition method)
        {
            return method.GetImport();
        }

        public static List<(MethodDefinition, MethodImport)> GetMethodDefinitionImports(MetadataReader reader)
        {
            var methodDefinitionImports = new List<(MethodDefinition, MethodImport)>();
            var methodDefinitions = GetMethodDefinitions(reader);

            foreach(var methodDefinition in methodDefinitions)
            {
              var methodImport = GetMethodImport(methodDefinition);
              methodDefinitionImports.Add((methodDefinition, methodImport));
            }
            return methodDefinitionImports;
        }

        public static Parameter GetParameter(MetadataReader reader, ParameterHandle handle)
        {
            return reader.GetParameter(handle);
        }

        public static List<Parameter> GetMethodParameters(MetadataReader reader, MethodDefinition method)
        {
            var parameters = new List<Parameter>();
            var parameterHandles = method.GetParameters();
            foreach(var parameterHandle in parameterHandles)
            {
                var parameter = GetParameter(reader, parameterHandle);
                parameters.Add(parameter);
            }
            return parameters;
        }

        public static GenericParameter GetGenericParameter(MetadataReader reader, GenericParameterHandle handle)
        {
            return reader.GetGenericParameter(handle);
        }

        public static List<GenericParameter> GetGenericMethodParameters(MetadataReader reader, MethodDefinition method)
        {
            var genericParameters = new List<GenericParameter>();
            var genericParameterHandles = method.GetGenericParameters();
            foreach(var genericParameterHandle in genericParameterHandles)
            {
                var genericParameter = GetGenericParameter(reader, genericParameterHandle);
                genericParameters.Add(genericParameter);
            }
            return genericParameters;
        }

        public static List<DocumentHandle> GetDocumentHandles(MetadataReader reader)
        {
            return reader.Documents.ToList();
        }

        public static Document GetDocumentFromdocumentHandle(MetadataReader reader, DocumentHandle handle)
        {
            return reader.GetDocument(handle);
        }

        public static List<Document> GetDocuments(MetadataReader reader)
        {
            var documents = new List<Document>();
            var documentHandles = GetDocumentHandles(reader);
            foreach(var documentHandle in documentHandles)
            {
                var document = GetDocumentFromdocumentHandle(reader, documentHandle);
                documents.Add(document);
            }
            return documents;
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
