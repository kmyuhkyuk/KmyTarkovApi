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
        ///     com.mpt.core
        /// </summary>
        public static readonly BaseUnityPlugin MultiplayerTarkov;

        static EFTPlugins()
        {
            if (!RefTool.TryGetPlugin("com.spt-aki.core", out AkiCore))
            {
                Logger.LogWarning("Can't get com.spt-aki.core plugin(aki-core.dll)");
            }

            if (!RefTool.TryGetPlugin("com.spt-aki.custom", out AkiCustom))
            {
                Logger.LogWarning("Can't get com.spt-aki.custom plugin(aki-custom.dll)");
            }

            if (!RefTool.TryGetPlugin("com.spt-aki.debugging", out AkiDebugging))
            {
                Logger.LogWarning("Can't get com.spt-aki.debugging plugin(aki-debugging.dll)");
            }

            if (!RefTool.TryGetPlugin("com.spt-aki.singleplayer", out AkiSinglePlayer))
            {
                Logger.LogWarning("Can't get com.spt-aki.singleplayer plugin(aki-singleplayer.dll)");
            }

            RefTool.TryGetPlugin("com.mpt.core", out MultiplayerTarkov);
        }
    }
}