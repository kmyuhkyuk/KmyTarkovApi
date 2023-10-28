#if !UNITY_EDITOR

using System;
using System.Collections.Generic;
using BepInEx.Logging;
using EFTConfiguration.Models;

namespace EFTConfiguration
{
    public class EFTLogListener : ILogListener
    {
        public static IReadOnlyList<LogModel> LogList => AllLog;

        public static event Action<LogModel> OnLog;

        private static readonly List<LogModel> AllLog = new List<LogModel>();

        private static readonly ManualLogSource LogSource = Logger.CreateLogSource("EFTLogListener");

        private static int _updateErrorCount;

        private static int _memberAccessExceptionCount;

        private static int _missingMemberExceptionCount;

        private static int _methodAccessExceptionCount;

        private static int _missingMethodExceptionCount;

        private static int _missingFieldExceptionCount;

        private static int _fieldAccessExceptionCount;

        private const int MaxErrorCount = 3;

        private enum ErrorType
        {
            None,
            Update,
            MemberAccessException,
            MissingMemberException,
            MethodAccessException,
            MissingMethodException,
            MissingFieldException,
            FieldAccessException
        }

        public void LogEvent(object sender, LogEventArgs eventArgs)
        {
            var error = GetErrorType(eventArgs);

            switch (error)
            {
                case ErrorType.None:
                    ClearErrorCount(ref _updateErrorCount);
                    ClearErrorCount(ref _memberAccessExceptionCount);
                    ClearErrorCount(ref _missingMemberExceptionCount);
                    ClearErrorCount(ref _methodAccessExceptionCount);
                    ClearErrorCount(ref _missingMethodExceptionCount);
                    ClearErrorCount(ref _missingFieldExceptionCount);
                    ClearErrorCount(ref _fieldAccessExceptionCount);
                    break;
                case ErrorType.Update:
                    if (NeedFilterLog(ref _updateErrorCount))
                        return;
                    break;
                case ErrorType.MemberAccessException:
                    if (NeedFilterLog(ref _memberAccessExceptionCount))
                        return;
                    break;
                case ErrorType.MissingMemberException:
                    if (NeedFilterLog(ref _missingMemberExceptionCount))
                        return;
                    break;
                case ErrorType.MethodAccessException:
                    if (NeedFilterLog(ref _methodAccessExceptionCount))
                        return;
                    break;
                case ErrorType.MissingMethodException:
                    if (NeedFilterLog(ref _missingMethodExceptionCount))
                        return;
                    break;
                case ErrorType.MissingFieldException:
                    if (NeedFilterLog(ref _missingFieldExceptionCount))
                        return;
                    break;
                case ErrorType.FieldAccessException:
                    if (NeedFilterLog(ref _fieldAccessExceptionCount))
                        return;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(error), error, null);
            }

            var log = new LogModel(sender, eventArgs);

            AllLog.Add(log);

            OnLog?.Invoke(log);

            switch (error)
            {
                case ErrorType.Update when _updateErrorCount == MaxErrorCount:
                    LogSource.LogError(
                        "Major Error, This method loop throw error in Update (), Now hidden all Update () error, Please contact dev");
                    break;
                case ErrorType.MemberAccessException when _memberAccessExceptionCount == MaxErrorCount:
                case ErrorType.MissingMemberException when _missingMemberExceptionCount == MaxErrorCount:
                case ErrorType.MethodAccessException when _methodAccessExceptionCount == MaxErrorCount:
                case ErrorType.MissingMethodException when _missingMethodExceptionCount == MaxErrorCount:
                case ErrorType.MissingFieldException when _missingFieldExceptionCount == MaxErrorCount:
                case ErrorType.FieldAccessException when _fieldAccessExceptionCount == MaxErrorCount:
                    LogSource.LogError(
                        $"Major Error, Loop throw {error}, Now hidden all {error} error, Please contact dev");
                    break;
                case ErrorType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(error), error, null);
            }
        }

        private static ErrorType GetErrorType(LogEventArgs eventArgs)
        {
            var logArg = eventArgs.Data.ToString();

            if (logArg.Contains(".Update ()"))
            {
                return ErrorType.Update;
            }
            else if (logArg.StartsWith(nameof(MemberAccessException)))
            {
                return ErrorType.MemberAccessException;
            }
            else if (logArg.StartsWith(nameof(MissingMemberException)))
            {
                return ErrorType.MissingMemberException;
            }
            else if (logArg.StartsWith(nameof(MethodAccessException)))
            {
                return ErrorType.MethodAccessException;
            }
            else if (logArg.StartsWith(nameof(MissingMethodException)))
            {
                return ErrorType.MissingMethodException;
            }
            else if (logArg.StartsWith(nameof(MissingFieldException)))
            {
                return ErrorType.MissingFieldException;
            }
            else if (logArg.StartsWith(nameof(FieldAccessException)))
            {
                return ErrorType.FieldAccessException;
            }
            else
            {
                return ErrorType.None;
            }
        }

        private static void ClearErrorCount(ref int errorCount)
        {
            if (errorCount < MaxErrorCount)
            {
                errorCount = 0;
            }
        }

        private static bool NeedFilterLog(ref int errorCount)
        {
            if (errorCount == MaxErrorCount)
            {
                return true;
            }
            else if (errorCount < MaxErrorCount)
            {
                errorCount++;
            }

            return false;
        }

        public void Dispose()
        {
        }
    }
}

#endif