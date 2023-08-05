using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using EFTConfiguration.Helpers;
using EFTConfiguration.UI;
using HtmlAgilityPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EFTConfiguration
{
    public class EFTConfiguration : MonoBehaviour
    {
        private readonly Dictionary<PluginInfo, Config> _configuration = new Dictionary<PluginInfo, Config>();

        private KeyValuePair<PluginInfo, Config> _currentConfiguration;

        private string CurrentModName => _currentConfiguration.Key.modName;

        [SerializeField] private Transform pluginInfosRoot;

        [SerializeField] private Transform configsRoot;

        [SerializeField] private TMP_Text modName;

        [SerializeField] private Button closeButton;

        [SerializeField] private ToggleGroup pluginInfosToggleGroup;

        [SerializeField] private ScrollRect scrollRect;

        [SerializeField] private Transform windowRoot;

        [SerializeField] private TMP_InputField search;

        [SerializeField] private TMP_Text advancedName;

        [SerializeField] private Toggle advanced;

        [SerializeField] private Button searchButton;

        private Transform _searchPanelTransform;

        private RectTransform _windowRect;

#if !UNITY_EDITOR
        private static readonly ManualLogSource LogSource = new ManualLogSource("EFTConfiguration");
#endif

        private bool State
        {
            get => windowRoot.gameObject.activeSelf;
            set
            {
#if !UNITY_EDITOR
                if (value)
                {
                    _windowRect.anchoredPosition = EFTConfigurationPlugin.SetData.KeyDefaultPosition.Value;

                    CheckPluginInfo(advanced.isOn);
                }
#endif

                windowRoot.gameObject.SetActive(value);
            }
        }

        internal static Action<PluginInfo> SwitchPluginInfo;

#if !UNITY_EDITOR
        private static EFTConfigurationPlugin.ConfigurationData[] ConfigurationsData =>
            EFTConfigurationPlugin.Configurations;

        private void Start()
        {
            _windowRect = windowRoot.GetComponent<RectTransform>();

            _searchPanelTransform = search.transform.parent;

            search.text = EFTConfigurationPlugin.SetData.KeySearch.Value;
            advanced.isOn = EFTConfigurationPlugin.SetData.KeyAdvanced.Value;

            EFTConfigurationPlugin.SetData.KeySearch.SettingChanged += (value1, value2) =>
                search.text = EFTConfigurationPlugin.SetData.KeySearch.Value;
            EFTConfigurationPlugin.SetData.KeyAdvanced.SettingChanged += (value1, value2) =>
                advanced.isOn = EFTConfigurationPlugin.SetData.KeyAdvanced.Value;

            search.onValueChanged.AddListener(value =>
            {
                EFTConfigurationPlugin.SetData.KeySearch.Value = value;
                Filter(advanced.isOn, value);
            });

            advanced.onValueChanged.AddListener(value =>
            {
                EFTConfigurationPlugin.SetData.KeyAdvanced.Value = value;
                Filter(value, search.text);
            });

            searchButton.onClick.AddListener(() =>
            {
                var searchPanel = _searchPanelTransform.gameObject;

                searchPanel.SetActive(!searchPanel.activeSelf);
            });

            closeButton.onClick.AddListener(() => State = false);

            SwitchPluginInfo = SwitchConfiguration;

            CustomLocalizedHelper.LanguageChange += UpdateLocalized;

            CreateUI();
        }

        private void Update()
        {
            if (EFTConfigurationPlugin.SetData.KeyConfigurationShortcut.Value.IsDown())
            {
                State = !State;
            }
        }

        private void CreateUI()
        {
            foreach (var configuration in ConfigurationsData)
            {
                var pluginInfo = Instantiate(EFTConfigurationPlugin.PrefabManager.pluginInfo, pluginInfosRoot)
                    .GetComponent<PluginInfo>();

                pluginInfo.isCore = configuration.IsCore;
                pluginInfo.modName = configuration.ModName;

                var modURL = configuration.ConfigurationPluginAttributes.ModURL;

                var hasURL = Uri.IsWellFormedUriString(modURL, UriKind.Absolute);

                pluginInfo.hasModURL = hasURL;
                pluginInfo.modURL = modURL;
                pluginInfo.toggleGroup = pluginInfosToggleGroup;

                pluginInfo.Init(configuration.ModVersion);

                if (hasURL && modURL.StartsWith("https://hub.sp-tarkov.com/files/file"))
                {
                    BindWeb(modURL, pluginInfo.BindIcon, pluginInfo.BindURL, pluginInfo.BindDownloads,
                        pluginInfo.BindVersion);
                }

                var config = Instantiate(EFTConfigurationPlugin.PrefabManager.config, configsRoot)
                    .GetComponent<Config>();

                config.Init(configuration);

                _configuration.Add(pluginInfo, config);
            }

            _currentConfiguration = _configuration.First(x => !x.Key.isCore);

            _currentConfiguration.Key.fistPluginInfo = true;

            _currentConfiguration.Value.State = true;

            UpdateLocalized();
        }

        private static async void BindWeb(string modURL, Action<Sprite> bindIcon, Action<string> bindURL,
            Action<int> bindDownloads, Action<Version> bindVersion)
        {
            HtmlDocument doc;
            try
            {
                var doc = await CrawlerHelper.CreateHtmlDocument(modURL);

                action?.Invoke(icon, downloadUrl, downloads, version);
            }
            catch (Exception e)
            {
                Console.WriteLine($"EFTConfiguration.BindWeb: {e.Message}");
            }
        }

        private void SwitchConfiguration(PluginInfo pluginInfo)
        {
            SwitchConfiguration(_configuration.Single(x => x.Key == pluginInfo), false);
        }

        private void SwitchConfiguration(KeyValuePair<PluginInfo, Config> keyValuePair, bool changeIsOn)
        {
            scrollRect.verticalNormalizedPosition = 1;

            _currentConfiguration.Value.State = false;

            _currentConfiguration = keyValuePair;

            if (changeIsOn)
            {
                keyValuePair.Key.IsOn = true;
            }

            keyValuePair.Value.State = true;

            keyValuePair.Value.Filter(advanced.isOn, search.text);

            UpdateLocalized();
        }

        private void UpdateLocalized()
        {
            modName.text = CustomLocalizedHelper.Localized(CurrentModName);

            ((TMP_Text)search.placeholder).text =
                CustomLocalizedHelper.Localized(EFTConfigurationPlugin.ModName, "EnterText");

            advancedName.text = CustomLocalizedHelper.Localized(EFTConfigurationPlugin.ModName, "Advanced");
        }

        private void Filter(bool isAdvanced, string searchName)
        {
            CheckPluginInfo(isAdvanced);
            _currentConfiguration.Value.Filter(isAdvanced, searchName);
        }

        private void CheckPluginInfo(bool isAdvanced)
        {
            var configurations = _configuration.ToArray();

            var changes = new List<(int Index, bool Active)>();

            for (var i = 0; i < ConfigurationsData.Length; i++)
            {
                var configurationData = ConfigurationsData[i];

                var attributes = configurationData.ConfigurationPluginAttributes;

                var show = !(attributes.HidePlugin || !attributes.AlwaysDisplay && configurationData.ConfigCount == 0);

                if (configurationData.Configs.All(x =>
                        x.ConfigurationAttributes.HideSetting || !isAdvanced && x.ConfigurationAttributes.Advanced))
                    show = false;

                changes.Add((i, show));
            }

            var changesArray = changes.ToArray();

            foreach (var change in changesArray)
            {
                var configuration = configurations[change.Index];

                if (!change.Active && configuration.Key == _currentConfiguration.Key)
                {
                    if (change.Index == 0)
                    {
                        for (var i = 0; i < changesArray.Length; i++)
                        {
                            var changeValue = changesArray[i];

                            if (changeValue.Active)
                            {
                                SwitchConfiguration(configurations[i], true);
                                break;
                            }
                        }
                    }
                    else if (change.Index == changesArray.Length - 1)
                    {
                        for (var i = change.Index; i >= 0; i--)
                        {
                            var changeActive = changesArray[i].Active;

                            if (changeActive)
                            {
                                SwitchConfiguration(configurations[i], true);
                                break;
                            }
                        }
                    }
                    else
                    {
                        var hasChange = false;

                        for (var i = change.Index; i < changesArray.Length; i++)
                        {
                            var changeValue = changesArray[i];

                            if (changeValue.Active)
                            {
                                hasChange = true;

                                SwitchConfiguration(configurations[i], true);
                                break;
                            }
                        }

                        if (!hasChange)
                        {
                            for (var i = change.Index; i >= 0; i--)
                            {
                                var changeActive = changesArray[i].Active;

                                if (changeActive)
                                {
                                    SwitchConfiguration(configurations[i], true);
                                    break;
                                }
                            }
                        }
                    }
                }

                configuration.Key.gameObject.SetActive(change.Active);
            }
        }
#endif
    }
}