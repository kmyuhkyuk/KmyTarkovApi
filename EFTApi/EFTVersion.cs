using System;
using System.Diagnostics;
using System.Reflection;
using BepInEx;
using EFTReflection;

// ReSharper disable MemberCanBePrivate.Global

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
            if (GameVersionRange("0.12.12.17107", "0.12.12.17349"))
            {
                return Version.Parse("2.3.0");
            }
            else if (GameVersionRange("0.12.12.17349", "0.12.12.18346"))
            {
                return Version.Parse("2.3.1");
            }
            else if (GameVersionRange("0.12.12.18346", "0.12.12.19078"))
            {
                return Version.Parse("3.0.0");
            }
            else if (GameVersionRange("0.12.12.19078", "0.12.12.19428"))
            {
                return Version.Parse("3.2.1");
            }
            else if (GameVersionRange("0.12.12.19428", "0.12.12.19904"))
            {
                return Version.Parse("3.2.4");
            }
            else if (GameVersionRange("0.12.12.19904", "0.12.12.20243"))
            {
                return Version.Parse("3.2.5");
            }
            else if (GameVersionRange("0.12.12.20243", "0.12.12.20765"))
            {
                return Version.Parse("3.3.0");
            }
            else if (GameVersionRange("0.12.12.20765", "0.13.0.21734"))
            {
                return Version.Parse("3.4.1");
            }
            else if (GameVersionRange("0.13.0.21734", "0.13.0.22032"))
            {
                return Version.Parse("3.5.0");
            }
            else if (GameVersionRange("0.13.0.22032", "0.13.0.22173"))
            {
                return Version.Parse("3.5.1");
            }
            else if (GameVersionRange("0.13.0.22173", "0.13.0.22617"))
            {
                return Version.Parse("3.5.4");
            }
            else if (GameVersionRange("0.13.0.22617", "0.13.0.23043"))
            {
                return Version.Parse("3.5.5");
            }
            else if (GameVersionRange("0.13.0.23043", "0.13.0.23399"))
            {
                return Version.Parse("3.5.6");
            }
            else if (GameVersionRange("0.13.0.23399", "0.13.1.25206"))
            {
                return Version.Parse("3.5.7");
            }
            else if (GameVersion > Version.Parse("0.13.0.23399") &&
                     RefTool.TryGetPlugin("com.spt-aki.core", out var plugin))
            {
                return plugin.GetType().GetCustomAttribute<BepInPlugin>().Version;
            }
            else
            {
                return Version.Parse("0.0.0");
            }
        }

        /// <summary>
        ///     Game Version greater or equal Min Version and less Max Version
        /// </summary>
        /// <param name="minVersion"></param>
        /// <param name="maxVersion"></param>
        /// <returns></returns>
        public static bool GameVersionRange(string minVersion, string maxVersion)
        {
            return GameVersionRange(new Version(minVersion), new Version(maxVersion));
        }

        /// <summary>
        ///     Game Version greater or equal Min Version and less Max Version
        /// </summary>
        /// <param name="minVersion"></param>
        /// <param name="maxVersion"></param>
        /// <returns></returns>
        public static bool GameVersionRange(Version minVersion, Version maxVersion)
        {
            return VersionRange(GameVersion, minVersion, maxVersion);
        }

        /// <summary>
        ///     Aki Version greater or equal Min Version and less Max Version
        /// </summary>
        /// <param name="minVersion"></param>
        /// <param name="maxVersion"></param>
        /// <returns></returns>
        public static bool AkiVersionRange(string minVersion, string maxVersion)
        {
            return AkiVersionRange(new Version(minVersion), new Version(maxVersion));
        }

        /// <summary>
        ///     Aki Version greater or equal Min Version and less Max Version
        /// </summary>
        /// <param name="minVersion"></param>
        /// <param name="maxVersion"></param>
        /// <returns></returns>
        public static bool AkiVersionRange(Version minVersion, Version maxVersion)
        {
            return VersionRange(AkiVersion, minVersion, maxVersion);
        }

        /// <summary>
        ///     Target Version greater or equal Min Version and less Max Version
        /// </summary>
        /// <param name="targetVersion"></param>
        /// <param name="minVersion"></param>
        /// <param name="maxVersion"></param>
        /// <returns></returns>
        public static bool VersionRange(Version targetVersion, Version minVersion, Version maxVersion)
        {
            return targetVersion >= minVersion && targetVersion < maxVersion;
        }
    }
}