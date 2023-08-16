#if !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using EFTConfiguration.Attributes;
using EFTConfiguration.Helpers;
using EFTConfiguration.Patches;
using Force.Crc32;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EFTConfiguration
{
    [BepInPlugin("com.kmyuhkyuk.EFTConfiguration", "kmyuhkyuk-EFTConfiguration", "1.1.5")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/1215-eft-api")]
    public class EFTConfigurationPlugin : BaseUnityPlugin
    {
        private static Dictionary<string, PluginInfo> PluginInfos => Chainloader.PluginInfos;

        private readonly GameObject _eftConfigurationPublic = new GameObject("EFTConfigurationPublic", typeof(Canvas),
            typeof(CanvasScaler), typeof(GraphicRaycaster));

        private readonly PropertyInfo _coreConfigInfo = typeof(ConfigFile).GetProperty("CoreConfig",
            BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

        private readonly FieldInfo[] _eftConfigurationPluginAttributesFields =
            typeof(EFTConfigurationPluginAttributes).GetFields();

        private readonly (string, Type)[] _eftConfigurationPluginAttributesFieldsTuple =
            typeof(EFTConfigurationPluginAttributes).GetFields().Select(x => (x.Name, x.FieldType)).ToArray();

        private static readonly ManualLogSource ModVerifyLogger = BepInEx.Logging.Logger.CreateLogSource("ModVerify");

        internal static SettingsData SetData { get; private set; }

        internal const string ModName = "kmyuhkyuk-EFTConfiguration";

        internal static ConfigurationData[] Configurations { get; private set; }

        internal static PrefabManager PrefabManager { get; private set; }

        internal static readonly string ModPath = Path.Combine(Paths.PluginPath, "kmyuhkyuk-EFTApi");

        internal static bool Unlock;

        public EFTConfigurationPlugin()
        {
            var canvas = _eftConfigurationPublic.GetComponent<Canvas>();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 |
                                              AdditionalCanvasShaderChannels.Normal |
                                              AdditionalCanvasShaderChannels.Tangent;

            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule)).transform.SetParent(
                _eftConfigurationPublic.transform);

            DontDestroyOnLoad(_eftConfigurationPublic);

            SetData = new SettingsData(Config);

            canvas.sortingOrder = SetData.KeySortingOrder.Value;
            SetData.KeySortingOrder.SettingChanged +=
                (value, value2) => canvas.sortingOrder = SetData.KeySortingOrder.Value;

            CustomLocalizedHelper.CurrentLanguage = SetData.KeyLanguage.Value;
            SetData.KeyLanguage.SettingChanged += (value, value2) =>
                CustomLocalizedHelper.CurrentLanguage = SetData.KeyLanguage.Value;

            BepInEx.Logging.Logger.Listeners.Add(new EFTLogListener());
        }


        private void Start()
        {
            new CursorLockStatePatch().Enable();
            new CursorVisiblePatch().Enable();

            Init();
        }

        private void Awake()
        {
            var assetBundle = AssetBundle.LoadFromFile(Path.Combine(ModPath, "bundles", "eftconfiguration.bundle"));

            if (assetBundle == null)
            {
                Logger.LogError($"{nameof(EFTConfigurationPlugin)}: Failed to load AssetBundle!");
            }
            else
            {
                var prefabManager = assetBundle.LoadAllAssets<PrefabManager>()[0];

                PrefabManager = prefabManager;

                Instantiate(prefabManager.eftConfiguration, _eftConfigurationPublic.transform);

                assetBundle.Unload(false);
            }
        }

        private void Init()
        {
            var configurationList = new List<ConfigurationData>
                { GetCoreConfigurationData(), GetConfigurationData(Info) };

            foreach (var pluginInfo in PluginInfos.Values)
            {
                ModVerifyLogger.LogMessage(
                    $"{Path.GetFileNameWithoutExtension(pluginInfo.Location)} Version:{pluginInfo.Metadata.Version} CRC32:{(string.IsNullOrEmpty(pluginInfo.Location) ? "null" : Crc32CAlgorithm.Compute(File.ReadAllBytes(pluginInfo.Location)).ToString("X"))}");

                if (pluginInfo == Info)
                    continue;

                configurationList.Add(GetConfigurationData(pluginInfo));
            }

            Configurations = configurationList.ToArray();
        }

        private ConfigurationData GetConfigurationData(PluginInfo pluginInfo)
        {
            var instance = pluginInfo.Instance;

            var type = instance.GetType();

            var attributes = new EFTConfigurationPluginAttributes(string.Empty);

            var hasAttributes = false;
            foreach (var attribute in type.GetCustomAttributes())
            {
                var attributeType = attribute.GetType();

                if (attributeType.Name == nameof(EFTConfigurationPluginAttributes))
                {
                    var attributeFieldInfos = attributeType.GetFields();

                    if (attributeFieldInfos.Select(x => (x.Name, x.FieldType))
                        .SequenceEqual(_eftConfigurationPluginAttributesFieldsTuple))
                    {
                        hasAttributes = true;

                        for (var i = 0; i < _eftConfigurationPluginAttributesFields.Length; i++)
                        {
                            var eftConfigurationPluginAttributesField = _eftConfigurationPluginAttributesFields[i];

                            var attributeFieldInfo = attributeFieldInfos[i];

                            eftConfigurationPluginAttributesField.SetValue(attributes,
                                attributeFieldInfo.GetValue(attribute));
                        }

                        break;
                    }
                }
            }

            if (!hasAttributes)
            {
                attributes.ModURL = FileVersionInfo.GetVersionInfo(pluginInfo.Location).CompanyName;
                attributes.HidePlugin = type.GetCustomAttributes<BrowsableAttribute>().Any(x => !x.Browsable);
            }

            var configFile = instance.Config;

            var metaData = pluginInfo.Metadata;

            CustomLocalizedHelper.LanguageList.Add(metaData.Name,
                GetLanguageDictionary(pluginInfo, hasAttributes ? attributes.LocalizedPath : string.Empty));

            return new ConfigurationData(configFile, metaData, attributes);
        }

        private ConfigurationData GetCoreConfigurationData()
        {
            var configFile = (ConfigFile)_coreConfigInfo.GetValue(null);

            var metaData = new BepInPlugin("BepInEx", "BepInEx",
                typeof(BaseUnityPlugin).Assembly.GetName().Version.ToString());

            return new ConfigurationData(configFile, metaData, new EFTConfigurationPluginAttributes(string.Empty),
                true);
        }

        private static Dictionary<string, Dictionary<string, string>> GetLanguageDictionary(PluginInfo pluginInfo,
            string localizedPath)
        {
            var localizedDictionary = new Dictionary<string, Dictionary<string, string>>();

            if (pluginInfo == null)
                return localizedDictionary;

            if (string.IsNullOrEmpty(localizedPath))
                return localizedDictionary;

            var assemblyPath = Path.GetDirectoryName(pluginInfo.Location);

            if (string.IsNullOrEmpty(assemblyPath))
                return localizedDictionary;

            var localizedDirectory = new DirectoryInfo(Path.Combine(assemblyPath, localizedPath));

            if (!localizedDirectory.Exists)
                return localizedDictionary;

            foreach (var localized in localizedDirectory.GetFiles("*.json"))
            {
                using (var stream = new StreamReader(localized.FullName))
                {
                    localizedDictionary.Add(Path.GetFileNameWithoutExtension(localized.Name),
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(stream.ReadToEnd()));
                }
            }

            return localizedDictionary;
        }

        public class ConfigurationData
        {
            public readonly bool IsCore;

            public Version ModVersion => _metadata.Version;

            public string ModName => _metadata.Name;

            public readonly EFTConfigurationPluginAttributes ConfigurationPluginAttributes;

            private readonly BepInPlugin _metadata;

            private readonly ConfigFile _configFile;

            public Action SettingChanged;

            public int ConfigCount => _configFile.Count;

            public ConfigData[] Configs => _configFile.Select(x => new ConfigData(x.Key, x.Value, IsCore)).ToArray();

            public ConfigurationData(ConfigFile configFile, BepInPlugin metadata,
                EFTConfigurationPluginAttributes configurationPlugin, bool isCore = false)
            {
                _configFile = configFile;
                _metadata = metadata;
                ConfigurationPluginAttributes = configurationPlugin;
                IsCore = isCore;

                configFile.SettingChanged += (value, value2) => SettingChanged?.Invoke();
            }
        }

        public class ConfigData
        {
            public string Section => _configDefinition.Section;

            public string Key => _configDefinition.Key;

            public object DefaultValue => _configEntryBase.DefaultValue;

            public Type SettingType => _configEntryBase.SettingType;

            public ConfigDescription ConfigDescription => _configEntryBase.Description;

            public string Description => ConfigDescription.Description;

            public AcceptableValueBase AcceptableValueBase => ConfigDescription.AcceptableValues;

            public readonly EFTConfigurationAttributes ConfigurationAttributes = new EFTConfigurationAttributes();

            private readonly ConfigDefinition _configDefinition;

            private readonly ConfigEntryBase _configEntryBase;

            private static readonly FieldInfo[] EFTConfigurationAttributesFields =
                typeof(EFTConfigurationAttributes).GetFields();

            private static readonly (string, Type)[] EFTConfigurationAttributesFieldsTuple =
                typeof(EFTConfigurationAttributes).GetFields().Select(x => (x.Name, x.FieldType)).ToArray();

            private static readonly FieldInfo[] ConfigurationManagerAttributesFields =
                typeof(ConfigurationManagerAttributes).GetFields();

            private static readonly (string, Type)[] ConfigurationManagerAttributesFieldsTuple =
                typeof(ConfigurationManagerAttributes).GetFields().Select(x => (x.Name, x.FieldType)).ToArray();

            public ConfigData(ConfigDefinition configDefinition, ConfigEntryBase configEntryBase, bool isCore)
            {
                _configDefinition = configDefinition;
                _configEntryBase = configEntryBase;

                if (!isCore)
                {
                    object eftConfigurationAttributes = null;
                    var eftConfigurationAttributesFieldInfos = Array.Empty<FieldInfo>();

                    object configurationManagerAttributes = null;
                    var configurationManagerAttributesFieldInfos = Array.Empty<FieldInfo>();

                    foreach (var tag in ConfigDescription.Tags)
                    {
                        var tagType = tag.GetType();

                        switch (tagType.Name)
                        {
                            case nameof(EFTConfigurationAttributes):
                            {
                                var tagFieldInfos = tagType.GetFields();

                                if (tagFieldInfos.Select(x => (x.Name, x.FieldType))
                                    .SequenceEqual(EFTConfigurationAttributesFieldsTuple))
                                {
                                    eftConfigurationAttributes = tag;
                                    eftConfigurationAttributesFieldInfos = tagFieldInfos;
                                }

                                break;
                            }
                            case nameof(ConfigurationManagerAttributes):
                            {
                                var tagFieldInfos = tagType.GetFields();

                                if (tagFieldInfos.Select(x => (x.Name, x.FieldType))
                                    .SequenceEqual(ConfigurationManagerAttributesFieldsTuple))
                                {
                                    configurationManagerAttributes = tag;
                                    configurationManagerAttributesFieldInfos = tagFieldInfos;
                                }

                                break;
                            }
                        }
                    }

                    if (eftConfigurationAttributes != null)
                    {
                        for (var i = 0; i < EFTConfigurationAttributesFields.Length; i++)
                        {
                            var configurationAttributesFields = EFTConfigurationAttributesFields[i];

                            var tagFieldInfo = eftConfigurationAttributesFieldInfos[i];

                            configurationAttributesFields.SetValue(ConfigurationAttributes,
                                tagFieldInfo.GetValue(eftConfigurationAttributes));
                        }
                    }
                    else if (configurationManagerAttributes != null)
                    {
                        ConfigurationAttributes.HideSetting = !(bool?)configurationManagerAttributesFieldInfos
                            .Single(x => x.Name == nameof(ConfigurationManagerAttributes.Browsable))
                            .GetValue(configurationManagerAttributes) ?? false;
                        ConfigurationAttributes.HideRest = (bool?)configurationManagerAttributesFieldInfos.Single(x =>
                                x.Name == nameof(ConfigurationManagerAttributes.HideDefaultButton))
                            .GetValue(configurationManagerAttributes) ?? false;
                        ConfigurationAttributes.HideRange = (bool?)configurationManagerAttributesFieldInfos.Single(x =>
                                x.Name == nameof(ConfigurationManagerAttributes.ShowRangeAsPercent))
                            .GetValue(configurationManagerAttributes) ?? false;
                        ConfigurationAttributes.Advanced = (bool?)configurationManagerAttributesFieldInfos
                            .Single(x => x.Name == nameof(ConfigurationManagerAttributes.IsAdvanced))
                            .GetValue(configurationManagerAttributes) ?? false;
                        ConfigurationAttributes.ReadOnly = (bool?)configurationManagerAttributesFieldInfos
                            .Single(x => x.Name == nameof(ConfigurationManagerAttributes.ReadOnly))
                            .GetValue(configurationManagerAttributes) ?? false;
                        ConfigurationAttributes.CustomToString =
                            (Func<object, string>)configurationManagerAttributesFieldInfos
                                .Single(x => x.Name == nameof(ConfigurationManagerAttributes.ObjToStr))
                                .GetValue(configurationManagerAttributes);
                        ConfigurationAttributes.CustomToObject =
                            (Func<string, object>)configurationManagerAttributesFieldInfos
                                .Single(x => x.Name == nameof(ConfigurationManagerAttributes.StrToObj))
                                .GetValue(configurationManagerAttributes);
                    }
                }
                else
                {
                    ConfigurationAttributes.Advanced = true;
                }
            }

            public object GetValue()
            {
                return _configEntryBase.BoxedValue;
            }

            public void SetValue(object value)
            {
                _configEntryBase.BoxedValue = value;
            }
        }

        public class SettingsData
        {
            public readonly ConfigEntry<KeyboardShortcut> KeyConfigurationShortcut;

            public readonly ConfigEntry<Vector2> KeyDefaultPosition;
            public readonly ConfigEntry<Vector2> KeyDescriptionPositionOffset;

            public readonly ConfigEntry<CustomLocalizedHelper.Language> KeyLanguage;

            public readonly ConfigEntry<string> KeySearch;

            public readonly ConfigEntry<bool> KeyAdvanced;

            public readonly ConfigEntry<int> KeySortingOrder;

            public SettingsData(ConfigFile configFile)
            {
                const string mainSettings = "Main Settings";

                KeyConfigurationShortcut = configFile.Bind<KeyboardShortcut>(mainSettings, "Configuration Shortcut",
                    new KeyboardShortcut(KeyCode.Home));
                KeyDefaultPosition = configFile.Bind<Vector2>(mainSettings, "Default Position", Vector2.zero);
                KeyDescriptionPositionOffset = configFile.Bind<Vector2>(mainSettings, "Description Position Offset",
                    new Vector2(0, -50), new ConfigDescription("Description position offset from Mouse position"));
                KeyLanguage = configFile.Bind<CustomLocalizedHelper.Language>(mainSettings, "Language",
                    CustomLocalizedHelper.Language.En,
                    new ConfigDescription(
                        "Preferred language, if not available will tried English, if still not available than return original text"));
                KeySearch = configFile.Bind<string>(mainSettings, "Search", string.Empty);
                KeyAdvanced = configFile.Bind<bool>(mainSettings, "Advanced", false);
                KeySortingOrder = configFile.Bind<int>(mainSettings, "Sorting Order", 29999);
            }
        }
    }
}
#endif