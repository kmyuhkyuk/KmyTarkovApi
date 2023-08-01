using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using BepInEx;

namespace EFTApi
{
    public static class EFTVersion
    {
        /// <summary>
        ///     Current Game File Version
        /// </summary>
        public static readonly Version GameVersion;

        public static readonly Version AkiVersion;

        /*/// <summary>
        ///     GameVersion > 0.12.12.17107
        /// </summary>
        public static readonly bool Is230Up;

        /// <summary>
        ///     GameVersion > 0.12.12.17349
        /// </summary>
        public static readonly bool Is231Up;

        /// <summary>
        ///     GameVersion > 0.12.12.18346
        /// </summary>
        public static readonly bool Is300Up;

        /// <summary>
        ///     GameVersion > 0.12.12.19078
        /// </summary>
        public static readonly bool Is321Up;

        /// <summary>
        ///     GameVersion > 0.12.12.19428
        /// </summary>
        public static readonly bool Is324Up;

        /// <summary>
        ///     GameVersion > 0.12.12.19904
        /// </summary>
        public static readonly bool Is325Up;

        /// <summary>
        ///     GameVersion > 0.12.12.20243
        /// </summary>
        public static readonly bool Is330Up;

        /// <summary>
        ///     GameVersion > 0.12.12.20765
        /// </summary>
        public static readonly bool Is341Up;

        /// <summary>
        ///     GameVersion > 0.13.0.21734
        /// </summary>
        public static readonly bool Is350Up;

        /// <summary>
        ///     GameVersion > 0.13.0.22032
        /// </summary>
        public static readonly bool Is351Up;

        /// <summary>
        ///     GameVersion > 0.13.0.22173
        /// </summary>
        public static readonly bool Is354Up;

        /// <summary>
        ///     GameVersion > 0.13.0.22617
        /// </summary>
        public static readonly bool Is355Up;

        /// <summary>
        ///     GameVersion > 0.13.0.23043
        /// </summary>
        public static readonly bool Is356Up;

        /// <summary>
        ///     GameVersion > 0.13.0.23399
        /// </summary>
        public static readonly bool Is357Up;*/

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

            if (gameVersion > new Version("0.13.0.23399"))
            {
                AkiVersion = AppDomain.CurrentDomain.GetAssemblies()
                    .Single(x => x.ManifestModule.Name == "aki-core.dll")
                    .GetTypes().Single(x => x.Name == "AkiCorePlugin").GetCustomAttribute<BepInPlugin>().Version;
            }
            else if (gameVersion >= new Version("0.12.12.17107") && gameVersion < new Version("0.12.12.17349"))
            {
                AkiVersion = new Version("2.3.0");
            }
            else if (gameVersion >= new Version("0.12.12.17349") && gameVersion < new Version("0.12.12.18346"))
            {
                AkiVersion = new Version("2.3.1");
            }
            else if (gameVersion >= new Version("0.12.12.18346") && gameVersion < new Version("0.12.12.19078"))
            {
                AkiVersion = new Version("3.0.0");
            }
            else if (gameVersion >= new Version("0.12.12.19078") && gameVersion < new Version("0.12.12.19428"))
            {
                AkiVersion = new Version("3.2.1");
            }
            else if (gameVersion >= new Version("0.12.12.19428") && gameVersion < new Version("0.12.12.19904"))
            {
                AkiVersion = new Version("3.2.4");
            }
            else if (gameVersion >= new Version("0.12.12.19904") && gameVersion < new Version("0.12.12.20243"))
            {
                AkiVersion = new Version("3.2.5");
            }
            else if (gameVersion >= new Version("0.12.12.20243") && gameVersion < new Version("0.12.12.20765"))
            {
                AkiVersion = new Version("3.3.0");
            }
            else if (gameVersion >= new Version("0.12.12.20765") && gameVersion < new Version("0.13.0.21734"))
            {
                AkiVersion = new Version("3.4.1");
            }
            else if (gameVersion >= new Version("0.13.0.21734") && gameVersion < new Version("0.13.0.22032"))
            {
                AkiVersion = new Version("3.5.0");
            }
            else if (gameVersion >= new Version("0.13.0.22032") && gameVersion < new Version("0.13.0.22173"))
            {
                AkiVersion = new Version("3.5.1");
            }
            else if (gameVersion >= new Version("0.13.0.22173") && gameVersion < new Version("0.13.0.22617"))
            {
                AkiVersion = new Version("3.5.4");
            }
            else if (gameVersion >= new Version("0.13.0.22617") && gameVersion < new Version("0.13.0.23043"))
            {
                AkiVersion = new Version("3.5.5");
            }
            else if (gameVersion >= new Version("0.13.0.23043") && gameVersion < new Version("0.13.0.23399"))
            {
                AkiVersion = new Version("3.5.6");
            }

            /*Is230Up = gameVersion > new Version("0.12.12.17107");
            Is231Up = gameVersion > new Version("0.12.12.17349");
            Is300Up = gameVersion > new Version("0.12.12.18346");
            Is321Up = gameVersion > new Version("0.12.12.19078");
            Is324Up = gameVersion > new Version("0.12.12.19428");
            Is325Up = gameVersion > new Version("0.12.12.19904");
            Is330Up = gameVersion > new Version("0.12.12.20243");
            Is341Up = gameVersion > new Version("0.12.12.20765"); //3.5.0 Add Launcher
            Is350Up = gameVersion > new Version("0.13.0.21734");
            Is351Up = gameVersion > new Version("0.13.0.22032");
            Is354Up = gameVersion > new Version("0.13.0.22173");
            Is355Up = gameVersion > new Version("0.13.0.22617");
            Is356Up = gameVersion > new Version("0.13.0.23043");
            Is357Up = gameVersion > new Version("0.13.0.23399");*/
        }
    }
}