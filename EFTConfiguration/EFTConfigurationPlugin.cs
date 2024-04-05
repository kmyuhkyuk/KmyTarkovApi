#if !UNITY_EDITOR

using System.Reflection;
using BepInEx;
using EFTConfiguration.Attributes;
using EFTConfiguration.Models;
using EFTConfiguration.Patches;

namespace EFTConfiguration
{
    [BepInPlugin("com.kmyuhkyuk.EFTConfiguration", "EFTConfiguration", "1.2.0")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/1215-eft-api")]
    public class EFTConfigurationPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            BepInEx.Logging.Logger.Listeners.Add(new EFTDiskLogListener("FullLogOutput.log"));

            SettingsModel.Create(Config);

            EFTConfigurationModel.Create(GetType().GetCustomAttribute<BepInPlugin>().Name, Info).LoadConfiguration();
        }

        private void Start()
        {
            new CursorLockStatePatch().Enable();
            new CursorVisiblePatch().Enable();
        }
    }
}

#endif