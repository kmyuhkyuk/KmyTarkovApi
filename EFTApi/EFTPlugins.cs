using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using EFTReflection;

// ReSharper disable NotAccessedField.Global

namespace EFTApi
{
    public static class EFTPlugins
    {
        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(EFTPlugins));

        /// <summary>
        ///     com.spt-aki.core
        /// </summary>
        public static readonly BaseUnityPlugin AkiCore;

        /// <summary>
        ///     com.spt-aki.custom
        /// </summary>
        public static readonly BaseUnityPlugin AkiCustom;

        /// <summary>
        ///     com.spt-aki.debugging
        /// </summary>
        public static readonly BaseUnityPlugin AkiDebugging;

        /// <summary>
        ///     com.spt-aki.singleplayer
        /// </summary>
        public static readonly BaseUnityPlugin AkiSinglePlayer;

        /// <summary>
        ///     com.fika.core
        /// </summary>
        public static readonly BaseUnityPlugin FikaCore;

        /// <summary>
        ///     Aki.Common.dll
        /// </summary>
        public static readonly Assembly AkiCommon;

        /// <summary>
        ///     Aki.Reflection.dll
        /// </summary>
        public static readonly Assembly AkiReflection;

        static EFTPlugins()
        {
            if (!RefTool.TryGetPlugin("com.SPT.core", out AkiCore) &&
                !RefTool.TryGetPlugin("com.spt-aki.core", out AkiCore))
            {
                Logger.LogWarning("Can't get com.SPT.core plugin(spt-core.dll)");
            }

            if (!RefTool.TryGetPlugin("com.SPT.custom", out AkiCustom) &&
                !RefTool.TryGetPlugin("com.spt-aki.custom", out AkiCustom))
            {
                Logger.LogWarning("Can't get ccom.SPT.custom plugin(spt-custom.dll)");
            }

            if (!RefTool.TryGetPlugin("com.SPT.debugging", out AkiDebugging) &&
                !RefTool.TryGetPlugin("com.spt-aki.debugging", out AkiDebugging))
            {
                Logger.LogWarning("Can't get com.SPT.debugging plugin(spt-debugging.dll)");
            }

            if (!RefTool.TryGetPlugin("com.SPT.singleplayer", out AkiSinglePlayer) &&
                !RefTool.TryGetPlugin("com.spt-aki.singleplayer", out AkiSinglePlayer))
            {
                Logger.LogWarning("Can't get com.SPT.singleplayer plugin(spt-singleplayer.dll)");
            }

            RefTool.TryGetPlugin("com.fika.core", out FikaCore);

            if (!RefTool.TryGetAssembly("spt-common", out AkiCommon) &&
                !RefTool.TryGetAssembly("Aki.Common", out AkiCommon))
            {
                Logger.LogWarning("Can't get spt-common.dll");
            }

            if (!RefTool.TryGetAssembly("spt-reflection", out AkiReflection) &&
                !RefTool.TryGetAssembly("Aki.Reflection", out AkiReflection))
            {
                Logger.LogWarning("Can't get spt-reflection.dll");
            }
        }
    }
}