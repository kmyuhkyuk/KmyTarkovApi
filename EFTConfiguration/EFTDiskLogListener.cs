#if !UNITY_EDITOR

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using BepInEx;
using BepInEx.Logging;

namespace EFTConfiguration
{
    public class EFTDiskLogListener : ILogListener
    {
        private static readonly ManualLogSource LogSource = Logger.CreateLogSource("EFTDiskLogListener");

        private int _updateErrorCount;

        private int _memberAccessExceptionCount;

        private int _missingMemberExceptionCount;

        private int _methodAccessExceptionCount;

        private int _missingMethodExceptionCount;

        private int _missingFieldExceptionCount;

        private int _fieldAccessExceptionCount;

        private readonly int _maxErrorCount;

        private readonly TextWriter _logWriter;

        private readonly Timer _flushTimer;

        private readonly bool _instantFlushing;

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

        //Modify from BepInEx.Core.Logging.DiskLogListener
        public EFTDiskLogListener(string localPath,
            int maxErrorCount = 3,
            bool appendLog = false,
            bool delayedFlushing = true,
            int fileLimit = 5)
        {
            _maxErrorCount = maxErrorCount;

            var counter = 1;
            FileStream fileStream;
            while (!Utility.TryOpenFileStream(Path.Combine(Paths.BepInExRootPath, localPath),
                       appendLog ? FileMode.Append : FileMode.Create, out fileStream,
                       share: FileShare.Read, access: FileAccess.Write))
            {
                if (counter == fileLimit)
                {
                    LogSource.Log(LogLevel.Error, "Couldn't open a log file for writing. Skipping log file creation");

                    return;
                }

                LogSource.Log(LogLevel.Warning, $"Couldn't open log file '{localPath}' for writing, trying another...");

                localPath = $"{Path.GetFileNameWithoutExtension(localPath)}.{counter++}.{Path.GetExtension(localPath)}";
            }

            _logWriter = TextWriter.Synchronized(new StreamWriter(fileStream, Utility.UTF8NoBom));

            if (delayedFlushing)
            {
                _flushTimer = new Timer(o => _logWriter?.Flush(), null, 2000, 2000);
            }

            _instantFlushing = !delayedFlushing;
        }

        public void LogEvent(object sender, LogEventArgs eventArgs)
        {
            if (_logWriter == null)
                return;

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

            _logWriter.WriteLine(eventArgs);

            if (_instantFlushing)
            {
                _logWriter.Flush();
            }

            switch (error)
            {
                case ErrorType.Update when _updateErrorCount == _maxErrorCount:
                    LogSource.LogError(
                        "Major Error, This method loop throw error in Update (), Now hidden all Update () error, Please contact dev");
                    break;
                case ErrorType.MemberAccessException when _memberAccessExceptionCount == _maxErrorCount:
                case ErrorType.MissingMemberException when _missingMemberExceptionCount == _maxErrorCount:
                case ErrorType.MethodAccessException when _methodAccessExceptionCount == _maxErrorCount:
                case ErrorType.MissingMethodException when _missingMethodExceptionCount == _maxErrorCount:
                case ErrorType.MissingFieldException when _missingFieldExceptionCount == _maxErrorCount:
                case ErrorType.FieldAccessException when _fieldAccessExceptionCount == _maxErrorCount:
                    LogSource.LogError(
                        $"Major Error, Loop throw {error}, Now hidden all {error} error, Please contact dev");
                    break;
                case ErrorType.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(error), error, null);
            }
        }

        [SuppressMessage("ReSharper", "RedundantIfElseBlock")]
        private static ErrorType GetErrorType(LogEventArgs eventArgs)
        {
            var logArg = eventArgs.Data.ToString();

            if (string.IsNullOrEmpty(logArg))
                return ErrorType.None;

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

        private void ClearErrorCount(ref int errorCount)
        {
            if (errorCount < _maxErrorCount)
            {
                errorCount = 0;
            }
        }

        private bool NeedFilterLog(ref int errorCount)
        {
            if (errorCount == _maxErrorCount)
                return true;

            if (errorCount < _maxErrorCount)
            {
                errorCount++;
            }

            return false;
        }

        public void Dispose()
        {
            _flushTimer?.Dispose();

            try
            {
                _logWriter?.Flush();
                _logWriter?.Dispose();
            }
            catch (ObjectDisposedException)
            {
            }
        }

        ~EFTDiskLogListener()
        {
            Dispose();
        }
    }
}

#endif