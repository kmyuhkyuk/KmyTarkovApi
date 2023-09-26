using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using EFTConfiguration.Components;
using EFTConfiguration.Components.Base;
using EFTConfiguration.Components.ClassType;
using EFTConfiguration.Components.ValueType;
using UnityEngine;
#if !UNITY_EDITOR
using HarmonyLib;
using static EFTConfiguration.EFTConfigurationPlugin;

#endif

namespace EFTConfiguration.UI
{
    public class Config : MonoBehaviour
    {
        public bool State
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        private readonly Dictionary<string, List<ConfigBase>> _configDictionary =
            new Dictionary<string, List<ConfigBase>>();

        private bool _advanced;

        private string _searchName;

#if !UNITY_EDITOR

        private ConfigurationData _configurationData;

        private string ModName => _configurationData.ModName;

        public void Init(ConfigurationData configurationData)
        {
            _configurationData = configurationData;
        }

        public void Filter(bool advanced, string searchName)
        {
            _advanced = advanced;
            _searchName = searchName;

            var hasName = !string.IsNullOrEmpty(searchName);

            foreach (var config in _configDictionary)
            {
                var key = config.Key;

                var valueList = config.Value;

                var count = valueList.Count;

                foreach (var configBase in valueList)
                {
                    var hasSearchName =
                        hasName && configBase.LocalizedName.IndexOf(searchName, StringComparison.OrdinalIgnoreCase) >
                        -1 || !hasName;

                    var active = configBase.IsAdvanced ? advanced && hasSearchName : hasSearchName;

                    configBase.gameObject.SetActive(active);

                    if (!active)
                    {
                        count--;
                    }
                }

                if (!string.IsNullOrEmpty(key) && count == 1)
                {
                    valueList[0].gameObject.SetActive(false);
                }
            }
        }

        private void UpdateConfigs()
        {
            foreach (var config in _configDictionary.Values.SelectMany(x => x).OfType<ConfigReset>())
            {
                config.UpdateCurrentValue();
            }
        }

        private void OnEnable()
        {
            var sectionConfigData = new Dictionary<string, List<ConfigData>>();

            foreach (var config in _configurationData.Configs)
            {
                if (config.ConfigurationAttributes.HideSetting)
                    continue;

                if (sectionConfigData.TryGetValue(config.Section, out var list))
                {
                    list.Add(config);
                }
                else
                {
                    sectionConfigData.Add(config.Section, new List<ConfigData> { config });
                }
            }

            foreach (var configData in sectionConfigData)
            {
                if (configData.Value.Count == 0)
                    continue;

                if (!string.IsNullOrEmpty(configData.Key))
                {
                    var configHeader = Instantiate(EFTConfigurationPlugin.PrefabManager.header, transform)
                        .GetComponent<ConfigHeader>();

                    configHeader.Init(ModName, configData.Key, string.Empty, false);

                    AddConfig(configData.Key, configHeader);
                }

                foreach (var config in configData.Value)
                {
                    AddConfig(configData.Key, config);
                }
            }

            Filter(_advanced, _searchName);

            _configurationData.SettingChanged = UpdateConfigs;
        }

