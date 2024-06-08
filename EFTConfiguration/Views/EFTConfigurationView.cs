using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if !UNITY_EDITOR
using EFTConfiguration.Helpers;
using EFTConfiguration.Models;

#endif

namespace EFTConfiguration.Views
{
    public class EFTConfigurationView : MonoBehaviour
    {
#if !UNITY_EDITOR

        private readonly Dictionary<PluginInfo, Config> _configuration = new Dictionary<PluginInfo, Config>();

        private KeyValuePair<PluginInfo, Config> _currentConfiguration;

        private string CurrentModName => _currentConfiguration.Key.modName;

#endif

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

        [SerializeField] private Description description;

        private Transform _searchPanelTransform;

        private RectTransform _windowRect;

        internal static Action<string> EnableDescription;

        internal static Action DisableDescription;

        private bool State
        {
            get => windowRoot.gameObject.activeSelf;
            set
            {
                if (State == value)
                    return;

#if !UNITY_EDITOR

                if (value)
                {
                    _windowRect.anchoredPosition = SettingsModel.Instance.KeyDefaultPosition.Value;

                    CheckPluginInfo(advanced.isOn);
                }

#endif

                windowRoot.gameObject.SetActive(value);
            }
        }

        internal static Action<PluginInfo> SwitchPluginInfo;

#if !UNITY_EDITOR

        private void Awake()
        {
            var settingsModel = SettingsModel.Instance;

            _windowRect = windowRoot.GetComponent<RectTransform>();

            _searchPanelTransform = search.transform.parent;

            search.text = settingsModel.KeySearch.Value;
            advanced.isOn = settingsModel.KeyAdvanced.Value;

            settingsModel.KeySearch.SettingChanged += (value1, value2) =>
                search.text = settingsModel.KeySearch.Value;
            settingsModel.KeyAdvanced.SettingChanged += (value1, value2) =>
                advanced.isOn = settingsModel.KeyAdvanced.Value;

            search.onValueChanged.AddListener(value =>
            {
                settingsModel.KeySearch.Value = value;
                Filter(advanced.isOn, value);
            });

            advanced.onValueChanged.AddListener(value =>
            {
                settingsModel.KeyAdvanced.Value = value;
                Filter(value, search.text);
            });

            searchButton.onClick.AddListener(() =>
            {
                var searchPanel = _searchPanelTransform.gameObject;

                searchPanel.SetActive(!searchPanel.activeSelf);
            });

            closeButton.onClick.AddListener(() => State = false);

            SwitchPluginInfo = SwitchConfiguration;

            EnableDescription = description.Enable;
            DisableDescription = description.Disable;

            LocalizedHelper.LanguageChange += UpdateLocalized;

            EFTConfigurationModel.Instance.CreateUI = CreateUI;
        }

        private void Update()
        {
            if (SettingsModel.Instance.KeyConfigurationShortcut.Value.IsDown())
            {
                State = !State;
            }
        }

        private void CreateUI()
        {
            var eftConfigurationModel = EFTConfigurationModel.Instance;

            foreach (var configuration in eftConfigurationModel.Configurations)
            {
                var pluginInfo = Instantiate(eftConfigurationModel.PrefabManager.pluginInfo, pluginInfosRoot)
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

                var config = Instantiate(eftConfigurationModel.PrefabManager.config, configsRoot)
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
            var doc = await CrawlerHelper.CreateHtmlDocument(modURL);

            if (doc == null)
                return;

            var url = CrawlerHelper.GetModDownloadURL(doc);

            if (!string.IsNullOrEmpty(url))
            {
                bindURL(url);
            }

            var downloadCount = CrawlerHelper.GetModDownloadCount(doc);

            if (downloadCount > -1)
            {
                bindDownloads(downloadCount);
            }

            var version = CrawlerHelper.GetModVersion(doc);

            if (version != null)
            {
                bindVersion(version);
            }

            var icon = await CrawlerHelper.GetModIcon(doc, modURL);

            if (icon != null)
            {
                bindIcon(icon);
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
            var eftConfigurationModel = EFTConfigurationModel.Instance;

            modName.text = LocalizedHelper.Localized(CurrentModName);

            ((TMP_Text)search.placeholder).text =
                LocalizedHelper.Localized(eftConfigurationModel.ModName, "EnterText");

            advancedName.text = LocalizedHelper.Localized(eftConfigurationModel.ModName, "Advanced");
        }

        private void Filter(bool isAdvanced, string searchName)
        {
            CheckPluginInfo(isAdvanced);
            _currentConfiguration.Value.Filter(isAdvanced, searchName);
        }

        private void CheckPluginInfo(bool isAdvanced)
        {
            var eftConfigurationModel = EFTConfigurationModel.Instance;

            var configurations = _configuration.ToArray();

            var needShowList = new List<bool>();
            foreach (var configurationData in eftConfigurationModel.Configurations)
            {
                var attributes = configurationData.ConfigurationPluginAttributes;

                var show = !(attributes.HidePlugin || !attributes.AlwaysDisplay && configurationData.ConfigCount == 0);

                if (configurationData.Configs.All(x =>
                        x.ConfigurationAttributes.HideSetting || !isAdvanced && x.ConfigurationAttributes.Advanced))
                {
                    show = false;
                }

                needShowList.Add(show);
            }

            for (var i = 0; i < needShowList.Count; i++)
            {
                var needShow = needShowList[i];
                var configuration = configurations[i];

                if (!needShow && configuration.Key == _currentConfiguration.Key)
                {
                    var fistIndex = i == needShowList.Count - 1
                        ? needShowList.FindLastIndex(x => x)
                        : needShowList.FindIndex(x => x);

                    if (fistIndex > -1)
                    {
                        SwitchConfiguration(configurations[fistIndex], true);
                    }
                }

                configuration.Key.gameObject.SetActive(needShow);
            }
        }

#endif
    }
}