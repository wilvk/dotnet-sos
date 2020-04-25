using System;
using System.IO;
using System.Threading;

using NetcoreDbgTest;
using NetcoreDbgTest.MI;
using NetcoreDbgTest.Script;

using Xunit;

namespace NetcoreDbgTest.Script
{
    class Context
    {
        public static void Prepare()
        {
            Assert.Equal(MIResultClass.Done,
                         MIDebugger.Request("-file-exec-and-symbols "
                                            + DebuggeeInfo.CorerunPath).Class);

            Assert.Equal(MIResultClass.Done,
                         MIDebugger.Request("-exec-arguments "
                                            + DebuggeeInfo.TargetAssemblyPath).Class);

            Assert.Equal(MIResultClass.Running, MIDebugger.Request("-exec-run").Class);
        }

        static bool IsStoppedEvent(MIOutOfBandRecord record)
        {
            if (record.Type != MIOutOfBandRecordType.Async) {
                return false;
            }

            var asyncRecord = (MIAsyncRecord)record;

            if (asyncRecord.Class != MIAsyncRecordClass.Exec ||
                asyncRecord.Output.Class != MIAsyncOutputClass.Stopped) {
                return false;
            }

            return true;
        }

        public static void WasEntryPointHit()
        {
            Func<MIOutOfBandRecord, bool> filter = (record) => {
                if (!IsStoppedEvent(record)) {
                    return false;
                }

                var output = ((MIAsyncRecord)record).Output;
                var reason = (MIConst)output["reason"];

                if (reason.CString != "entry-point-hit") {
                    return false;
                }

                var frame = (MITuple)(output["frame"]);
                var func = (MIConst)(frame["func"]);
                if (func.CString == DebuggeeInfo.TestName + ".Program.Main()") {
                    return true;
                }

                return false;
            };

            if (!MIDebugger.IsEventReceived(filter))
                throw new NetcoreDbgTestCore.ResultNotSuccessException();
        }

        public static void DebuggerExit()
        {
            Assert.Equal(MIResultClass.Exit, Context.MIDebugger.Request("-gdb-exit").Class);
        }

        public static MIDebugger MIDebugger = new MIDebugger();
    }
}

namespace MITestExecInt
{
    class Program
    {
        static void Main(string[] args)
        {
            Label.Checkpoint("init", "", () => {
                Context.Prepare();
                Context.WasEntryPointHit();

                Assert.Equal(MIResultClass.Running, Context.MIDebugger.Request("1-exec-continue").Class);
                Assert.Equal(MIResultClass.Error, Context.MIDebugger.Request("2-exec-continue").Class);

                Assert.Equal(MIResultClass.Done, Context.MIDebugger.Request("3-exec-interrupt").Class);
                Assert.Equal(MIResultClass.Done, Context.MIDebugger.Request("4-exec-interrupt").Class);

                Assert.Equal(MIResultClass.Running, Context.MIDebugger.Request("5-exec-continue").Class);
                Assert.Equal(MIResultClass.Error, Context.MIDebugger.Request("6-exec-continue").Class);

                Assert.Equal(MIResultClass.Done, Context.MIDebugger.Request("7-exec-abort").Class);

                Context.DebuggerExit();
            });

            Thread.Sleep(10000);
            Console.WriteLine("Hello World!");
        }
    }
}
