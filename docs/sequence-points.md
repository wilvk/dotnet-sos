

using system.metadata.methoddebuginformation:

https://docs.microsoft.com/en-us/dotnet/api/system.reflection.metadata.methoddebuginformation.sequencepointsblob?view=netcore-3.1

methoddebuginformation seems to be the entrypoint for debugging information:

https://docs.microsoft.com/en-us/dotnet/api/system.reflection.metadata.methoddebuginformation?view=netcore-3.1


looking at symreader-portable:

./src/Microsoft.DiaSymReader.PortablePdb.Tests/SymBinderTests.cs:306:        public void GetReaderFromPdbFile()

looks like this is for gettinig a reader from a pdb file.

the reader can then be used to map code lines to il
