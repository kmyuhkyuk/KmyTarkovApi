#if !UNITY_EDITOR

using System;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;
using EFTConfiguration.Attributes;

// ReSharper disable MemberCanBePrivate.Global

namespace EFTConfiguration.Models
{
    internal class ConfigModel
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

        public ConfigModel(ConfigDefinition configDefinition, ConfigEntryBase configEntryBase, bool isCore)
        {
            _configDefinition = configDefinition;
            _configEntryBase = configEntryBase;

            if (isCore)
            {
                ConfigurationAttributes.Advanced = true;
                return;
            }

            object eftConfigurationExternalAttributes = null;
            var eftConfigurationExternalAttributesFieldInfos = Array.Empty<FieldInfo>();

            object configurationManagerExternalAttributes = null;
            var configurationManagerExternalAttributesFieldInfos = Array.Empty<FieldInfo>();

            foreach (var tag in ConfigDescription.Tags)
            {
                var tagType = tag.GetType();

                switch (tagType.Name)
                {
                    case nameof(EFTConfigurationAttributes):
                    {
                        eftConfigurationExternalAttributes = tag;
                        eftConfigurationExternalAttributesFieldInfos = tagType.GetFields();

                        break;
                    }
                    case nameof(ConfigurationManagerAttributes):
                    {
                        configurationManagerExternalAttributes = tag;
                        configurationManagerExternalAttributesFieldInfos = tagType.GetFields();

                        break;
                    }
                }
            }

            if (eftConfigurationExternalAttributes != null)
            {
                foreach (var eftConfigurationAttributes in
                         EFTConfigurationAttributesFields)
                {
                    var eftConfigurationExternalAttributesFieldInfo =
                        eftConfigurationExternalAttributesFieldInfos.SingleOrDefault(x =>
                            x.Name == eftConfigurationAttributes.Name &&
                            x.FieldType == eftConfigurationAttributes.FieldType);

                    if (eftConfigurationExternalAttributesFieldInfo == null)
                        continue;

                    eftConfigurationAttributes.SetValue(ConfigurationAttributes,
                        eftConfigurationExternalAttributesFieldInfo.GetValue(
                            eftConfigurationExternalAttributes));
                }
            }
            else if (configurationManagerExternalAttributes != null)
            {
                ConfigurationAttributes.HideSetting = !(bool?)configurationManagerExternalAttributesFieldInfos
                    .SingleOrDefault(x => x.Name == nameof(ConfigurationManagerAttributes.Browsable))
                    ?.GetValue(configurationManagerExternalAttributes) ?? false;
                ConfigurationAttributes.HideReset = (bool?)configurationManagerExternalAttributesFieldInfos
                    .SingleOrDefault(x =>
                        x.Name == nameof(ConfigurationManagerAttributes.HideDefaultButton))
                    ?.GetValue(configurationManagerExternalAttributes) ?? false;
                ConfigurationAttributes.HideRange = (bool?)configurationManagerExternalAttributesFieldInfos
                    .SingleOrDefault(x =>
                        x.Name == nameof(ConfigurationManagerAttributes.ShowRangeAsPercent))
                    ?.GetValue(configurationManagerExternalAttributes) ?? false;
                ConfigurationAttributes.Advanced = (bool?)configurationManagerExternalAttributesFieldInfos
                    .SingleOrDefault(x => x.Name == nameof(ConfigurationManagerAttributes.IsAdvanced))
                    ?.GetValue(configurationManagerExternalAttributes) ?? false;
                ConfigurationAttributes.ReadOnly = (bool?)configurationManagerExternalAttributesFieldInfos
                    .SingleOrDefault(x => x.Name == nameof(ConfigurationManagerAttributes.ReadOnly))
                    ?.GetValue(configurationManagerExternalAttributes) ?? false;
                ConfigurationAttributes.CustomToString =
                    (Func<object, string>)configurationManagerExternalAttributesFieldInfos
                        .SingleOrDefault(x => x.Name == nameof(ConfigurationManagerAttributes.ObjToStr))
                        ?.GetValue(configurationManagerExternalAttributes);
                ConfigurationAttributes.CustomToObject =
                    (Func<string, object>)configurationManagerExternalAttributesFieldInfos
                        .SingleOrDefault(x => x.Name == nameof(ConfigurationManagerAttributes.StrToObj))
                        ?.GetValue(configurationManagerExternalAttributes);
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
}

#endif