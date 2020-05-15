using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;


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

        public static Document GetDocumentFromDocumentHandle(MetadataReader reader, DocumentHandle handle)
        {
            return reader.GetDocument(handle);
        }

        public static List<Document> GetDocuments(MetadataReader reader)
        {
            var documents = new List<Document>();
            var documentHandles = GetDocumentHandles(reader);
            foreach(var documentHandle in documentHandles)
            {
                var document = GetDocumentFromDocumentHandle(reader, documentHandle);
                documents.Add(document);
            }
            return documents;
        }

        public static Document GetDocumentByNumber(MetadataReader reader, int number)
        {
            var documentHandles = GetDocumentHandles(reader);

            foreach(var documentHandle in documentHandles)
            {
                var rowNumber = reader.GetRowNumber(documentHandle);
                if(rowNumber == number)
                {
                    return GetDocumentFromDocumentHandle(reader, documentHandle);
                }
            }

           throw new ApplicationException("Error: unable to find Document with  number: " + number.ToString());
        }

        public static int GetDocumentNumberByName(MetadataReader reader, string name)
        {
            var documentHandles = GetDocumentHandles(reader);

            foreach(var documentHandle in documentHandles)
            {
                var document = GetDocumentFromDocumentHandle(reader, documentHandle);

                if(name == Helpers.DocumentName(reader, document.Name))
                {
                  return reader.GetRowNumber(documentHandle);
                }
            }

            return 0;
        }

        public static List<MethodDebugInformationHandle> GetMethodDebugInformationHandles(MetadataReader reader)
        {
            return reader.MethodDebugInformation.ToList();
        }

        public static List<MethodDebugInformation> GetMethodDebugInformation(MetadataReader reader)
        {
            var methodDebugInformationList = new List<MethodDebugInformation>();
            var methodDebugInformationHandles = GetMethodDebugInformationHandles(reader);

            foreach(var methodDebugInformationHandle in methodDebugInformationHandles)
            {
                var methodDebugInformation = reader.GetMethodDebugInformation(methodDebugInformationHandle);
                methodDebugInformationList.Add(methodDebugInformation);
            }

            return methodDebugInformationList;
        }

        public static int GetRowNumberFromDocumentHandle(MetadataReader reader, DocumentHandle documentHandle)
        {
            return reader.GetRowNumber(documentHandle);
        }

        public static List<(MethodDebugInformation, List<SequencePoint>)> GetMethodInformationAndSequencePoints(MetadataReader reader)
        {
            var methodDebugInformationAndSequencePoints = new List<(MethodDebugInformation, List<SequencePoint>)>();
            var methodDebugInformation = GetMethodDebugInformation(reader);

            foreach(var methodDebugInfo in methodDebugInformation)
            {
                var sequencePoints = GetSequencePointsFromMethodDebugInformation(methodDebugInfo);
                methodDebugInformationAndSequencePoints.Add((methodDebugInfo, sequencePoints));
            }

            return methodDebugInformationAndSequencePoints;
        }

        public static List<SequencePoint> GetSequencePointsFromMethodDebugInformation(MethodDebugInformation method)
        {
            return method.GetSequencePoints().ToList();
        }

//        public static string SequencePoint(SequencePoint sequencePoint, bool includeDocument = true)
//        {
//            string range = sequencePoint.IsHidden ?
//                "<hidden>" :
//                $"({sequencePoint.StartLine}, {sequencePoint.StartColumn}) - ({sequencePoint.EndLine}, {sequencePoint.EndColumn})" +
//                    (includeDocument ? $" [{RowId(() => sequencePoint.Document)}]" : "");
//
//            return $"IL_{sequencePoint.Offset:X4}: " + range;
//        }
//


//        public void WriteMethodDebugInformation()
//        {
//            if (_reader.MethodDebugInformation.Count == 0)
//            {
//                return;
//            }
//
//            _writer.WriteLine(MakeTableName(TableIndex.MethodDebugInformation));
//            _writer.WriteLine(new string('=', 50));
//
//            foreach (var handle in _reader.MethodDebugInformation)
//            {
//                if (handle.IsNil)
//                {
//                    continue;
//                }
//
//                var entry = _reader.GetMethodDebugInformation(handle);
//
//                bool hasSingleDocument = false;
//                bool hasSequencePoints = false;
//                try
//                {
//                    hasSingleDocument = !entry.Document.IsNil;
//                    hasSequencePoints = !entry.SequencePointsBlob.IsNil;
//                }
//                catch (BadImageFormatException)
//                {
//                    hasSingleDocument = hasSequencePoints = false;
//                }
//
//                _writer.WriteLine($"{MetadataTokens.GetRowNumber(handle):x}: {HeapOffset(() => entry.SequencePointsBlob)}");
//
//                if (!hasSequencePoints)
//                {
//                    continue;
//                }
//
//                _blobKinds[entry.SequencePointsBlob] = BlobKind.SequencePoints;
//
//                _writer.WriteLine("{");
//
//                bool addLineBreak = false;
//
//                if (!TryGetValue(() => entry.GetStateMachineKickoffMethod(), out var kickoffMethod) || !kickoffMethod.IsNil)
//                {
//                    _writer.WriteLine($"  Kickoff Method: {(kickoffMethod.IsNil ? BadMetadataStr : Token(kickoffMethod))}");
//                    addLineBreak = true;
//                }
//
//                if (!TryGetValue(() => entry.LocalSignature, out var localSignature) || !localSignature.IsNil)
//                {
//                    _writer.WriteLine($"  Locals: {(localSignature.IsNil ? BadMetadataStr : Token(localSignature))}");
//                    addLineBreak = true;
//                }
//
//                if (hasSingleDocument)
//                {
//                    _writer.WriteLine($"  Document: {RowId(() => entry.Document)}");
//                    addLineBreak = true;
//                }
//
//                if (addLineBreak)
//                {
//                    _writer.WriteLine();
//                }
//
//                try
//                {
//                    foreach (var sequencePoint in entry.GetSequencePoints())
//                    {
//                        _writer.Write("  ");
//                        _writer.WriteLine(SequencePoint(sequencePoint, includeDocument: !hasSingleDocument));
//                    }
//                }
//                catch (BadImageFormatException)
//                {
//                    _writer.WriteLine("  " + BadMetadataStr);
//                }
//
//                _writer.WriteLine("}");
//            }
//
//            _writer.WriteLine();
//        }
//
//
//



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