        private void AddConfig(string section, ConfigData configData)
        {
            var type = configData.SettingType;

            var attributes = configData.ConfigurationAttributes;

            if (type == typeof(bool))
            {
                var configToggle = Instantiate(EFTConfigurationPlugin.PrefabManager.toggle, transform)
                    .GetComponent<ConfigToggle>();

                configToggle.Init(ModName, configData.Key, configData.Description, attributes.Advanced,
                    attributes.ReadOnly, configData.DefaultValue, configData.SetValue, attributes.HideReset,
                    configData.GetValue);

                AddConfig(section, configToggle);
            }
            else if (type == typeof(string))
            {
                if (attributes.ButtonAction != null)
                {
                    var configStringAction = Instantiate(EFTConfigurationPlugin.PrefabManager.stringAction, transform)
                        .GetComponent<ConfigStringAction>();

                    configStringAction.Init(ModName, configData.Key, configData.Description, attributes.Advanced,
                        attributes.ButtonAction);

                    AddConfig(section, configStringAction);
                }
                else
                {
                    if (configData.AcceptableValueBase is AcceptableValueList<string> acceptableValueList)
                    {
                        var configStringDropdown =
                            Instantiate(EFTConfigurationPlugin.PrefabManager.stringDropdown, transform)
                                .GetComponent<ConfigStringDropdown>();

                        configStringDropdown.Init(ModName, configData.Key, configData.Description, attributes.Advanced,
                            attributes.ReadOnly, configData.DefaultValue, configData.SetValue, attributes.HideReset,
                            configData.GetValue, acceptableValueList.AcceptableValues);

                        AddConfig(section, configStringDropdown);
                    }
                    else
                    {
                        var configString = Instantiate(EFTConfigurationPlugin.PrefabManager.@string, transform)
                            .GetComponent<ConfigString>();

                        configString.Init(ModName, configData.Key, configData.Description, attributes.Advanced,
                            attributes.ReadOnly, configData.DefaultValue, configData.SetValue, attributes.HideReset,
                            configData.GetValue);

                        AddConfig(section, configString);
                    }
                }
            }
            else if (configData.AcceptableValueBase != null &&
                     configData.AcceptableValueBase.GetType().GetGenericTypeDefinition() ==
                     typeof(AcceptableValueList<>))
            {
                var configEnum = Instantiate(EFTConfigurationPlugin.PrefabManager.@enum, transform)
                    .GetComponent<ConfigEnum>();

                configEnum.Init(ModName, configData.Key, configData.Description, attributes.Advanced,
                    attributes.ReadOnly, configData.DefaultValue, configData.SetValue, attributes.HideReset,
                    configData.GetValue,
                    Traverse.Create(configData.AcceptableValueBase).Property("AcceptableValues").GetValue<Array>());

                AddConfig(section, configEnum);
            }
            else if (type == typeof(int))
            {
                if (configData.AcceptableValueBase != null &&
                    configData.AcceptableValueBase.GetType().GetGenericTypeDefinition() ==
                    typeof(AcceptableValueRange<>) && !attributes.HideRange)
                {
                    var configIntSlider = Instantiate(EFTConfigurationPlugin.PrefabManager.intSlider, transform)
                        .GetComponent<ConfigIntSlider>();

                    configIntSlider.Init(ModName, configData.Key, configData.Description, attributes.Advanced,
                        attributes.ReadOnly, configData.DefaultValue, configData.SetValue, attributes.HideReset,
                        configData.GetValue,
                        Traverse.Create(configData.AcceptableValueBase).Property("MinValue").GetValue<int>(),
                        Traverse.Create(configData.AcceptableValueBase).Property("MaxValue").GetValue<int>());

                    AddConfig(section, configIntSlider);
                }
                else
                {
                    var configInt = Instantiate(EFTConfigurationPlugin.PrefabManager.@int, transform)
                        .GetComponent<ConfigInt>();

                    configInt.Init(ModName, configData.Key, configData.Description, attributes.Advanced,
                        attributes.ReadOnly, configData.DefaultValue, configData.SetValue, attributes.HideReset,
                        configData.GetValue);

                    AddConfig(section, configInt);
                }
            }
            else if (type == typeof(float))
            {
                if (configData.AcceptableValueBase != null &&
                    configData.AcceptableValueBase.GetType().GetGenericTypeDefinition() ==
                    typeof(AcceptableValueRange<>) && !attributes.HideRange)
                {
                    var configFloatSlider = Instantiate(EFTConfigurationPlugin.PrefabManager.floatSlider, transform)
                        .GetComponent<ConfigFloatSlider>();

                    configFloatSlider.Init(ModName, configData.Key, configData.Description, attributes.Advanced,
                        attributes.ReadOnly, configData.DefaultValue, configData.SetValue, attributes.HideReset,
                        configData.GetValue,
                        Traverse.Create(configData.AcceptableValueBase).Property("MinValue").GetValue<float>(),
                        Traverse.Create(configData.AcceptableValueBase).Property("MaxValue").GetValue<float>());

                    AddConfig(section, configFloatSlider);
                }
                else
                {
                    var configFloat = Instantiate(EFTConfigurationPlugin.PrefabManager.@float, transform)
                        .GetComponent<ConfigFloat>();

                    configFloat.Init(ModName, configData.Key, configData.Description, attributes.Advanced,
                        attributes.ReadOnly, configData.DefaultValue, configData.SetValue, attributes.HideReset,
                        configData.GetValue);

                    AddConfig(section, configFloat);
                }
            }
            else if (type == typeof(Vector2))
            {
                var configVector2 = Instantiate(EFTConfigurationPlugin.PrefabManager.vector2, transform)
                    .GetComponent<ConfigVector2>();

                configVector2.Init(ModName, configData.Key, configData.Description, attributes.Advanced,
                    attributes.ReadOnly, configData.DefaultValue, configData.SetValue, attributes.HideReset,
                    configData.GetValue);

                AddConfig(section, configVector2);
            }
            else if (type == typeof(Vector3))
            {
                var configVector3 = Instantiate(EFTConfigurationPlugin.PrefabManager.vector3, transform)
                    .GetComponent<ConfigVector3>();

                configVector3.Init(ModName, configData.Key, configData.Description, attributes.Advanced,
                    attributes.ReadOnly, configData.DefaultValue, configData.SetValue, attributes.HideReset,
                    configData.GetValue);

                AddConfig(section, configVector3);
            }
            else if (type == typeof(Vector4))
            {
                var configVector4 = Instantiate(EFTConfigurationPlugin.PrefabManager.vector4, transform)
                    .GetComponent<ConfigVector4>();

                configVector4.Init(ModName, configData.Key, configData.Description, attributes.Advanced,
                    attributes.ReadOnly, configData.DefaultValue, configData.SetValue, attributes.HideReset,
                    configData.GetValue);

                AddConfig(section, configVector4);
            }
            else if (type == typeof(Quaternion))
            {
                var configQuaternion = Instantiate(EFTConfigurationPlugin.PrefabManager.quaternion, transform)
                    .GetComponent<ConfigQuaternion>();

                configQuaternion.Init(ModName, configData.Key, configData.Description, attributes.Advanced,
                    attributes.ReadOnly, configData.DefaultValue, configData.SetValue, attributes.HideReset,
                    configData.GetValue);

                AddConfig(section, configQuaternion);
            }
            else if (type == typeof(Color))
            {
                var configColor = Instantiate(EFTConfigurationPlugin.PrefabManager.color, transform)
                    .GetComponent<ConfigColor>();

                configColor.Init(ModName, configData.Key, configData.Description, attributes.Advanced,
                    attributes.ReadOnly, configData.DefaultValue, configData.SetValue, attributes.HideReset,
                    configData.GetValue);

                AddConfig(section, configColor);
            }
            else if (type == typeof(KeyboardShortcut))
            {
                var configKeyboardShortcut =
                    Instantiate(EFTConfigurationPlugin.PrefabManager.keyboardShortcut, transform)
                        .GetComponent<ConfigKeyboardShortcut>();

                configKeyboardShortcut.Init(ModName, configData.Key, configData.Description, attributes.Advanced,
                    attributes.ReadOnly, configData.DefaultValue, configData.SetValue, attributes.HideReset,
                    configData.GetValue);

                AddConfig(section, configKeyboardShortcut);
            }
            else if (type.IsEnum)
            {
                var configEnum = Instantiate(EFTConfigurationPlugin.PrefabManager.@enum, transform)
                    .GetComponent<ConfigEnum>();

                configEnum.Init(ModName, configData.Key, configData.Description, attributes.Advanced,
                    attributes.ReadOnly, configData.DefaultValue, configData.SetValue, attributes.HideReset,
                    configData.GetValue, Enum.GetValues(type));

                AddConfig(section, configEnum);
            }
            else if (type.IsPrimitive)
            {
                var configUnknown = Instantiate(EFTConfigurationPlugin.PrefabManager.unknown, transform)
                    .GetComponent<ConfigUnknown>();

                configUnknown.Init(ModName, configData.Key, configData.Description, attributes.Advanced,
                    attributes.ReadOnly, configData.DefaultValue, configData.SetValue, attributes.HideReset,
                    configData.GetValue, type);

                AddConfig(section, configUnknown);
            }
            else if (attributes.CustomToString != null && attributes.CustomToObject != null)
            {
                var configUnknownCustom = Instantiate(EFTConfigurationPlugin.PrefabManager.unknownCustom, transform)
                    .GetComponent<ConfigUnknownCustom>();

                configUnknownCustom.Init(ModName, configData.Key, configData.Description, attributes.Advanced,
                    attributes.ReadOnly, configData.DefaultValue, configData.SetValue, attributes.HideReset,
                    configData.GetValue, attributes.CustomToString, attributes.CustomToObject);

                AddConfig(section, configUnknownCustom);
            }
        }

        private void AddConfig(string section, ConfigBase config)
        {
            if (_configDictionary.TryGetValue(section, out var list))
            {
                list.Add(config);
            }
            else
            {
                _configDictionary.Add(section, new List<ConfigBase> { config });
            }
        }

        private void OnDisable()
        {
            _configurationData.SettingChanged = null;

            var childArray = GetComponentsInChildren<Transform>(true);
            for (var i = 1; i < childArray.Length; i++)
            {
                var child = childArray[i];

                if (child.transform.parent != transform)
                    continue;

                Destroy(child.gameObject);
            }

            _configDictionary.Clear();
        }

#endif
    }
}