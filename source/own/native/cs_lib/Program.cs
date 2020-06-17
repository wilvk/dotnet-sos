using System;
using System.Runtime.InteropServices;

namespace cs_lib
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            TestWrapper.DoNothing();

            IntPtr returnStringPointer = TestWrapper.ReturnString();
            string returnString = Marshal.PtrToStringAnsi(returnStringPointer);
            Console.WriteLine(returnString);
        }
    }
}
