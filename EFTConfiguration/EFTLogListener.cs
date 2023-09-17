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

        private static readonly ManualLogSource LogSource = Logger.CreateLogSource("EFTLogListener");

        private static int _updateErrorCount;

        private const int MaxUpdateErrorCount = 3;

        public void LogEvent(object sender, LogEventArgs eventArgs)
        {
            var logArg = eventArgs.Data.ToString();

            var isUpdateError = logArg.Contains(".Update ()");

            switch (isUpdateError)
            {
                case true when _updateErrorCount == MaxUpdateErrorCount:
                    return;
                case true when _updateErrorCount < MaxUpdateErrorCount:
                    _updateErrorCount++;
                    break;
                case false when _updateErrorCount < MaxUpdateErrorCount:
                    _updateErrorCount = 0;
                    break;
            }

            var log = new LogData(sender, eventArgs);

            AllLog.Add(log);

            OnLog?.Invoke(log);

            if (isUpdateError && _updateErrorCount == MaxUpdateErrorCount)
            {
                LogSource.LogError(
                    "Major Error, This method loop throw error in Update (), Now hidden all Update () error, Please contact dev");
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