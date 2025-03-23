using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using KmyTarkovReflection;

// ReSharper disable NotAccessedField.Global

namespace KmyTarkovApi
{
    public static class EFTPlugins
    {
        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(EFTPlugins));

        /// <summary>
        ///     com.spt-aki.core
        /// </summary>
        public static readonly BaseUnityPlugin SPTCore;

        /// <summary>
        ///     com.spt-aki.custom
        /// </summary>
        public static readonly BaseUnityPlugin SPTCustom;

        /// <summary>
        ///     com.spt-aki.debugging
        /// </summary>
        public static readonly BaseUnityPlugin SPTDebugging;

        /// <summary>
        ///     com.spt-aki.singleplayer
        /// </summary>
        public static readonly BaseUnityPlugin SPTSinglePlayer;

        /// <summary>
        ///     com.fika.core
        /// </summary>
        public static readonly BaseUnityPlugin FikaCore;

        /// <summary>
        ///     Aki.Common.dll
        /// </summary>
        public static readonly Assembly SPTCommon;

        /// <summary>
        ///     Aki.Reflection.dll
        /// </summary>
        public static readonly Assembly SPTReflection;

        static EFTPlugins()
        {
            if (!RefTool.TryGetPlugin("com.SPT.core", out SPTCore))
            {
                Logger.LogWarning("Can't get com.SPT.core plugin(spt-core.dll)");
            }

            if (!RefTool.TryGetPlugin("com.SPT.custom", out SPTCustom))
            {
                Logger.LogWarning("Can't get ccom.SPT.custom plugin(spt-custom.dll)");
            }

            if (!RefTool.TryGetPlugin("com.SPT.debugging", out SPTDebugging))
            {
                Logger.LogWarning("Can't get com.SPT.debugging plugin(spt-debugging.dll)");
            }

            if (!RefTool.TryGetPlugin("com.SPT.singleplayer", out SPTSinglePlayer))
            {
                Logger.LogWarning("Can't get com.SPT.singleplayer plugin(spt-singleplayer.dll)");
            }

            RefTool.TryGetPlugin("com.fika.core", out FikaCore);

            if (!RefTool.TryGetAssembly("spt-common", out SPTCommon))
            {
                Logger.LogWarning("Can't get spt-common.dll");
            }

            if (!RefTool.TryGetAssembly("spt-reflection", out SPTReflection))
            {
                Logger.LogWarning("Can't get spt-reflection.dll");
            }
        }
    }
}