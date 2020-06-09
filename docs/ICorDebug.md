# https://lowleveldesign.org/2010/10/11/writing-a-net-debugger-part-1-starting-the-debugging-session/

ICorDebug is what VSCode uses to interrogate a process for it's debugging state.

mscordacwks.{dll|so} is the library that contains the ICorDebug interface.

clrmd has some access to ICorDebug - under ./src/Microsoft.Diagnostics.Runtime/ICorDebug/
