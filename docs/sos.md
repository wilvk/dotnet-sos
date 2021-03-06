## ip2md:

IP2MD <Code address>

Given an address in managed JITTED code, IP2MD attempts to find the MethodDesc
associated with it. For example, this output from K:

    (lldb) bt
        ...
        frame #9: 0x00007fffffffbf60 0x00007ffff61c6d89 libcoreclr.so`MethodDesc::DoPrestub(this=0x00007ffff041f870, pDispatchingMT=0x0000000000000000) + 3001 at prestub.cpp:1490
        frame #10: 0x00007fffffffc140 0x00007ffff61c5f17 libcoreclr.so`::PreStubWorker(pTransitionBlock=0x00007fffffffc9a8, pMD=0x00007ffff041f870) + 1399 at prestub.cpp:1037
        frame #11: 0x00007fffffffc920 0x00007ffff5f5238c libcoreclr.so`ThePreStub + 92 at theprestubamd64.S:800
        frame #12: 0x00007fffffffca10 0x00007ffff04981cc
        frame #13: 0x00007fffffffca30 0x00007ffff049773c
        frame #14: 0x00007fffffffca80 0x00007ffff04975ad
        ...
        frame #22: 0x00007fffffffcc90 0x00007ffff5f51a0f libcoreclr.so`CallDescrWorkerInternal + 124 at calldescrworkeramd64.S:863
        frame #23: 0x00007fffffffccb0 0x00007ffff5d6d6dc libcoreclr.so`CallDescrWorkerWithHandler(pCallDescrData=0x00007fffffffce80, fCriticalCall=0) + 476 at callhelpers.cpp:88
        frame #24: 0x00007fffffffcd00 0x00007ffff5d6eb38 libcoreclr.so`MethodDescCallSite::CallTargetWorker(this=0x00007fffffffd0c8, pArguments=0x00007fffffffd048) + 2504 at callhelpers.cpp:633

    (lldb) ip2md 0x00007ffff049773c
        MethodDesc:   00007ffff7f71920
        Method Name:  Microsoft.Win32.SafeHandles.SafeFileHandle.Open(System.Func`1<Int32>)
        Class:        00007ffff0494bf8
        MethodTable:  00007ffff7f71a58
        mdToken:      0000000006000008
        Module:       00007ffff7f6b938
        IsJitted:     yes
        CodeAddr:     00007ffff04976c0

We have taken a return address into Mainy.Main, and discovered information
about that method. You could run U, DumpMT, DumpClass, DumpMD, or
DumpModule on the fields listed to learn more.

The "Source line" output will only be present if the debugger can find the
symbols for the managed module containing the given <code address>, and if the
debugger is configured to load line number information.

--
method descriptor

## clrstack:

(lldb) soshelp clrstack
-------------------------------------------------------------------------------
ClrStack [-a] [-l] [-p] [-n] [-f] [-r] [-all]
ClrStack [-a] [-l] [-p] [-i] [variable name] [frame]

ClrStack attempts to provide a true stack trace for managed code only. It is
handy for clean, simple traces when debugging straightforward managed
programs. The -p parameter will show arguments to the managed function. The
-l parameter can be used to show information on local variables in a frame.
SOS can't retrieve local names at this time, so the output for locals is in
the format <local address> = <value>. The -a (all) parameter is a short-cut
for -l and -p combined.

The -f option (full mode) displays the native frames intermixing them with
the managed frames and the assembly name and function offset for the managed
frames.

The -r option dumps the registers for each stack frame.

The -all option dumps all the managed threads' stacks.

If the debugger has the option SYMOPT_LOAD_LINES specified (either by the
.lines or .symopt commands), SOS will look up the symbols for every managed
frame and if successful will display the corresponding source file name and
line number. The -n (No line numbers) parameter can be specified to disable
this behavior.

When you see methods with the name "[Frame:...", that indicates a transition
between managed and unmanaged code. You could run IP2MD on the return
addresses in the call stack to get more information on each managed method.

On x64 platforms, Transition Frames are not displayed at this time. To avoid
heavy optimization of parameters and locals one can request the JIT compiler
to not optimize functions in the managed app by creating a file myapp.ini
(if your program is myapp.exe) in the same directory. Put the following lines
in myapp.ini and re-run:

[.NET Framework Debugging Control]
GenerateTrackingInfo=1
AllowOptimize=0

The -i option is a new EXPERIMENTAL addition to ClrStack and will use the ICorDebug
interfaces to display the managed stack and variables. With this option you can also
view and expand arrays and fields for managed variables. If a stack frame number is
specified in the command line, ClrStack will show you the parameters and/or locals
only for that frame (provided you specify -l or -p or -a of course). If a variable
name and a stack frame number are specified in the command line, ClrStack will show
you the parameters and/or locals for that frame, and will also show you the fields
for that variable name you specified. Here are some examples:
   clrstack -i -a           : This will show you all parameters and locals for all frames
   clrstack -i -a 3         : This will show you all parameters and locals, for frame 3
   clrstack -i var1 0       : This will show you the fields of 'var1' for frame 0
   clrstack -i var1.abc 2   : This will show you the fields of 'var1', and expand
                              'var1.abc' to show you the fields of the 'abc' field,
                              for frame 2.
   clrstack -i var1.[basetype] 0   : This will show you the fields of 'var1', and
                                     expand the base type of 'var1' to show you its
                                     fields.
   clrstack -i var1.[6] 0   : If 'var1' is an array, this will show you the element
                              at index 6 in the array, along with its fields
The -i options uses DML output for a better debugging experience, so typically you
should only need to execute "clrstack -i", and from there, click on the DML
hyperlinks to inspect the different managed stack frames and managed variables.

## dso:

(lldb) soshelp dso
-------------------------------------------------------------------------------
COMMAND: dumpstackobjects.
DumpStackObjects [-verify] [top stack [bottom stack]]

This command will display any managed objects it finds within the bounds of
the current stack. Combined with the stack tracing commands like K and
ClrStack, it is a good aid to determining the values of locals and
parameters.

If you use the -verify option, each non-static CLASS field of an object
candidate is validated. This helps to eliminate false positives. It is not
on by default because very often in a debugging scenario, you are
interested in objects with invalid fields.

The abbreviation dso can be used for brevity.

## dumpheap:

(lldb) soshelp dumpheap
-------------------------------------------------------------------------------
DumpHeap [-stat]
         [-strings]
         [-short]
         [-min <size>]
         [-max <size>]
         [-live]
         [-dead]
         [-thinlock]
         [-startAtLowerBound]
         [-mt <MethodTable address>]
         [-type <partial type name>]
         [start [end]]

DumpHeap is a powerful command that traverses the garbage collected heap,
collection statistics about objects. With it's various options, it can look for
particular types, restrict to a range, or look for ThinLocks (see syncblk
documentation). Finally, it will provide a warning if it detects excessive
fragmentation in the GC heap.

When called without options, the output is first a list of objects in the heap,
followed by a report listing all the types found, their size and number:

    (lldb) dumpheap
     Address       MT     Size
    00a71000 0015cde8       12 Free
    00a7100c 0015cde8       12 Free
    00a71018 0015cde8       12 Free
    00a71024 5ba58328       68
    00a71068 5ba58380       68
    00a710ac 5ba58430       68
    00a710f0 5ba5dba4       68
    ...
    total 619 objects
    Statistics:
          MT    Count TotalSize Class Name
    5ba7607c        1        12 System.Security.Permissions.HostProtectionResource
    5ba75d54        1        12 System.Security.Permissions.SecurityPermissionFlag
    5ba61f18        1        12 System.Collections.CaseInsensitiveComparer
    ...
    0015cde8        6     10260      Free
    5ba57bf8      318     18136 System.String
    ...

"Free" objects are simply regions of space the garbage collector can use later.
If 30% or more of the heap contains "Free" objects, the process may suffer from
heap fragmentation. This is usually caused by pinning objects for a long time
combined with a high rate of allocation. Here is example output where DumpHeap
provides a warning about fragmentation:

    <After the Statistics section>
    Fragmented blocks larger than 1MB:
        Addr     Size Followed by
    00a780c0    1.5MB    00bec800 System.Byte[]
    00da4e38    1.2MB    00ed2c00 System.Byte[]
    00f16df0    1.2MB    01044338 System.Byte[]

The arguments in detail:

-stat     Restrict the output to the statistical type summary
-strings  Restrict the output to a statistical string value summary
-short    Limits output to just the address of each object. This allows you
          to easily pipe output from the command to another debugger
          command for automation.
-min      Ignore objects less than the size given in bytes
-max      Ignore objects larger than the size given in bytes
-live     Only print live objects
-dead     Only print dead objects (objects which will be collected in the
          next full GC)
-thinlock Report on any ThinLocks (an efficient locking scheme, see syncblk
          documentation for more info)
-startAtLowerBound
          Force heap walk to begin at lower bound of a supplied address range.
          (During plan phase, the heap is often not walkable because objects
          are being moved. In this case, DumpHeap may report spurious errors,
          in particular bad objects. It may be possible to traverse more of
          the heap after the reported bad object. Even if you specify an
          address range, DumpHeap will start its walk from the beginning of
          the heap by default. If it finds a bad object before the specified
          range, it will stop before displaying the part of the heap in which
          you are interested. This switch will force DumpHeap to begin its
          walk at the specified lower bound. You must supply the address of a
          good object as the lower bound for this to work. Display memory at
          the address of the bad object to manually find the next method
          table (use DumpMT to verify). If the GC is currently in a call to
          memcopy, You may also be able to find the next object's address by
          adding the size to the start address given as parameters.)
-mt       List only those objects with the MethodTable given
-type     List only those objects whose type name is a substring match of the
          string provided.
start     Begin listing from this address
end       Stop listing at this address

A special note about -type: Often, you'd like to find not only Strings, but
System.Object arrays that are constrained to contain Strings. ("new
String[100]" actually creates a System.Object array, but it can only hold
System.String object pointers). You can use -type in a special way to find
these arrays. Just pass "-type System.String[]" and those Object arrays will
be returned. More generally, "-type <Substring of interesting type>[]".

The start/end parameters can be obtained from the output of eeheap -gc. For
example, if you only want to list objects in the large heap segment:

    (lldb) eeheap -gc
    Number of GC Heaps: 1
    generation 0 starts at 0x00c32754
    generation 1 starts at 0x00c32748
    generation 2 starts at 0x00a71000
     segment    begin allocated     size
    00a70000 00a71000  010443a8 005d33a8(6108072)
    Large object heap starts at 0x01a71000
     segment    begin allocated     size
    01a70000 01a71000  01a75000 0x00004000(16384)
    Total Size  0x5d73a8(6124456)
    ------------------------------
    GC Heap Size  0x5d73a8(6124456)

    (lldb) dumpheap 1a71000 1a75000
     Address       MT     Size
    01a71000 5ba88bd8     2064
    01a71810 0019fe48     2032 Free
    01a72000 5ba88bd8     4096
    01a73000 0019fe48     4096 Free
    01a74000 5ba88bd8     4096
    total 5 objects
    Statistics:
          MT    Count TotalSize Class Name
    0019fe48        2      6128      Free
    5ba88bd8        3     10256 System.Object[]
    Total 5 objects

Finally, if GC heap corruption is present, you may see an error like this:

    (lldb) dumpheap -stat
    object 00a73d24: does not have valid MT
    curr_object : 00a73d24
    Last good object: 00a73d14
    ----------------

That indicates a serious problem. See the help for VerifyHeap for more
information on diagnosing the cause.
(lldb)

## dumpmt:

(lldb) soshelp dumpmt
-------------------------------------------------------------------------------
DumpMT [-MD] <MethodTable address>

Examine a MethodTable. Each managed object has a MethodTable pointer at the
start. If you pass the "-MD" flag, you'll also see a list of all the methods
defined on the object.

## dumpclass:

(lldb) soshelp dumpclass
-------------------------------------------------------------------------------
DumpClass <EEClass address>

The EEClass is a data structure associated with an object type. DumpClass
will show attributes, as well as list the fields of the type. The output is
similar to DumpObj. Although static field values will be displayed,
non-static values won't because you need an instance of an object for that.

You can get an EEClass to look at from DumpMT, DumpObj, Name2EE, and
Token2EE among others.

## dumpmodule:

(lldb) soshelp dumpmodule
-------------------------------------------------------------------------------
DumpModule [-mt] <Module address>

You can get a Module address from DumpDomain, DumpAssembly and other
functions. Here is sample output:

    (lldb) sos DumpModule 1caa50
    Name: /home/user/pub/unittest
    Attributes: PEFile
    Assembly: 001ca248
    LoaderHeap: 001cab3c
    TypeDefToMethodTableMap: 03ec0010
    TypeRefToMethodTableMap: 03ec0024
    MethodDefToDescMap: 03ec0064
    FieldDefToDescMap: 03ec00a4
    MemberRefToDescMap: 03ec00e8
    FileReferencesMap: 03ec0128
    AssemblyReferencesMap: 03ec012c
    MetaData start address: 00402230 (1888 bytes)

The Maps listed map metadata tokens to CLR data structures. Without going into
too much detail, you can examine memory at those addresses to find the
appropriate structures. For example, the TypeDefToMethodTableMap above can be
examined:

    (lldb) dd 3ec0010
    03ec0010  00000000 00000000 0090320c 0090375c
    03ec0020  009038ec ...

This means TypeDef token 2 maps to a MethodTable with the value 0090320c. You
can run DumpMT to verify that. The MethodDefToDescMap takes a MethodDef token
and maps it to a MethodDesc, which can be passed to dumpmd.

There is a new option "-mt", which will display the types defined in a module,
and the types referenced by the module. For example:

    (lldb) sos DumpModule -mt 1aa580
    Name: /home/user/pub/unittest
    ...<etc>...
    MetaData start address: 0040220c (1696 bytes)

    Types defined in this module

          MT    TypeDef Name
    --------------------------------------------------------------------------
    030d115c 0x02000002 Funny
    030d1228 0x02000003 Mainy

    Types referenced in this module

          MT    TypeRef Name
    --------------------------------------------------------------------------
    030b6420 0x01000001 System.ValueType
    030b5cb0 0x01000002 System.Object
    030fceb4 0x01000003 System.Exception
    0334e374 0x0100000c System.Console
    03167a50 0x0100000e System.Runtime.InteropServices.GCHandle
    0336a048 0x0100000f System.GC

## dumpassembly:

(lldb) soshelp dumpassembly
-------------------------------------------------------------------------------
DumpAssembly <Assembly address>

Example output:

    (lldb) sos DumpAssembly 1ca248
    Parent Domain: 0014f000
    Name: /home/user/pub/unittest
    ClassLoader: 001ca060
      Module Name
    001caa50 /home/user/pub/unittest

An assembly can consist of multiple modules, and those will be listed. You can
get an Assembly address from the output of DumpDomain.

## eestack:

(lldb) soshelp eestack
-------------------------------------------------------------------------------
EEStack [-short] [-EE]

This command runs DumpStack on all threads in the process. The -EE option is
passed directly to DumpStack. The -short option tries to narrow down the
output to "interesting" threads only, which is defined by

1) The thread has taken a lock.
2) The thread has been "hijacked" in order to allow a garbage collection.
3) The thread is currently in managed code.

See the documentation for DumpStack for more info.

## bpmd:

(lldb) soshelp bpmd
-------------------------------------------------------------------------------
bpmd [-nofuturemodule] <module name> <method name> [<il offset>]
bpmd <source file name>:<line number>
bpmd -md <MethodDesc>
bpmd -list
bpmd -clear <pending breakpoint number>
bpmd -clearall

bpmd provides managed breakpoint support. If it can resolve the method name
to a loaded, jitted or ngen'd function it will create a breakpoint with "bp".
If not then either the module that contains the method hasn't been loaded yet
or the module is loaded, but the function is not jitted yet. In these cases,
bpmd asks the Debugger to receive CLR Notifications, and waits to receive news
of module loads and JITs, at which time it will try to resolve the function to
a breakpoint. -nofuturemodule can be used to suppress creating a breakpoint
against a module that has not yet been loaded.

Management of the list of pending breakpoints can be done via bpmd -list,
bpmd -clear, and bpmd -clearall commands. bpmd -list generates a list of
all of the pending breakpoints. If the pending breakpoint has a non-zero
module id, then that pending breakpoint is specific to function in that
particular loaded module. If the pending breakpoint has a zero module id, then
the breakpoint applies to modules that have not yet been loaded. Use
bpmd -clear or bpmd -clearall to remove pending breakpoints from the list.

The bpmd command can now be used before the runtime is loaded. You can execute
bpmd right after the SOS plug-in is loaded. Always add the module extension for
the module name parameter.

This brings up a good question: "I want to set a breakpoint on the main
method of my application. How can I do this?"

  1) Add the breakpoint with command such as:
       bpmd myapp.dll MyApp.Main
  2) g
  3) You will stop at the start of MyApp.Main. If you type "bl" you will
     see the breakpoint listed.

To correctly specify explicitly implemented methods make sure to retrieve the
method name from the metadata, or from the output of the "dumpmt -md" command.
For example:

    public interface I1
    {
        void M1();
    }
    public class ExplicitItfImpl : I1
    {
        ...
        void I1.M1()        // this method's name is 'I1.M1'
        { ... }
    }

    bpmd myapp.dll ExplicitItfImpl.I1.M1


bpmd works equally well with generic types. Adding a breakpoint on a generic
type sets breakpoints on all already JIT-ted generic methods and sets a pending
breakpoint for any instantiation that will be JIT-ted in the future.

Example for generics:
    Given the following two classes:

    class G3<T1, T2, T3>
    {
        ...
        public void F(T1 p1, T2 p2, T3 p3)
        { ... }
    }

    public class G1<T> {
        // static method
        static public void G<W>(W w)
        { ... }
    }

    One would issue the following commands to set breakpoints on G3.F() and
    G1.G():

    bpmd myapp.dll G3`3.F
    bpmd myapp.dll G1`1.G

And for explicitly implemented methods on generic interfaces:
    public interface IT1<T>
    {
        void M1(T t);
    }

    public class ExplicitItfImpl<U> : IT1<U>
    {
        ...
        void IT1<U>.M1(U u)    // this method's name is 'IT1<U>.M1'
        { ... }
    }

    bpmd bpmd.dll ExplicitItfImpl`1.IT1<U>.M1

Additional examples:
    If IT1 and ExplicitItfImpl are types declared inside another class,
    Outer, the bpmd command would become:

    bpmd bpmd.exe Outer+ExplicitItfImpl`1.Outer.IT1<U>.M1

    (note that the fully qualified type name for ExplicitItfImpl became
    Outer+ExplicitItfImpl, using the '+' separator, while the method name
    is Outer.IT1<U>.M1, using a '.' as the separator)

    Furthermore, if the Outer class resides in a namespace, NS, the bpmd
    command to use becomes:

    bpmd bpmd.dll NS.Outer+ExplicitItfImpl`1.NS.Outer.IT1<U>.M1

bpmd does not accept offsets nor parameters in the method name. You can add
an IL offset as an optional parameter separate from the name. If there are overloaded
methods, bpmd will set a breakpoint for all of them.

In the case of hosted environments such as SQL, the module name may be
complex, like 'price, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'.
For this case, just be sure to surround the module name with single quotes,
like:

bpmd 'price, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null' Price.M2

(lldb)
