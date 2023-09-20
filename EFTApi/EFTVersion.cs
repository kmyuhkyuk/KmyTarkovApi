using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using BepInEx;
using EFT;

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

            AkiVersion = GetAkiVersion();
        }

        private static Version GetAkiVersion()
        {
            if (VersionRange("0.12.12.17107", "0.12.12.17349"))
            {
                return new Version("2.3.0");
            }
            else if (VersionRange("0.12.12.17349", "0.12.12.18346"))
            {
                return new Version("2.3.1");
            }
            else if (VersionRange("0.12.12.18346", "0.12.12.19078"))
            {
                return new Version("3.0.0");
            }
            else if (VersionRange("0.12.12.19078", "0.12.12.19428"))
            {
                return new Version("3.2.1");
            }
            else if (VersionRange("0.12.12.19428", "0.12.12.19904"))
            {
                return new Version("3.2.4");
            }
            else if (VersionRange("0.12.12.19904", "0.12.12.20243"))
            {
                return new Version("3.2.5");
            }
            else if (VersionRange("0.12.12.20243", "0.12.12.20765"))
            {
                return new Version("3.3.0");
            }
            else if (VersionRange("0.12.12.20765", "0.13.0.21734"))
            {
                return new Version("3.4.1");
            }
            else if (VersionRange("0.13.0.21734", "0.13.0.22032"))
            {
                return new Version("3.5.0");
            }
            else if (VersionRange("0.13.0.22032", "0.13.0.22173"))
            {
                return new Version("3.5.1");
            }
            else if (VersionRange("0.13.0.22173", "0.13.0.22617"))
            {
                return new Version("3.5.4");
            }
            else if (VersionRange("0.13.0.22617", "0.13.0.23043"))
            {
                return new Version("3.5.5");
            }
            else if (VersionRange("0.13.0.23043", "0.13.0.23399"))
            {
                return new Version("3.5.6");
            }
            else if (VersionRange("0.13.0.23399", "0.13.1.25206"))
            {
                return new Version("3.5.7");
            }
            else if (GameVersion > new Version("0.13.0.23399"))
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .Single(x => x.ManifestModule.Name == "aki-core.dll")
                    .GetTypes().Single(x => x.Name == "AkiCorePlugin").GetCustomAttribute<BepInPlugin>().Version;
            }
            else
            {
                return new Version("0.0.0");
            }
        }

        public static bool VersionRange(string minVersion, string maxVersion)
        {
            return GameVersion >= new Version(minVersion) && GameVersion <= new Version(maxVersion);
        }
    }
}