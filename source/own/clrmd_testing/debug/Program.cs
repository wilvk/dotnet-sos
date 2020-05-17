using System;
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
            }
            else
            {
              Console.WriteLine("Not attached.");
            }
        }
    }
}
