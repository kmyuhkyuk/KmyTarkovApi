#if !UNITY_EDITOR

using System;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using KmyTarkovConfiguration.Attributes;

namespace KmyTarkovConfiguration.Models
{
    internal class ConfigurationModel
    {
        public readonly bool IsCore;

        public Version ModVersion => _metadata.Version;

        public string ModName => _metadata.Name;

        public readonly EFTConfigurationPluginAttributes ConfigurationPluginAttributes;

        private readonly BepInPlugin _metadata;

        private readonly ConfigFile _configFile;

        public Action SettingChanged;

        public int ConfigCount => _configFile.Count;

        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        public ConfigModel[] Configs => _configFile.Select(x => new ConfigModel(x.Key, x.Value, IsCore)).ToArray();

        public ConfigurationModel(ConfigFile configFile, BepInPlugin metadata,
            EFTConfigurationPluginAttributes configurationPlugin, bool isCore = false)
        {
            _configFile = configFile;
            _metadata = metadata;
            ConfigurationPluginAttributes = configurationPlugin;
            IsCore = isCore;

            configFile.SettingChanged += (value, value2) => SettingChanged?.Invoke();
        }
    }
}

#endif