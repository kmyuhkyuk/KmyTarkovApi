#if !UNITY_EDITOR

using System;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

// ReSharper disable MemberCanBePrivate.Global

namespace KmyTarkovConfiguration.Models
{
    internal class EFTConfigurationModel
    {
        public static EFTConfigurationModel Instance { get; private set; }

        private static readonly ManualLogSource Logger =
            BepInEx.Logging.Logger.CreateLogSource(nameof(EFTConfigurationModel));

        public ConfigurationModel[] Configurations;

        public readonly PrefabManager PrefabManager;

        public readonly string ModName;

        public readonly GameObject EFTConfigurationPublic = new GameObject("KmyTarkovConfigurationPublic",
            typeof(Canvas),
            typeof(CanvasScaler), typeof(GraphicRaycaster));

        public readonly string ModPath = Path.Combine(Paths.PluginPath, "kmyuhkyuk-KmyTarkovApi");

        public readonly PluginInfo Info;

        public bool Unlock;

        public Action CreateUI;

        private EFTConfigurationModel(string modName, PluginInfo info)
        {
            Info = info;
            ModName = modName;

            var canvas = EFTConfigurationPublic.GetComponent<Canvas>();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 |
                                              AdditionalCanvasShaderChannels.Normal |
                                              AdditionalCanvasShaderChannels.Tangent;

            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule)).transform.SetParent(
                EFTConfigurationPublic.transform);

            Object.DontDestroyOnLoad(EFTConfigurationPublic);

            var assetBundle =
                AssetBundle.LoadFromFile(Path.Combine(ModPath, "bundles", "kmytarkovconfiguration.bundle"));
            if (assetBundle == null)
            {
                Logger.LogError($"{nameof(EFTConfigurationModel)}: Failed to load AssetBundle!");
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

                PrefabManager = prefabManager;

                assetBundle.Unload(false);
            }

            var settingsModel = SettingsModel.Instance;

            canvas.sortingOrder = settingsModel.KeySortingOrder.Value;
            settingsModel.KeySortingOrder.SettingChanged +=
                (value, value2) => canvas.sortingOrder = settingsModel.KeySortingOrder.Value;
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public GameObject LoadConfiguration()
        {
            return Object.Instantiate(PrefabManager.eftConfiguration, EFTConfigurationPublic.transform);
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public static EFTConfigurationModel Create(string modName, PluginInfo info)
        {
            if (Instance != null)
                return Instance;

            return Instance = new EFTConfigurationModel(modName, info);
        }
    }
}

#endif