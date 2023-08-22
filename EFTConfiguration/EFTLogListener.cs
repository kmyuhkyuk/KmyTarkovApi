using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;

namespace EFTConfiguration
{
    public class EFTLogListener : ILogListener
    {
        public static IReadOnlyList<LogData> LogList => AllLog;

        public static event Action<LogData> OnLog;

        private static readonly List<LogData> AllLog = new List<LogData>();

        private static readonly HashSet<string> HideLog = new HashSet<string>();

        private static readonly ManualLogSource LogSource = Logger.CreateLogSource("EFTLogListener");

        public void LogEvent(object sender, LogEventArgs eventArgs)
        {
            var logArg = (string)eventArgs.Data;

            if (HideLog.Contains(logArg))
                return;

            if (logArg.Contains(".Update ()") && (string)AllLog.Last().EventArgs.Data == logArg)
            {
                LogSource.LogError("Major Error, This method loop throw error in Update (), Now added to hidden list, Please contact dev");

                HideLog.Add(logArg);
            }
            else
            {
                var log = new LogData(sender, eventArgs);

                AllLog.Add(log);

                OnLog?.Invoke(log);
            }
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