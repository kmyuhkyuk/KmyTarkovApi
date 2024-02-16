using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using EFTReflection;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace EFTApi
{
    public static class EFTVersion
    {
        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(EFTVersion));

        /// <summary>
        ///     Current Game File Version
        /// </summary>
        public static readonly Version GameVersion;

        /// <summary>
        ///     Current Aki Version
        /// </summary>
        public static readonly Version AkiVersion;

        private static readonly Dictionary<string, Version> SaveVersions =
            new Dictionary<string, Version>();

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

        public static Version Parse(string version)
        {
            if (SaveVersions.TryGetValue(version, out var saveVersion))
                return saveVersion;

            saveVersion = Version.Parse(version);

            SaveVersions.Add(version, saveVersion);

            return saveVersion;
        }

        private static Version GetAkiVersion()
        {
            if (GameVersionRange("0.12.12.17107", "0.12.12.17349"))
                return Parse("2.3.0");

            if (GameVersionRange("0.12.12.17349", "0.12.12.18346"))
                return Parse("2.3.1");

            if (GameVersionRange("0.12.12.18346", "0.12.12.19078"))
                return Parse("3.0.0");

            if (GameVersionRange("0.12.12.19078", "0.12.12.19428"))
                return Parse("3.2.1");

            if (GameVersionRange("0.12.12.19428", "0.12.12.19904"))
                return Parse("3.2.4");

            if (GameVersionRange("0.12.12.19904", "0.12.12.20243"))
                return Parse("3.2.5");

            if (GameVersionRange("0.12.12.20243", "0.12.12.20765"))
                return Parse("3.3.0");

            if (GameVersionRange("0.12.12.20765", "0.13.0.21734"))
                return Parse("3.4.1");

            if (GameVersionRange("0.13.0.21734", "0.13.0.22032"))
                return Parse("3.5.0");

            if (GameVersionRange("0.13.0.22032", "0.13.0.22173"))
                return Parse("3.5.1");

            if (GameVersionRange("0.13.0.22173", "0.13.0.22617"))
                return Parse("3.5.4");

            if (GameVersionRange("0.13.0.22617", "0.13.0.23043"))
                return Parse("3.5.5");

            if (GameVersionRange("0.13.0.23043", "0.13.0.23399"))
                return Parse("3.5.6");

            if (GameVersionRange("0.13.0.23399", "0.13.1.25206"))
                return Parse("3.5.7");

            if (GameVersion > Parse("0.13.0.23399"))
            {
                if (RefTool.TryGetPlugin("com.spt-aki.core", out var plugin))
                {
                    var version = plugin.GetType().GetCustomAttribute<BepInPlugin>().Version;

                    if (version == Parse("0.0.0") || version == Parse("1.0.0"))
                    {
                        Logger.LogError(
                            "The com.spt-aki.core plugin version incorrect, please do not overwrite aki-core.dll file");
                    }

                    return version;
                }

                Logger.LogError("Can't get com.spt-aki.core plugin, please run on Spt Tarkov");
            }

            Logger.LogError("Unable to get current Aki version, which will cause errors in mod");

            return Parse("0.0.0");
        }

        /// <summary>
        ///     Game Version greater or equal Min Version and less Max Version
        /// </summary>
        /// <param name="minVersion"></param>
        /// <param name="maxVersion"></param>
        /// <returns></returns>
        public static bool GameVersionRange(string minVersion, string maxVersion)
        {
            return GameVersionRange(Parse(minVersion), Parse(maxVersion));
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
            return AkiVersionRange(Parse(minVersion), Parse(maxVersion));
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

        public static void WriteVersionLog()
        {
            Logger.LogMessage($"GameVersion:{GameVersion} AkiVersion:{AkiVersion}");
        }
    }
}