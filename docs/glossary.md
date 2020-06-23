SOS - Son of Strike. A debugger extension that uses the DAC (Data Access Component) to hook into and reveal internal CLR data structures.
EE - Execution Engine. Appears in the names of parts of the CLR that were adapted specifically for running .NET code. As you found out. Also referred to as the Execution Environment.
pdb - program database
dia - debug interface access
llvm - low level virtual machine
lldb - low level debugger
gdb - gnu debugger
msil - microsoft intermediate language
cil - common intermediate language
clr - common language runtime - what dotnet assemblies run in
jit - just in time compiler used in the clr
ngen - native image generator - ahead of time compiler for producing native binary images for the current environment (can be quicker than JIT-ed code)
cts - common type system
cls - common language specification
cli - common language infrastructure
ves - virtual execution system
dif - debug interchange format
BNF - Backus-Naur form -

ip - instruction pointer
md - method descriptor
mt - method table
sequence point - used in pdb file for debugging-
EEClass - a cli class in sos
DAC - data access component - https://github.com/dotnet/runtime/blob/master/docs/design/coreclr/botr/dac-notes.md - The DAC is conceptually a subset of the runtime's execution engine code that runs out-of-process. (mscordacwks.dll, msdaccore.dll)
EnC - Edit and Continue - https://github.com/dotnet/roslyn/wiki/EnC-Supported-Edits
gnujit - GNU JII interface - now maintained by LLVM
metadata -
dbi - debugger interface
bc - base class liibrary
ICorDebug - CLR debugging api - https://docs.microsoft.com/en-us/dotnet/framework/unmanaged-api/debugging/
BOTR - Book of the Runtime - https://github.com/dotnet/coreclr/tree/master/Documentation/botr
mdbg - managed debugger
CSE - Common Subexpression Elimination
PAL - Platform Abstraction Layer
IR - Internal Representation - https://github.com/dotnet/coreclr/blob/master/Documentation/botr/ryujit-overview.md
HIR - High-level IR
LIR - Low-level IR
ABI - Application Binary Interface
clang, g++ - c/c++ compilers - https://medium.com/@alitech_2017/gcc-vs-clang-llvm-an-in-depth-comparison-of-c-c-compilers-899ede2be378


internal:
Telesto - a clr module/process?
retail - shipped version?
TLS - Thread Local Storage
Crst - critical section (usually with threads)
