using System;
using System.Runtime.InteropServices;
using Microsoft.Diagnostics.Runtime;

namespace com
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var debugger = new DbgEngDataReader(1, false, 1000);
        }
    }

    public class TestDll
    {
        [DllImport("/usr/local/share/dotnet/shared/Microsoft.NETCore.App/3.1.3/libdbgshim.dylib")]
        public static extern int DebugCreate(in Guid InterfaceId, out IntPtr Interface);

    }
}
