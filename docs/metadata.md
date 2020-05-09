original source:

18    static long GetTicksElapsed(long lastTicks)
19    {
20      var currentTicks = DateTime.Now.Ticks;
21      var delta = currentTicks - lastTicks;
22      return delta;
23    }

in the pdb symbols:

2: #b5
{
  Locals: 0x11000002 (StandAloneSig)
  Document: #1

  IL_0000: (19, 5) - (19, 6)
  IL_0001: (20, 7) - (20, 45)
  IL_000F: (21, 7) - (21, 44)
  IL_0013: (22, 7) - (22, 20)
  IL_0017: (23, 5) - (23, 6)
}

in the dll metadata:


Method 'GetTicksElapsed' (#28) (0x06000002)
  Locals: int64, int64, typeref#d, int64 (#3a)
{
  // Code size       25 (0x19)
  .maxstack  2
  IL_0000:  nop
  IL_0001:  call       0x0A00000B
  IL_0006:  stloc.2
  IL_0007:  ldloca.s   V_2
  IL_0009:  call       0x0A00000C
  IL_000e:  stloc.0
  IL_000f:  ldloc.0
  IL_0010:  ldarg.0
  IL_0011:  sub
  IL_0012:  stloc.1
  IL_0013:  ldloc.1
  IL_0014:  stloc.3
  IL_0015:  br.s       IL_0017
  IL_0017:  ldloc.3
  IL_0018:  ret
}



these lines match to the IL code and also the source code.

for example.

IL_0000: (19, 5) - (19, 6)

starts at line 19, column 5 to line 19, column 6, (not including column 6, so just 1 character, '{')
in the document, this is:

```
19    {
```
      ^

b5 (SequencePoints): 02-00-00-01-13-05-01-00-26-02-04-0E-00-25-02-00-04-00-0D-02-00-04-00-01-02-7D

- #b5 is the method id in the pdb file - this is different in the dll metadata
- need a way to match between the two effectively

-------


in .dll:

Method (0x06, 0x1C):
method 2
GetTicksElapsed:
- method #28
- document #51
- RVA 0x0000209C

#28 -

Method 'GetTicksElapsed' (#28) (0x06000002)
  Locals: int64, int64, typeref#d, int64 (#3a)
{
  // Code size       25 (0x19)
  .maxstack  2
  IL_0000:  nop
  IL_0001:  call       0x0A00000B
  IL_0006:  stloc.2
  IL_0007:  ldloca.s   V_2
  IL_0009:  call       0x0A00000C
  IL_000e:  stloc.0
  IL_000f:  ldloc.0
  IL_0010:  ldarg.0
  IL_0011:  sub
  IL_0012:  stloc.1
  IL_0013:  ldloc.1
  IL_0014:  stloc.3
  IL_0015:  br.s       IL_0017
  IL_0017:  ldloc.3
  IL_0018:  ret
}

in pdb:
Document #51

MethodDebugInformation (index: 0x31, size: 12):
==================================================


1: #7f
{
  Locals: 0x11000001 (StandAloneSig)
  Document: #1

  IL_0000: (8, 5) - (8, 6)
  IL_0001: <hidden>
  IL_0003: (10, 7) - (10, 8)
  IL_0004: (11, 9) - (11, 44)
  IL_0012: (12, 9) - (12, 45)
  IL_001D: (13, 9) - (13, 55)
  IL_0024: (14, 9) - (14, 72)
  IL_003B: (15, 7) - (15, 8)
  IL_003C: (9, 7) - (9, 19)
}
2: #b5
{
  Locals: 0x11000002 (StandAloneSig)
  Document: #1

  IL_0000: (19, 5) - (19, 6)
  IL_0001: (20, 7) - (20, 45)
  IL_000F: (21, 7) - (21, 44)
  IL_0013: (22, 7) - (22, 20)
  IL_0017: (23, 5) - (23, 6)
}
3: nil

7f (SequencePoints): 01-00-00-01-08-05-01-00-00-02-00-01-04-04-01-00-23-02-04-0E-00-24-02-00-0B-00-2E-02-00-07-00-3F-02-00-17-00-01-02-7D-01-00-0C-75-00

b5 (SequencePoints): 02-00-00-01-13-05-01-00-26-02-04-0E-00-25-02-00-04-00-0D-02-00-04-00-01-02-7D

2: 0x06000001 (MethodDef)  0x35000003 (ImportScope)  0x33000001-0x33000002  nil        0x0003       57
3: 0x06000002 (MethodDef)  0x35000003 (ImportScope)  0x33000003-0x33000004  nil        0x0000       25



0x06000002 matches method between dll and pdb


look at: https://docs.microsoft.com/en-us/dotnet/api/system.reflection.metadata.sequencepoint?view=netcore-3.1
