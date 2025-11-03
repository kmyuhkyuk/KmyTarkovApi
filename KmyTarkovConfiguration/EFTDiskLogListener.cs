#if !UNITY_EDITOR

using System.IO;
using System.Threading;
using BepInEx;
using BepInEx.Logging;

namespace KmyTarkovConfiguration
{
    public class EFTDiskLogListener : ILogListener
    {
        private static readonly ManualLogSource Logger =
            BepInEx.Logging.Logger.CreateLogSource(nameof(EFTDiskLogListener));

        private readonly TextWriter _logWriter;

        private readonly Timer _flushTimer;

        //Modify from BepInEx.Core.Logging.DiskLogListener
        public EFTDiskLogListener(string localPath, bool appendLog = false)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(localPath);

            var extension = Path.GetExtension(localPath);

            var counter = 1;

            FileStream fileStream;

            while (!Utility.TryOpenFileStream(Path.Combine(BepInEx.Paths.BepInExRootPath, localPath),
                       appendLog ? FileMode.Append : FileMode.Create, out fileStream, share: FileShare.Read,
                       access: FileAccess.Write))
            {
                if (counter == 5)
                {
                    Logger.LogError("Couldn't open a log file for writing. Skipping log file creation");

                    return;
                }

                Logger.LogWarning($"Couldn't open log file '{localPath}' for writing, trying another...");

                localPath = $"{fileNameWithoutExtension}.{counter++}.{extension}";
            }

            _logWriter = TextWriter.Synchronized(new StreamWriter(fileStream, Utility.UTF8NoBom));

            _flushTimer = new Timer(o => _logWriter?.Flush(), null, 2000, 2000);
        }

        public void LogEvent(object sender, LogEventArgs eventArgs)
        {
            _logWriter.WriteLine(eventArgs.ToString());
        }

        public void Dispose()
        {
            _flushTimer?.Dispose();
            _logWriter?.Flush();
            _logWriter?.Dispose();
        }

        ~EFTDiskLogListener()
        {
            Dispose();
        }
    }
}

#endif