using System;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.DacInterface;
using Microsoft.Diagnostics.Runtime;

namespace debug
{
    class Program
    {
        static void Main(string[] args)
        {
            // needs to run in docker: AttachToProcess is not supported on Darwin 19.4.0
            int.TryParse(args[0], out var processId);
            Console.WriteLine("Attaching for PID: " + processId.ToString());
            var suspendOnStart = false;
            Console.WriteLine("Hello World!");
            DataTarget dataTarget = DataTarget.AttachToProcess(processId, suspendOnStart);

            if(dataTarget != null)
            {
              Console.WriteLine("Attached.");
              Console.WriteLine("Name: " + dataTarget.DataReader.DisplayName);
              Console.WriteLine("Arch: " + dataTarget.DataReader.Architecture.ToString());
              var modules = dataTarget.DataReader.EnumerateModules();
              foreach(var module in modules)
              {
                Console.WriteLine("file: " + module.FileName);
              }

              var clrVersions = dataTarget.ClrVersions;

              foreach(var version in clrVersions)
              {
                Console.WriteLine("dacPath: " + version.DacInfo.LocalDacPath);
              }


              // we have a live dac! (i think).
              var dac = new DacLibrary(dataTarget, clrVersions[0].DacInfo.LocalDacPath);

            }
            else
            {
              Console.WriteLine("Not attached.");
            }
        }
    }
}
