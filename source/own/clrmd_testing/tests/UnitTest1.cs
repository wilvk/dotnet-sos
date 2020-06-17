using System;
using System.Reflection;
using Xunit;

namespace tests
{
    public class UnitTest1
    {
      [TestMethod]
      public void ComAccessReflectionCoreAnd45Test()
      {
          // this works with both .NET 4.5+ and .NET Core 2.0+

          string progId = "InternetExplorer.Application";
          Type type = Type.GetTypeFromProgID(progId);
          object inst = Activator.CreateInstance(type);


          inst.GetType().InvokeMember("Visible", ReflectionUtils.MemberAccess | BindingFlags.SetProperty, null, inst,
              new object[1]
              {
                  true
              });

          inst.GetType().InvokeMember("Navigate", ReflectionUtils.MemberAccess | BindingFlags.InvokeMethod, null,
              inst, new object[]
              {
                  "https://markdownmonster.west-wind.com",
              });

          //result = ReflectionUtils.GetPropertyCom(inst, "cAppStartPath");
          bool result = (bool)inst.GetType().InvokeMember("Visible",
              ReflectionUtils.MemberAccess | BindingFlags.GetProperty, null, inst, null);
          Console.WriteLine(result); // path
      }
    }
}
