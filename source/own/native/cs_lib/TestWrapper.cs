using System;

using System.Runtime.InteropServices;

namespace cs_lib
{
  public class TestWrapper
  {
    [DllImport("libtest.dylib", EntryPoint="DoNothing")]
    public static extern void DoNothing();

    [DllImport("libtest.dylib", EntryPoint="ReturnString", CharSet = CharSet.Ansi)]
    public static extern String ReturnString();
  }
}
