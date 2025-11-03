#if !UNITY_EDITOR

using BepInEx;
using HarmonyLib;
using KmyTarkovConfiguration.Attributes;
using KmyTarkovConfiguration.Models;
using KmyTarkovConfiguration.Patches;

namespace KmyTarkovConfiguration
{
    [BepInPlugin("com.kmyuhkyuk.KmyTarkovConfiguration", "KmyTarkovConfiguration", "1.4.0")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/1215-kmy-tarkov-api")]
    public class KmyTarkovConfigurationPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            BepInEx.Logging.Logger.Listeners.Add(new EFTDiskLogListener("FullLogOutput.log"));

            SettingsModel.Create(Config);
        }

        private void Start()
        {
            EFTConfigurationModel.Create("KmyTarkovConfiguration", Info).LoadConfiguration();

            Harmony.CreateAndPatchAll(typeof(CursorLockStatePatch));
            Harmony.CreateAndPatchAll(typeof(CursorVisiblePatch));
        }
    }
}

#endif