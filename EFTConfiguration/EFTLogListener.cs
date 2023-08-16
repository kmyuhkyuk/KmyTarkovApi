using System;
using System.Collections.Generic;
using BepInEx.Logging;

namespace EFTConfiguration
{
    public class EFTLogListener : ILogListener
    {
        public static IReadOnlyList<LogData> LogList => AllLog;

        public static event Action<LogData> OnLog;

        private static readonly List<LogData> AllLog = new List<LogData>();

        public void LogEvent(object sender, LogEventArgs eventArgs)
        {
            var log = new LogData(sender, eventArgs);

            AllLog.Add(log);

            OnLog?.Invoke(log);
        }

        public class LogData
        {
            public readonly object Sender;

            public readonly LogEventArgs EventArgs;

            public LogData(object sender, LogEventArgs eventArgs)
            {
                Sender = sender;
                EventArgs = eventArgs;
            }
        }

        public void Dispose()
        {
        }
    }
}