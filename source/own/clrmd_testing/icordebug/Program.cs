using System;

namespace icordebug
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("ProcId: " + args[0]);
            var processId = int.Parse(args[0]);
            var minVersion = "0";
            var debugger = new CLRDebugging();
            var result = CLRDebugging.GetDebuggerForProcess(processId, minVersion);

        }
    }
}
