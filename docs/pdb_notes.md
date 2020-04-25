# Notes on understanding pdb files

Debug Interface Access SDK:
https://docs.microsoft.com/en-us/visualstudio/debugger/debug-interface-access/debug-interface-access-sdk-reference?view=vs-2015&redirectedfrom=MSDN

Symbol reader:
https://github.com/dotnet/symreader-portable

Good overview of non-portable pdb (symbol) files:
https://www.wintellect.com/pdb-files-what-every-developer-must-know/

## debuging dotnet core on linux with llvm
http://sonicguo.com/2018/How-To-Debugging-DotNet-Core-On-Linux/


## debugging asp.net core source

https://www.raydbg.com/2018/Debugging-ASP-NET-Core-Application-with-Framework-Source-Code/

## SourceLink

https://github.com/dotnet/sourcelink

source control metadata

## metadata visualiser tools:
https://github.com/dotnet/metadata-tools

Tool for diagnosing and displaying content of ECMA335 metadata files and Portable PDBs.


## Reading .net pdb files:
https://github.com/dotnet/symreader-portable

infoq overview:
https://www.infoq.com/news/2017/02/Portable-PDB/

ppdb overview:
https://github.com/dotnet/core/blob/master/Documentation/diagnostics/portable_pdb.md

New applications should use System.Reflection.Metadata
https://www.nuget.org/packages/System.Reflection.Metadata

https://docs.microsoft.com/en-us/dotnet/api/system.reflection.metadata?view=netcore-3.1

assembly metadata dumper:
https://github.com/microsoft/dotnet-samples/tree/master/System.Reflection.Metadata/MdDumper

CLI ECMA standard:
http://www.ecma-international.org/publications/standards/Ecma-335.htm

Portable Executable (.exe, .dll) reader class.
https://docs.microsoft.com/en-us/dotnet/api/system.reflection.portableexecutable.pereader?view=netcore-3.1

## reading a pdb:

Reads the data pointed to by the specified Debug Directory entry and interprets it as an Embedded Portable PDB blob.

```
https://docs.microsoft.com/en-us/dotnet/api/system.reflection.portableexecutable.pereader.readembeddedportablepdbdebugdirectorydata?view=netcore-3.1#System_Reflection_PortableExecutable_PEReader_ReadEmbeddedPortablePdbDebugDirectoryData_System_Reflection_PortableExecutable_DebugDirectoryEntry_
```


## .net framework backwards compatibility:
for MdDumper project

https://stackoverflow.com/questions/50755065/net-core-in-linux-build-c-sharp
dotnet add package Microsoft.NETFramework.ReferenceAssemblies
chmod -R 0755 ./
dotnet build
change project file to netcoreapp3.1
change frameworks to framework
dotnet run ../../../../dotnet-sos/test_app/bin/Debug/netcoreapp3.1/test_app.dll


## extending gdb:

https://sourceware.org/gdb/current/onlinedocs/gdb/Extending-GDB.html


## debugger history:
https://blog.lextudio.com/the-rough-history-of-net-core-debuggers-b9fb206dc4aa


## Metadata reader class
https://docs.microsoft.com/en-us/dotnet/api/system.reflection.metadata.metadatareader?view=netcore-3.1

github question:
https://github.com/dotnet/runtime/issues/25899

### Sequence points

https://benhall.io/c-debug-vs-release-builds-and-debugging-in-visual-studio-from-novice-to-expert-in-one-blog-article/

sequence points - allow for debugging

ILEmitStyle.Debug – no optimization of IL in addition to adding nop instructions in order to map sequence points to IL

Put simply, a .pdb file stores debugging information about your DLL or EXE, which will help a debugger map the IL instructions to the original C# code.

You can ignore IsJITTrackingEnabled, as it is has been ignored by the JIT compiler since .NET 2.0. The JIT compiler will always generate tracking information during debugging to match up IL with its machine code and track where local variables and function arguments are stored


https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.debuggableattribute.isjittrackingenabled?redirectedfrom=MSDN&view=netcore-3.1#System_Diagnostics_DebuggableAttribute_IsJITTrackingEnabled

Specifically, the compiler tracks the Microsoft Intermediate Language (MSIL)-offset to the native-code offset within a method.


DebuggingModes.IgnoreSymbolStoreSequencePoints tells the debugger to work out the sequence points from the IL instead of loading the .pdb file, which would have performance implications. Sequence points are used to map locations in the IL code to locations in your C# source code.



