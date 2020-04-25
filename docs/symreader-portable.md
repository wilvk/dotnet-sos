# process for setting breakpoints and telling the ide where to highlight

setting a breakpoint:

ide - open file - select method - select a line in the method

line in the method will correspond to a method and offset in the pdb - this will refer to a method and sequence point in the cil - this will refer to an address in moemory to set a breakpoint on


calling back from lldb/gdb:

gdb will stop at a memory address from a breakpont being hit. the address needs to be mapped to an address in the cil that is mapped to a method and offset in the pdb and then mapped to a file name, method and line number in the ide


## Getting the filename from an offset

https://github.com/dotnet/symreader-portable/blob/f6ce54910bd3e3c0563fcd6d16d4b1c1123288ca/src/Microsoft.DiaSymReader.PortablePdb/SymMethod.cs#L528
