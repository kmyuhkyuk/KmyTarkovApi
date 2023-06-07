using System;
using System.Diagnostics;

namespace EFTApi
{
    public static class EFTVersion
    {
        /// <summary>
        ///     Current Game File Version
        /// </summary>
        public static readonly Version GameVersion;

        public static readonly bool Is231Up;

        public static readonly bool Is330Up;

        public static readonly bool Is341Up;

        public static readonly bool Is350Up;

        static EFTVersion()
        {
            var processModule = Process.GetCurrentProcess().MainModule;

            if (processModule == null)
            {
                throw new ArgumentNullException(nameof(processModule));
            }

            var exeInfo = processModule.FileVersionInfo;

            var gameVersion = new Version(exeInfo.FileMajorPart, exeInfo.ProductMinorPart, exeInfo.ProductBuildPart,
                exeInfo.FilePrivatePart);

            GameVersion = gameVersion;

            Is231Up = gameVersion > new Version("0.12.12.17349");
            Is330Up = gameVersion > new Version("0.12.12.20243");
            Is341Up = gameVersion > new Version("0.12.12.20765"); //3.5.0 Add Launcher
            Is350Up = gameVersion > new Version("0.13.0.21734");
        }
    }
}