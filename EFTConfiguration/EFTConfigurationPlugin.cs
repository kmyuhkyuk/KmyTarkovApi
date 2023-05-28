#if !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using EFTConfiguration.Attributes;
using EFTConfiguration.Helpers;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using PluginInfo = BepInEx.PluginInfo;

namespace EFTConfiguration
{
    [BepInPlugin("com.kmyuhkyuk.EFTConfiguration", "kmyuhkyuk-EFTConfiguration", "1.0.0")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud")]
    public class EFTConfigurationPlugin : BaseUnityPlugin
    {
        private static Dictionary<string, PluginInfo> PluginInfos => Chainloader.PluginInfos;

        private readonly GameObject _eftConfigurationPublic = new GameObject("EFTConfigurationPublic", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));

        private readonly PropertyInfo _coreConfigInfo = typeof(ConfigFile).GetProperty("CoreConfig", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

        private readonly FieldInfo[] _eftConfigurationPluginAttributesFields = typeof(EFTConfigurationPluginAttributes).GetFields();

        private readonly (string, Type)[] _eftConfigurationPluginAttributesFieldsTuple = typeof(EFTConfigurationPluginAttributes).GetFields().Select(x => (x.Name, x.FieldType)).ToArray();

        internal static readonly SettingsData SetData = new SettingsData();

        internal const string ModName = "kmyuhkyuk-EFTConfiguration";

        internal static PrefabManager PrefabManager { get; private set; }

        internal static readonly string ModPath = Path.Combine(Paths.PluginPath, "kmyuhkyuk-EFTApi");

        internal static Action<ConfigurationData[]> ShowUI;

        internal static Action UISwitch;

        private readonly Canvas _canvas;

        public EFTConfigurationPlugin()
        {
            _canvas = _eftConfigurationPublic.GetComponent<Canvas>();

            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;

            DontDestroyOnLoad(_eftConfigurationPublic);
        }

        private void Start()
        {
            const string mainSettings = "Main Settings";

            SetData.KeyConfigurationShortcut = Config.Bind<KeyboardShortcut>(mainSettings, "Configuration Shortcut", new KeyboardShortcut(KeyCode.Home));
            SetData.KeyDefaultPosition = Config.Bind<Vector2>(mainSettings, "Default Position", Vector2.zero);
            SetData.KeyDescriptionPositionOffset = Config.Bind<Vector2>(mainSettings, "Description Position Offset", new Vector2(0, -50), new ConfigDescription("Description position offset from Mouse position"));
            SetData.KeyLanguage = Config.Bind<CustomLocalizedHelper.Language>(mainSettings, "Language", CustomLocalizedHelper.Language.En, new ConfigDescription("Preferred language, if not available will tried English, if still not available than return original text"));
            SetData.KeySearch = Config.Bind<string>(mainSettings, "Search", string.Empty);
            SetData.KeyAdvanced = Config.Bind<bool>(mainSettings, "Advanced", false);
            SetData.KeySortingOrder = Config.Bind<int>(mainSettings, "Sorting Order", 29999);

            //Test
            /*const string testSettings = "Test Settings";
            var eftConfigurationAttributes = new EFTConfigurationAttributes { Advanced = true };

            Config.Bind<bool>(testSettings, "Bool", false, new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind<int>(testSettings, "Int", 0, new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind<int>(testSettings, "Int Slider", 0, new ConfigDescription(string.Empty, new AcceptableValueRange<int>(0, 100), eftConfigurationAttributes));
            Config.Bind<float>(testSettings, "Float", 0f, new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind<float>(testSettings, "Float Slider", 0f, new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0f, 100f), eftConfigurationAttributes));
            Config.Bind<string>(testSettings, "String", string.Empty, new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind<string>(testSettings, "String Dropdown", string.Empty, new ConfigDescription("123", new AcceptableValueList<string>("123", "234"), eftConfigurationAttributes));
            Config.Bind<Vector2>(testSettings, "Vector2", Vector2.zero, new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind<Vector3>(testSettings, "Vector3", Vector3.zero, new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind<Vector4>(testSettings, "Vector4", Vector4.zero, new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind<Quaternion>(testSettings, "Quaternion", Quaternion.identity, new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind<Color>(testSettings, "Color", Color.white, new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind<CustomLocalizedHelper.Language>(testSettings, "Enum", CustomLocalizedHelper.Language.En, new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind<KeyboardShortcut>(testSettings, "KeyboardShortcut", KeyboardShortcut.Empty, new ConfigDescription(string.Empty, null, eftConfigurationAttributes));

            Config.Bind<double>(testSettings, "Double", 0d, new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind(testSettings, "Action", string.Empty, new ConfigDescription(string.Empty, null, new EFTConfigurationAttributes { Advanced = true, ButtonAction = () => Logger.LogError("Work")}));*/

            _canvas.sortingOrder = SetData.KeySortingOrder.Value;
            SetData.KeySortingOrder.SettingChanged += (value, value2) => _canvas.sortingOrder = SetData.KeySortingOrder.Value;

            CustomLocalizedHelper.CurrentLanguage = SetData.KeyLanguage.Value;
            SetData.KeyLanguage.SettingChanged += (value, value2) => CustomLocalizedHelper.CurrentLanguage = SetData.KeyLanguage.Value;

            Init();
        }

        private void Awake()
        {
            var assetBundle = AssetBundle.LoadFromFile(Path.Combine(ModPath, "bundles", "eftconfiguration.bundle"));

            if (assetBundle == null)
            {
                Logger.LogError(nameof(EFTConfigurationPlugin) + ": " + "Failed to load AssetBundle!");
            }
            else
            {
                var prefabManager = assetBundle.LoadAllAssets<PrefabManager>()[0];

                PrefabManager = prefabManager;

                Instantiate(prefabManager.eftConfiguration, _eftConfigurationPublic.transform);

                assetBundle.Unload(false);
            }
        }

        private async void Init()
        {
            while (PluginInfos.Count == 0)
                await Task.Yield();

            var configurationList = new List<ConfigurationData> { GetCoreConfigurationData(), GetConfigurationData(Info) };

            foreach (var pluginInfo in PluginInfos.Values)
            {
                if (pluginInfo == Info)
                    continue;

                configurationList.Add(GetConfigurationData(pluginInfo));
            }

            while (ShowUI == null)
                await Task.Yield(); 
            
            ShowUI(configurationList.ToArray());
        }

        private void Update()
        {
            if (SetData.KeyConfigurationShortcut.Value.IsDown())
            {
                UISwitch();
            }
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

                    if (attributeFieldInfos.Select(x => (x.Name, x.FieldType)).SequenceEqual(_eftConfigurationPluginAttributesFieldsTuple))
                    {
                        hasAttributes = true;

                        for (var i = 0; i < _eftConfigurationPluginAttributesFields.Length; i++)
                        {
                            var eftConfigurationPluginAttributesField = _eftConfigurationPluginAttributesFields[i];

                            var attributeFieldInfo = attributeFieldInfos[i];

                            eftConfigurationPluginAttributesField.SetValue(attributes, attributeFieldInfo.GetValue(attribute));
                        }

                        break;
                    }
                }
            }

            if (!hasAttributes)
            {
                attributes.ModUrl = FileVersionInfo.GetVersionInfo(pluginInfo.Location).CompanyName;
                attributes.HidePlugin = type.GetCustomAttributes<BrowsableAttribute>().Any(x => !x.Browsable);
            }

            var configFile = instance.Config;

            var metaData = pluginInfo.Metadata;

            CustomLocalizedHelper.LanguageList.Add(metaData.Name, GetLanguageDictionary(pluginInfo, hasAttributes ? attributes.LocalizedPath : string.Empty));

            return new ConfigurationData(configFile, metaData, attributes);
        }

