# gdb

gdb has the jit interface

https://sourceware.org/gdb/current/onlinedocs/gdb/JIT-Interface.html


there is also some source in dotnet/runtime dealing with gdbjit:

https://github.com/dotnet/runtime/blob/master/src/coreclr/src/vm/gdbjit.cpp


there are also a few other debugging engines:

MIEngine - Machine Interface Engine for Visual Studio:

https://github.com/microsoft/MIEngine

clrdbg - command-line debugger for dotnet core:

https://github.com/Microsoft/MIEngine/wiki/What-is-CLRDBG

Core interfaces (ICorDebug et al.):

https://docs.microsoft.com/en-us/visualstudio/extensibility/debugger/reference/core-interfaces?view=vs-2015&redirectedfrom=MSDN

overview of MI commands:

https://sourceware.org/gdb/onlinedocs/gdb/GDB_002fMI.html
