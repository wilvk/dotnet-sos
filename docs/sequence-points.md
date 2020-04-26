

using system.metadata.methoddebuginformation:

https://docs.microsoft.com/en-us/dotnet/api/system.reflection.metadata.methoddebuginformation.sequencepointsblob?view=netcore-3.1

methoddebuginformation seems to be the entrypoint for debugging information:

https://docs.microsoft.com/en-us/dotnet/api/system.reflection.metadata.methoddebuginformation?view=netcore-3.1


there is also the metadatareaderprovider that covers ecma335 binary metadata and separate pdb files

https://docs.microsoft.com/en-us/dotnet/api/system.reflection.metadata.metadatareaderprovider?view=netcore-3.1

the reader can then be used to map code lines to il


looking at symreader-portable:

./src/Microsoft.DiaSymReader.PortablePdb.Tests/SymBinderTests.cs:306:        public void GetReaderFromPdbFile()

looks like this is for gettinig a reader from a pdb file.


the interesting bit starts at metadatareader:

https://docs.microsoft.com/en-us/dotnet/api/system.reflection.metadata.metadatareader?view=netcore-3.1

two things to check:

- [ ] System.Reflection.Metadata in coreclr
- [ ] bpmd in dotnet/diagnostics


to get the method's debugging information:

https://docs.microsoft.com/en-us/dotnet/api/system.reflection.metadata.metadatareader.getmethoddebuginformation?view=netcore-3.1#System_Reflection_Metadata_MetadataReader_GetMethodDebugInformation_System_Reflection_Metadata_MethodDebugInformationHandle_

returns a MethodDebugInformation Struct

https://docs.microsoft.com/en-us/dotnet/api/system.reflection.metadata.methoddebuginformation?view=netcore-3.1

has a method called GetSequencePoints(). this is what we need:

https://docs.microsoft.com/en-us/dotnet/api/system.reflection.metadata.methoddebuginformation.getsequencepoints?view=netcore-3.1#System_Reflection_Metadata_MethodDebugInformation_GetSequencePoints

returns a sequencepointcollection:

https://docs.microsoft.com/en-us/dotnet/api/system.reflection.metadata.methoddebuginformation.getsequencepoints?view=netcore-3.1#System_Reflection_Metadata_MethodDebugInformation_GetSequencePoints

that is a collection of SequencePoint Struct:

https://docs.microsoft.com/en-us/dotnet/api/system.reflection.metadata.sequencepoint?view=netcore-3.1

the properties on this struct are what do the mapping:

 Document
EndColumn
EndLine
IsHidden
Offset
StartColumn
StartLine



the test uses:

./src/Microsoft.DiaSymReader.PortablePdb/SymBinder.cs:356:        public int GetReaderFromPdbFile(
