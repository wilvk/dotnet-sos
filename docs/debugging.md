# setting a breakpoint:

https://docs.microsoft.com/en-us/dotnet/framework/unmanaged-api/debugging/icordebug-interface?redirectedfrom=MSDN

https://docs.microsoft.com/en-us/dotnet/framework/unmanaged-api/debugging/icordebugmanagedcallback-interface

vscode uses ICorDebug on linux and mac:
https://github.com/dotnet/runtime/issues/8042

the runtime hosts ICorDebug:
the BOTR has notes on DAC and ICorDebug:

https://github.com/dotnet/runtime/tree/master/docs/design/coreclr/botr
https://github.com/dotnet/runtime/tree/master/docs/design/coreclr/botr/dac-notes.md

debug src:
https://github.com/dotnet/runtime/tree/master/src/coreclr/src/debug

old article on ICorDebug debugger:
https://lowleveldesign.org/2010/10/11/writing-a-net-debugger-part-1-starting-the-debugging-session/

ICorDebug debugging methods:
https://docs.microsoft.com/en-us/dotnet/framework/unmanaged-api/debugging/debugging-interfaces

ICorDebug Breakpoing methods:
https://docs.microsoft.com/en-us/dotnet/framework/unmanaged-api/debugging/icordebugbreakpoint-interface

oldish article on debugging dotnet core from the command line:
https://codeblog.dotsandbrackets.com/command-line-debugging-core-linux/

long discussion on debugging .net: (to read)
https://github.com/dotnet/core/issues/505

debugging/clr dlls:

 53K Apr 15 01:46 System.Globalization.Native.so
771K Apr 15 01:46 System.IO.Compression.Native.so
 64K Apr 15 01:46 System.Native.so
 15K Apr 15 01:46 System.Net.Http.Native.so
 15K Apr 15 01:46 System.Net.Security.Native.so
115K Apr 15 01:46 System.Security.Cryptography.Native.OpenSsl.so
3.0M Apr 15 01:46 libclrjit.so
 92M May 18 13:59 libcoreclr.so
905K Apr 15 01:46 libdbgshim.so
3.4M Apr 15 01:46 libmscordaccore.so
2.6M Apr 15 01:46 libmscordbi.so
3.0M Apr 15 01:46 libvsbaseservices.so
 20M Apr 15 01:46 libvsdbg.so
 19M Apr 15 01:46 libvsdebugeng.impl.so
 18M Apr 15 01:46 libvsdebugeng.so

todo:
- read botr docs
- add to diagram
- look at coreclr ./src/debug/di/process.cpp
- look at decompiling IL from vsdbg
- look at clrmd2/src/TestTasks/src/Debugger.cs line 62 test




https://github.com/dotnet/coreclr/blob/master/Documentation/botr/ryujit-overview.md

ICorJitCompiler – this is the interface that the JIT compiler implements.
compileMethod is the main entry point for the JIT - optionally returns debugging info
implementation here:  https://github.com/dotnet/coreclr/blob/master/src/jit/ee_il_dll.cpp

RyuJit Debugger info -
https://github.com/dotnet/coreclr/blob/master/Documentation/botr/ryujit-overview.md#debugger-info

Mapping of IL offsets to native code offsets. This is accomplished via:

    the m_ILOffsetX on the statement nodes (Statement)
    the gtLclILoffs on lclVar references (GenTreeLclVar)

debugging support: Line number info:

https://github.com/dotnet/coreclr/blob/a9f3fc16483eecfc47fb79c362811d870be02249/src/jit/ee_il_dll.cpp#L917

debugging support: local var info:

https://github.com/dotnet/coreclr/blob/a9f3fc16483eecfc47fb79c362811d870be02249/src/jit/ee_il_dll.cpp#L621

ICorDebug Com discussion:
https://github.com/dotnet/runtime/issues/7968

## dlls:


    coreclr - This is the main runtime DLL (the GC, class loader, interop are all here)
    corjit - This is the Just In Time (JIT) compiler that compiles .NET Intermediate language to native code.
    corerun - This is the simple host program that can load the CLR and run a .NET Core application
    crossgen - This is the host program that runs the JIT compiler and produces .NET Native images (*.ni.dll) for C# code.



## following netcoredbg

dbgshim.so is a useful starting point:

dotnet/coreclr:  /src/dlls/dbgshim

### instructions on installing:

CC=clang CXX=clang++ cmake .. -DCMAKE_INSTALL_PREFIX=$PWD/../bin all
make
cd ./build/src/debug/netcoredbg
cp /work/source/cloned/netcoredbg/.dotnet/shared/Microsoft.NETCore.App/2.1.16/libdbgshim.so .