        private ConfigurationData GetCoreConfigurationData()
        {
            var configFile = (ConfigFile)_coreConfigInfo.GetValue(null);

            var metaData = new BepInPlugin("BepInEx", "BepInEx", typeof(BaseUnityPlugin).Assembly.GetName().Version.ToString());

            return new ConfigurationData(configFile, metaData, new EFTConfigurationPluginAttributes(string.Empty), true);
        }

        private static Dictionary<string, Dictionary<string, string>> GetLanguageDictionary(PluginInfo pluginInfo, string localizedPath)
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
                    localizedDictionary.Add(Path.GetFileNameWithoutExtension(localized.Name), JsonConvert.DeserializeObject<Dictionary<string, string>>(stream.ReadToEnd()));
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

            public ConfigurationData(ConfigFile configFile, BepInPlugin metadata, EFTConfigurationPluginAttributes configurationPlugin, bool isCore = false)
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

            public readonly EFTConfigurationAttributes ConfigurationAttributes;

            private readonly ConfigDefinition _configDefinition;

            private readonly ConfigEntryBase _configEntryBase;

            private static readonly FieldInfo[] EFTConfigurationAttributesFields = typeof(EFTConfigurationAttributes).GetFields();

            private static readonly (string, Type)[] EFTConfigurationAttributesFieldsTuple = typeof(EFTConfigurationAttributes).GetFields().Select(x => (x.Name, x.FieldType)).ToArray();

