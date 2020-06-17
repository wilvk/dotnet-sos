using System;

namespace cs_lib
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            TestWrapper.DoNothing();

            unsafe
            {
              String returnStringString = TestWrapper.ReturnString();
              Console.WriteLine(returnStringString);
            }

        }
    }
}
