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

        /// <summary>
        ///     Current Aki Version
        /// </summary>
        public static readonly Version AkiVersion;

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

            if (gameVersion >= new Version("0.12.12.17107") && gameVersion < new Version("0.12.12.17349"))
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
            else if (gameVersion >= new Version("0.13.0.23399") && gameVersion < new Version("0.13.1.25206"))
            {
                AkiVersion = new Version("3.5.7");
            }
            else if (gameVersion > new Version("0.13.0.23399"))
            {
                AkiVersion = AppDomain.CurrentDomain.GetAssemblies()
                    .Single(x => x.ManifestModule.Name == "aki-core.dll")
                    .GetTypes().Single(x => x.Name == "AkiCorePlugin").GetCustomAttribute<BepInPlugin>().Version;
            }
            else
            {
                AkiVersion = new Version("0.0.0");
            }
        }
    }
}