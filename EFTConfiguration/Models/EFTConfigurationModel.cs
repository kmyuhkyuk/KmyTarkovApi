#if !UNITY_EDITOR

using System;
using BepInEx;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

// ReSharper disable MemberCanBePrivate.Global

namespace EFTConfiguration.Models
{
    internal class EFTConfigurationModel
    {
        public static EFTConfigurationModel Instance { get; private set; }

        public ConfigurationModel[] Configurations;

        public readonly PrefabManager PrefabManager;

        public readonly string ModName;

        public readonly GameObject EFTConfigurationPublic;

        public readonly string ModPath;

        public readonly PluginInfo Info;

        public bool Unlock;

        public Action CreateUI;

        private EFTConfigurationModel(string modName, GameObject eftConfigurationPublic, PrefabManager prefabManager,
            string modPath, PluginInfo info)
        {
            EFTConfigurationPublic = eftConfigurationPublic;
            ModPath = modPath;
            Info = info;
            PrefabManager = prefabManager;
            ModName = modName;

            var canvas = EFTConfigurationPublic.GetComponent<Canvas>();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 |
                                              AdditionalCanvasShaderChannels.Normal |
                                              AdditionalCanvasShaderChannels.Tangent;

            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule)).transform.SetParent(
                EFTConfigurationPublic.transform);

            Object.DontDestroyOnLoad(EFTConfigurationPublic);

            var settingsModel = SettingsModel.Instance;

            canvas.sortingOrder = settingsModel.KeySortingOrder.Value;
            settingsModel.KeySortingOrder.SettingChanged +=
                (value, value2) => canvas.sortingOrder = settingsModel.KeySortingOrder.Value;
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public static EFTConfigurationModel Create(string modName, GameObject eftConfigurationPublic,
            PrefabManager prefabManager, string modPath, PluginInfo info)
        {
            if (Instance != null)
                return Instance;

            return Instance = new EFTConfigurationModel(modName, eftConfigurationPublic, prefabManager, modPath, info);
        }
    }
}

#endif