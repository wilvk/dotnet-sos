(lldb) sos
-------------------------------------------------------------------------------
SOS is a debugger extension DLL designed to aid in the debugging of managed
programs. Functions are listed by category, then roughly in order of
importance. Shortcut names for popular functions are listed in parenthesis.
Type "soshelp <functionname>" for detailed info on that function.

Object Inspection                  Examining code and stacks
-----------------------------      -----------------------------
DumpObj (dumpobj)                  Threads (clrthreads)
DumpArray                          ThreadState
DumpAsync (dumpasync)              IP2MD (ip2md)
DumpDelegate (dumpdelegate)        u (clru)
DumpStackObjects (dso)             DumpStack (dumpstack)
DumpHeap (dumpheap)                EEStack (eestack)
DumpVC                             ClrStack (clrstack)
FinalizeQueue (finalizequeue)      GCInfo
GCRoot (gcroot)                    EHInfo
PrintException (pe)                bpmd (bpmd)

Examining CLR data structures      Diagnostic Utilities
-----------------------------      -----------------------------
DumpDomain (dumpdomain)            VerifyHeap
EEHeap (eeheap)                    FindAppDomain
Name2EE (name2ee)                  DumpLog (dumplog)
SyncBlk (syncblk)                  SuppressJitOptimization
DumpMT (dumpmt)
DumpClass (dumpclass)
DumpMD (dumpmd)
Token2EE
DumpModule (dumpmodule)
DumpAssembly (dumpassembly)
DumpRuntimeTypes
DumpIL (dumpil)
DumpSig
DumpSigElem

Examining the GC history           Other
-----------------------------      -----------------------------
HistInit (histinit)                SetHostRuntime (sethostruntime)
HistRoot (histroot)                SetSymbolServer (setsymbolserver, loadsymbols)
HistObj  (histobj)                 SetClrPath (setclrpath)
HistObjFind (histobjfind)          SOSFlush (sosflush)
HistClear (histclear)              SOSStatus (sosstatus)
                                   FAQ
                                   Help (soshelp)
