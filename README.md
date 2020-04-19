# dotnet-sos
dotnet core and sos testing

## Approach

[x] Build binaries
[ ] Understand symbol files
[ ] Understand llvm sos debugging
[ ] (If needed) Modify sos debugger to stop on line numbers


## TODO:

determine how to set a breakpoint for a line in a file
determine how to interact with llvm in a container
determine how to match up ildasm code to lines in assembly
how to get a breakpoint for a specific line - and how to confirm it is correct
how to make a breakpoint hit more than once after continue in lldb
complete docker file with llvm from: https://apt.llvm.org/

## Links

## Notes

### Setting a breakpoint:

dotnet build ./test_debug/
dotnet ./test_debug/bin/Debug/netcoreapp3.1/test_debug.dll &
export TEST_PID=$!
lldb

plugin load /root/.dotnet/sos/libsosplugin.so
process attach -p $TEST_PID
bpmd test_debug.dll Program.GetTicksElapsed
process continue
clrstack -p


###  Setting a breakpoint for a line:

bpmd is for setting breakpoints. It contains many ways to set a breakpoint:
- method name
- file and line number
- address

for a file and line:

```
bpmd Program.cs:21
```


### Extended sos help

from llvl:

soshelp

or

soshelp <commamd>


## Symbols and symbol servers

symbols are a link between the source code representation and the location in memory that the line of code is running at.

There is also the ildasm that gives a msil representation of the code that is jitted to assembly

sos uses the symbols to map the code to the memory locations in debugging

todo: read https://github.com/dotnet/diagnostics/issues/121

llvm commands:

plugin load /app/diagnostics/artifacts/bin/Linux.x64.Debug/libsosplugin.so
sethostruntime /usr/share/dotnet/shared/Microsoft.NETCore.App/2.1.6
setclrpath /usr/share/dotnet/shared/Microsoft.NETCore.App/2.1.6
target create --core coredump.1

todo: determine sethostruntime and setclrpath

from https://github.com/dotnet/runtime/issues/9073

If you are using the 2.0.0 release runtime, you can use the nuget packages with the stripped symbols on myget:
https://dotnet.myget.org/feed/dotnet-core/package/nuget/runtime.linux-x64.Microsoft.NETCore.Runtime.CoreCLR/2.0.0
There is a link to "Download symbols". It gets you a nuget package, which can be renamed to .zip and unzipped to get the symbol files. Please note that the unzip on Linux for some reason sets access rights wrong on the files, so that they are not accessible even for read, so you'd need to chmod them.

If you are using a preview version of the runtime downloaded from https://github.com/dotnet/core-setup, then there are links to tarballs with symbols on that page next to the corresponding runtime links.


https://github.com/dotnet/runtime/issues/9213
