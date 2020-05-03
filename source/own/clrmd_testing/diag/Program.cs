using System;
using Microsoft.Diagnostics.Runtime;
using System.Linq;

namespace diag
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var crashDumpPath = "../../artefacts/core_20200503_003216";
            using (DataTarget dataTarget = DataTarget.LoadDump(crashDumpPath))
            {
                foreach (ClrInfo version in dataTarget.ClrVersions)
                {
                    Console.WriteLine("Found CLR Version: " + version.Version);

                    // This is the data needed to request the dac from the symbol server:
                    DacInfo dacInfo = version.DacInfo;
                    Console.WriteLine("Filesize:  {0:X}", dacInfo.IndexFileSize);
                    Console.WriteLine("Timestamp: {0:X}", dacInfo.IndexTimeStamp);
                    Console.WriteLine("Dac File:  {0}", dacInfo.PlatformSpecificFileName);

                    // If we just happen to have the correct dac file installed on the machine,
                    // the "LocalMatchingDac" property will return its location on disk:
                    string dacLocation = version.DacInfo.LocalDacPath; ;
                    if (!string.IsNullOrEmpty(dacLocation))
                        Console.WriteLine("Local dac location: " + dacLocation);

                    ClrInfo runtimeInfo = dataTarget.ClrVersions[0];  // just using the first runtime
                    ClrRuntime runtime = runtimeInfo.CreateRuntime();

                    // You may also download the dac from the symbol server, which is covered
                    // in a later section of this tutorial.
                    foreach (ClrAppDomain domain in runtime.AppDomains)
                    {
                        Console.WriteLine("ID:      {0}", domain.Id);
                        Console.WriteLine("Name:    {0}", domain.Name);
                        Console.WriteLine("Address: {0}", domain.Address);
                        foreach (ClrModule module in domain.Modules)
                        {
                            Console.WriteLine("Module: {0}", module.Name);
                        }
                        foreach (ClrThread thread in runtime.Threads)
                        {
                            if (!thread.IsAlive)
                                continue;

                            Console.WriteLine("Thread {0:X}:", thread.OSThreadId);

                            foreach (ClrStackFrame frame in thread.EnumerateStackTrace())
                            {
                                Console.WriteLine("Stack Name: " + frame.ToString());
                                Console.WriteLine("{0,12:X} {1,12:X} {2}", frame.StackPointer, frame.InstructionPointer, frame);
                            }

                            Console.WriteLine();

                            Console.WriteLine("{0,12} {1,12} {2,12} {3,12} {4,4} {5}", "Start", "End", "CommittedMemory", "ReservedMemory", "Heap", "Type");
                            foreach (ClrSegment segment in runtime.Heap.Segments)
                            {
                                string type;
                                if (segment.IsEphemeralSegment)
                                    type = "Ephemeral";
                                else if (segment.IsLargeObjectSegment)
                                    type = "Large";
                                else
                                    type = "Gen2";

                                Console.WriteLine("{0,12:X} {1,12:X} {2,12:X} {3,12:X} {4,4} {5}", segment.Start, segment.End, segment.CommittedMemory, segment.ReservedMemory, segment.LogicalHeap, type);
                            }

                            foreach (var item in (from seg in runtime.Heap.Segments
                                                  group seg by seg.LogicalHeap into g
                                                  orderby g.Key
                                                  select new
                                                  {
                                                      Heap = g.Key,
                                                      Size = g.Sum(p => (uint)p.Length)
                                                  }))
                            {
                                Console.WriteLine("Heap {0,2}: {1:n0} bytes", item.Heap, item.Size);
                            }



                        }

                        foreach (var handle in runtime.EnumerateHandles())
                        {
                            string objectType = runtime.Heap.GetObjectType(handle.Object).Name;
                            Console.WriteLine("{0,12:X} {1,12:X} {2,12} {3}", handle.Address, handle.Object, handle.GetType(), objectType);
                        }


                        if (!runtime.Heap.CanWalkHeap)
                        {
                            Console.WriteLine("Cannot walk the heap!");
                        }
                        else
                        {
                            foreach (ClrSegment seg in runtime.Heap.Segments)
                            {
                                for (ulong obj = seg.FirstObjectAddress; obj != 0; obj = seg.GetNextObjectAddress(obj))
                                {
                                    ClrType type = runtime.Heap.GetObjectType(obj);

                                    // If heap corruption, continue past this object.
                                    if (type == null)
                                        continue;

                                    int size = type.StaticSize;
                                    Console.WriteLine("{0,12:X} {1,8:n0} {2,1:n0} {3}", obj, size, seg.GetGeneration(obj), type.Name);
                                }
                            }
                        }
                        //  https://github.com/microsoft/clrmd/blob/master/doc/WalkingTheHeap.md#walking-objects-without-walking-the-segments
                        if (!runtime.Heap.CanWalkHeap)
                        {
                            Console.WriteLine("Cannot walk the heap!");
                        }
                        else
                        {
                            foreach (ulong obj in runtime.Heap.EnumerateObjects())
                            {
                                ClrType type = runtime.Heap.GetObjectType(obj);

                                // If heap corruption, continue past this object.
                                if (type == null)
                                    continue;

                                int size = type.StaticSize;
                                Console.WriteLine("{0,12:X} {1,8:n0} {2,1:n0} {3}", obj, size, type.GCDesc, type.Name);
                            }
                        }
                    }
                }
            }
        }
    }
}
