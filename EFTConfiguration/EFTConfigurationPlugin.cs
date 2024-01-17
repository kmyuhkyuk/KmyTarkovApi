#if !UNITY_EDITOR

using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using EFTConfiguration.Attributes;
using EFTConfiguration.Models;
using EFTConfiguration.Patches;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EFTConfiguration
{
    [BepInPlugin("com.kmyuhkyuk.EFTConfiguration", "EFTConfiguration", "1.1.8")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/1215-eft-api")]
    public class EFTConfigurationPlugin : BaseUnityPlugin
    {
        private static readonly GameObject EFTConfigurationPublic = new GameObject("EFTConfigurationPublic",
            typeof(Canvas),
            typeof(CanvasScaler), typeof(GraphicRaycaster));

        private static readonly string ModPath = Path.Combine(Paths.PluginPath, "kmyuhkyuk-EFTApi");

        private void Awake()
        {
            BepInEx.Logging.Logger.Listeners.Add(new EFTDiskLogListener("FullLogOutput.log"));

            SettingsModel.Create(Config);

            var assetBundle = AssetBundle.LoadFromFile(Path.Combine(ModPath, "bundles", "eftconfiguration.bundle"));
            if (assetBundle == null)
            {
                Logger.LogError($"{nameof(EFTConfigurationPlugin)}: Failed to load AssetBundle!");
            }
            else
            {
                var prefabManager = assetBundle.LoadAllAssets<PrefabManager>()[0];

                foreach (var tmpText in prefabManager.AllGameObject.SelectMany(x =>
                             x.GetComponentsInChildren<TMP_Text>(true)))
                {
                    // ReSharper disable once Unity.UnknownResource
                    tmpText.font = Resources.Load<TMP_FontAsset>("ui/fonts/jovanny lemonad - bender normal sdf");
                }

                EFTConfigurationModel.Create(GetType().GetCustomAttribute<BepInPlugin>().Name, EFTConfigurationPublic,
                    prefabManager, ModPath, Info);

                Instantiate(prefabManager.eftConfiguration, EFTConfigurationPublic.transform);

                assetBundle.Unload(false);
            }
        }

        private void Start()
        {
            new CursorLockStatePatch().Enable();
            new CursorVisiblePatch().Enable();
        }
    }
}

#endif