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

--
all instructions are codified in the makefile now:

terminal 1:

make dotnet-build

make d-buildnetcoredbg-310

make d-build-debug-310

terminal 2:

docker exec -it < id_of dotnet-build container > bash

cd /work/source/own/test_debug/
/app/corefx/artifacts/bin/testhost/netcoreapp-Linux-Debug-x64/dotnet exec ./bin/Debug/netcoreapp3.1/test_debug.dll
< starts test_debug app >

terminal 1:

ps aux
< find test_debug PID >

cd source/own/netcoredbg310/build/src/debug/netcoredbg/
./netcoredbg --attach < PID >

## Output listed on attach:


root@5499edf596ec:/work/source/own/netcoredbg310/build/src/debug/netcoredbg# ./netcoredbg --attach 1603
(gdb)
=library-loaded,id="{c494bb81-2820-4c33-848b-a8de8cd3793c}",target-name="/app/corefx/artifacts/bin/testhost/netcoreapp-Linux-Debug-x64/shared/Microsoft.NETCore.App/3.1.3/System.Private.CoreLib.dll",host-name="/app/corefx/artifacts/bin/testhost/netcoreapp-Linux-Debug-x64/shared/Microsoft.NETCore.App/3.1.3/System.Private.CoreLib.dll",symbols-loaded="0",base-address="0x7fee814db000",size="3011584"
=library-loaded,id="{ac810feb-8bd2-46ff-bb56-8343c1a334a7}",target-name="/work/source/own/test_debug/bin/Debug/netcoreapp3.1/test_debug.dll",host-name="/work/source/own/test_debug/bin/Debug/netcoreapp3.1/test_debug.dll",symbols-loaded="1",base-address="0x7fee87746000",size="4608"
=library-loaded,id="{f5f80e18-b06d-4701-97b9-5c6ce44afa01}",target-name="/app/corefx/artifacts/bin/testhost/netcoreapp-Linux-Debug-x64/shared/Microsoft.NETCore.App/3.1.3/System.Runtime.dll",host-name="/app/corefx/artifacts/bin/testhost/netcoreapp-Linux-Debug-x64/shared/Microsoft.NETCore.App/3.1.3/System.Runtime.dll",symbols-loaded="0",base-address="0x7fee8758a000",size="36864"
=library-loaded,id="{de74e8e8-d589-4acf-8196-9015172d2b2a}",target-name="/app/corefx/artifacts/bin/testhost/netcoreapp-Linux-Debug-x64/shared/Microsoft.NETCore.App/3.1.3/System.Threading.Thread.dll",host-name="/app/corefx/artifacts/bin/testhost/netcoreapp-Linux-Debug-x64/shared/Microsoft.NETCore.App/3.1.3/System.Threading.Thread.dll",symbols-loaded="0",base-address="0x7fee87588000",size="6144"
=library-loaded,id="{6a8b2ba5-e7c8-4125-a85f-e165efab5b9e}",target-name="/app/corefx/artifacts/bin/testhost/netcoreapp-Linux-Debug-x64/shared/Microsoft.NETCore.App/3.1.3/System.Console.dll",host-name="/app/corefx/artifacts/bin/testhost/netcoreapp-Linux-Debug-x64/shared/Microsoft.NETCore.App/3.1.3/System.Console.dll",symbols-loaded="0",base-address="0x7fee87573000",size="84480"
=library-loaded,id="{5d06fb5f-352f-4320-9925-64d3eed5be16}",target-name="/app/corefx/artifacts/bin/testhost/netcoreapp-Linux-Debug-x64/shared/Microsoft.NETCore.App/3.1.3/System.Runtime.Extensions.dll",host-name="/app/corefx/artifacts/bin/testhost/netcoreapp-Linux-Debug-x64/shared/Microsoft.NETCore.App/3.1.3/System.Runtime.Extensions.dll",symbols-loaded="0",base-address="0x7fee87553000",size="90112"
=library-loaded,id="{da07e87c-65ea-4cf7-93c9-7d729520a52c}",target-name="/app/corefx/artifacts/bin/testhost/netcoreapp-Linux-Debug-x64/shared/Microsoft.NETCore.App/3.1.3/System.Threading.dll",host-name="/app/corefx/artifacts/bin/testhost/netcoreapp-Linux-Debug-x64/shared/Microsoft.NETCore.App/3.1.3/System.Threading.dll",symbols-loaded="0",base-address="0x7fee80200000",size="43008"
=library-loaded,id="{782fc2d1-0856-4fc4-930e-64399ab60dac}",target-name="/app/corefx/artifacts/bin/testhost/netcoreapp-Linux-Debug-x64/shared/Microsoft.NETCore.App/3.1.3/System.Diagnostics.Debug.dll",host-name="/app/corefx/artifacts/bin/testhost/netcoreapp-Linux-Debug-x64/shared/Microsoft.NETCore.App/3.1.3/System.Diagnostics.Debug.dll",symbols-loaded="0",base-address="0x7fee8756d000",size="5120"
=library-loaded,id="{a1520061-1d96-4246-87de-7936528dbcbf}",target-name="/app/corefx/artifacts/bin/testhost/netcoreapp-Linux-Debug-x64/shared/Microsoft.NETCore.App/3.1.3/System.Text.Encoding.Extensions.dll",host-name="/app/corefx/artifacts/bin/testhost/netcoreapp-Linux-Debug-x64/shared/Microsoft.NETCore.App/3.1.3/System.Text.Encoding.Extensions.dll",symbols-loaded="0",base-address="0x7fee8756b000",size="5120"
=thread-created,id="1603"
=thread-created,id="1609"
=thread-created,id="1611"


## Dlls loaded:

System.Private.CoreLib.dll
test_debug.dll
System.Runtime.dll
System.Threading.Thread.dll
System.Console.dll
System.Runtime.Extensions.dll
System.Threading.dll
System.Diagnostics.Debug.dll
System.Text.Encoding.Extensions.dll