            private static readonly FieldInfo[] ConfigurationManagerAttributesFields = typeof(ConfigurationManagerAttributes).GetFields();

            private static readonly (string, Type)[] ConfigurationManagerAttributesFieldsTuple = typeof(ConfigurationManagerAttributes).GetFields().Select(x => (x.Name, x.FieldType)).ToArray();

            public ConfigData(ConfigDefinition configDefinition, ConfigEntryBase configEntryBase, bool isCore)
            {
                _configDefinition = configDefinition;
                _configEntryBase = configEntryBase;

                ConfigurationAttributes = new EFTConfigurationAttributes();

                if (!isCore)
                {
                    foreach (var tag in ConfigDescription.Tags)
                    {
                        var tagType = tag.GetType();

                        var hasTag = false;
                        switch (tagType.Name)
                        {
                            case nameof(EFTConfigurationAttributes):
                            {
                                var tagFieldInfos = tagType.GetFields();

                                if (tagFieldInfos.Select(x => (x.Name, x.FieldType)).SequenceEqual(EFTConfigurationAttributesFieldsTuple))
                                {
                                    hasTag = true;

                                    for (var i = 0; i < EFTConfigurationAttributesFields.Length; i++)
                                    {
                                        var configurationAttributesFields = EFTConfigurationAttributesFields[i];

                                        var tagFieldInfo = tagFieldInfos[i];

                                        configurationAttributesFields.SetValue(ConfigurationAttributes, tagFieldInfo.GetValue(tag));
                                    }
                                }
                                break;
                            }
                            case nameof(ConfigurationManagerAttributes):
                            {
                                var tagFieldInfos = tagType.GetFields();

                                if (tagFieldInfos.Select(x => (x.Name, x.FieldType)).SequenceEqual(ConfigurationManagerAttributesFieldsTuple))
                                {
                                    hasTag = true;

                                    ConfigurationAttributes.HideSetting = (bool?)tagFieldInfos.Single(x => x.Name == nameof(ConfigurationManagerAttributes.Browsable)).GetValue(tag) ?? false;
                                    ConfigurationAttributes.HideRest = (bool?)tagFieldInfos.Single(x => x.Name == nameof(ConfigurationManagerAttributes.HideDefaultButton)).GetValue(tag) ?? false;
                                    ConfigurationAttributes.HideRange = (bool?)tagFieldInfos.Single(x => x.Name == nameof(ConfigurationManagerAttributes.ShowRangeAsPercent)).GetValue(tag) ?? false;
                                    ConfigurationAttributes.Advanced = (bool?)tagFieldInfos.Single(x => x.Name == nameof(ConfigurationManagerAttributes.IsAdvanced)).GetValue(tag) ?? false;
                                    ConfigurationAttributes.ReadOnly = (bool?)tagFieldInfos.Single(x => x.Name == nameof(ConfigurationManagerAttributes.ReadOnly)).GetValue(tag) ?? false;
                                    ConfigurationAttributes.CustomToString = (Func<object, string>)tagFieldInfos.Single(x => x.Name == nameof(ConfigurationManagerAttributes.ObjToStr)).GetValue(tag);
                                    ConfigurationAttributes.CustomToObject = (Func<string, object>)tagFieldInfos.Single(x => x.Name == nameof(ConfigurationManagerAttributes.StrToObj)).GetValue(tag);
                                }
                                break;
                            }
                        }

                        if (hasTag)
                            break;
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
            public ConfigEntry<KeyboardShortcut> KeyConfigurationShortcut;

            public ConfigEntry<Vector2> KeyDefaultPosition;
            public ConfigEntry<Vector2> KeyDescriptionPositionOffset;

            public ConfigEntry<CustomLocalizedHelper.Language> KeyLanguage;

            public ConfigEntry<string> KeySearch;

            public ConfigEntry<bool> KeyAdvanced;

            public ConfigEntry<int> KeySortingOrder;
        }
    }
}
#endif
