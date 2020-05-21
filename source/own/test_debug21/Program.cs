using System;

namespace console
{
  class Program
  {
    static void Main(string[] args)
    {
      while (true)
      {
        var lastTicks = DateTime.Now.Ticks;
        System.Threading.Thread.Sleep(2000);
        var ticksElapsed = GetTicksElapsed(lastTicks);
        Console.WriteLine("ticks elapsed: " + ticksElapsed.ToString());
      }
    }
 
    static long GetTicksElapsed(long lastTicks)
    {
      var currentTicks = DateTime.Now.Ticks;
      var delta = currentTicks - lastTicks;
      return delta;
    }
  }
}
