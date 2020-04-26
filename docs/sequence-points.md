

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


the sos debugger is part of diagnostics:

https://github.com/dotnet/diagnostics

specifically, the source for it is here:

https://github.com/dotnet/diagnostics/tree/master/src/SOS/SOS.NETCore

the method GetInfoForMethod():

https://github.com/dotnet/diagnostics/blob/9a2ccfbf15679c0bd3da6fc3f077e9b70c79d093/src/SOS/SOS.NETCore/SymbolReader.cs#L769

Returns source name, line numbers and IL offsets for given method token. Used by the GDBJIT support.

signature:

```
internal static bool GetInfoForMethod(string assemblyPath, int methodToken, ref MethodDebugInfo debugInfo)
```

requires a MethodDebugInfo object to operate on. this can be retrieved from System.Reflection.Metadata above.

GetDebugInfoForMethod() also returns a list of sequence points.

https://github.com/dotnet/diagnostics/blob/9a2ccfbf15679c0bd3da6fc3f077e9b70c79d093/src/SOS/SOS.NETCore/SymbolReader.cs#L823


also in diagnstics, the bpmd command is listed mostly in the strike project:

 grep -rni bpmd .|grep -Evi 'test|txt'|grep -i bpmd


dotnet/diagnostics/src/SOS/
  lldbplugin/soscommand.cpp:133
  Strike/util.cpp:2755-60
  Strike/strike.cpp:7280-7304, 8052-8242

in strike.cpp L8203 onwards is where the breakpoint is set

on L8265 - call with Offset is defined - this is the il offset and can only be used with  `bpmd module_name managed_fucntion_name <iloffset>`


L8277 - after loading symbols, we resolve the sequence points for the method:
if(SUCCEEDED(symbolReader.ResolveSequencePoint(Filename, lineNumber, &methodDefToken, &ilOffset)))
