using System;
using System.IO;
using System.Collections.Generic;
using NetcoreDbgTestCore;
using NetcoreDbgTestCore.VSCode;

using Newtonsoft.Json;

namespace NetcoreDbgTest.VSCode
{
    public class VSCodeResult : Tuple<bool, string>
    {
        public VSCodeResult(bool Success, string ResponseStr)
            :base(Success, ResponseStr)
        {
        }

        public bool Success { get{ return this.Item1; } }
        public string ResponseStr { get{ return this.Item2; } }
    }

    public class VSCodeDebugger
    {
        public bool isResponseContainProperty(string stringJSON, string testField, string testValue)
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(stringJSON));
            while (reader.Read()) {
                if (reader.Value != null
                    && reader.TokenType.ToString() == "PropertyName"
                    && reader.Value.ToString() == testField
                    && reader.Read()
                    && reader.Value != null
                    && reader.Value.ToString() == testValue) {
                    return true;
                }
            }

            return false;
        }

        public object GetResponsePropertyValue(string stringJSON, string testField)
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(stringJSON));
            while (reader.Read()) {
                if (reader.Value != null
                    && reader.TokenType.ToString() == "PropertyName"
                    && reader.Value.ToString() == testField
                    && reader.Read()) {
                    return reader.Value;
                }
            }

            return null;
        }
        public VSCodeResult Request(Request command, int timeout = -1)
        {
            string stringJSON = JsonConvert.SerializeObject(command,
                                                            Formatting.None,
                                                            new JsonSerializerSettings { 
                                                                NullValueHandling = NullValueHandling.Ignore});

            Logger.LogLine("-> (C) " + stringJSON);
            Int64 RequestSeq = (Int64)GetResponsePropertyValue(stringJSON, "seq");

            if (!Debuggee.DebuggerClient.Send(stringJSON)) {
                throw new DebuggerNotResponsesException();
            }

            while (true) {
                string[] response = Debuggee.DebuggerClient.Receive(timeout);
                if (response == null) {
                    throw new DebuggerNotResponsesException();
                }
                string line = response[0];

                if (isResponseContainProperty(line, "type", "response")
                    && (Int64)GetResponsePropertyValue(line, "request_seq") == RequestSeq) {
                    Logger.LogLine("<- (R) " + line);
                    return new VSCodeResult((bool)GetResponsePropertyValue(line, "success"), line);
                } else {
                    Logger.LogLine("<- (E) " + line);
                    EventQueue.Enqueue(line);
                }
            }
        }

        void ReceiveEvents(int timeout = -1)
        {
            while (true) {
                string[] response = Debuggee.DebuggerClient.Receive(timeout);
                if (response == null) {
                    throw new DebuggerNotResponsesException();
                }
                string line = response[0];

                Logger.LogLine("<- (E) " + line);
                EventQueue.Enqueue(line);

                foreach (var Event in StopEvents) {
                    if (isResponseContainProperty(line, "event", Event)) {
                        return;
                    }
                }
            }
        }

        public bool IsEventReceived(Func<string, bool> filter)
        {
            // check previously received events first
            while (EventQueue.Count > 0) {
                if (filter(EventQueue.Dequeue()))
                    return true;
            }

            // receive new events and check them
            ReceiveEvents();
            while (EventQueue.Count > 0) {
                if (filter(EventQueue.Dequeue()))
                    return true;
            }

            return false;
        }

        Queue<string> EventQueue = new Queue<string>();
        string[] StopEvents = {"stopped",
                               "terminated"};
    }
}
