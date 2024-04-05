using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if !UNITY_EDITOR
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using EFTConfiguration.Attributes;
using EFTConfiguration.Helpers;
using EFTConfiguration.Models;
using Force.Crc32;
using Newtonsoft.Json;
#endif

namespace EFTConfiguration.Controllers
{
    public class EFTConfigurationController : MonoBehaviour
    {
#if !UNITY_EDITOR

        private static Dictionary<string, PluginInfo> PluginInfos => Chainloader.PluginInfos;

        private static readonly PropertyInfo CoreConfigInfo = typeof(ConfigFile).GetProperty("CoreConfig",
            BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

        private static readonly FieldInfo[] EFTConfigurationPluginAttributesFields =
            typeof(EFTConfigurationPluginAttributes).GetFields();

        private static readonly ManualLogSource ModVerifyLogger = BepInEx.Logging.Logger.CreateLogSource("ModVerify");

        private void Start()
        {
            var eftConfigurationModel = EFTConfigurationModel.Instance;

            var configurationList = new List<ConfigurationModel>
                { GetCoreConfigurationData() };

            if (TryGetConfigurationData(eftConfigurationModel.Info, out var selfConfigurationData))
            {
                configurationList.Add(selfConfigurationData);
            }

            foreach (var pluginInfo in PluginInfos.Values)
            {
                var pluginVersion = pluginInfo.Metadata.Version;

                var fileVersion = FileVersionInfo.GetVersionInfo(pluginInfo.Location).FileVersion;

                var crc32 = Crc32CAlgorithm.Compute(File.ReadAllBytes(pluginInfo.Location)).ToString("X");

                ModVerifyLogger.LogMessage(
                    $"{Path.GetFileNameWithoutExtension(pluginInfo.Location)} Version:{pluginVersion}{(!string.IsNullOrEmpty(fileVersion) ? $"({fileVersion})" : null)} CRC32:{crc32}");

                if (pluginInfo == eftConfigurationModel.Info)
                    continue;

                if (TryGetConfigurationData(pluginInfo, out var configurationData))
                {
                    configurationList.Add(configurationData);
                }
            }

            eftConfigurationModel.Configurations = configurationList.ToArray();

            EFTConfigurationModel.Instance.CreateUI();
        }

        private static bool TryGetConfigurationData(PluginInfo pluginInfo, out ConfigurationModel configurationData)
        {
            configurationData = null;

            var instance = pluginInfo.Instance;

            if (instance == null)
                return false;

            var configFile = instance.Config;

            if (configFile == null)
                return false;

            var metaData = pluginInfo.Metadata;

            var type = instance.GetType();

            var eftConfigurationPluginAttributes = new EFTConfigurationPluginAttributes(string.Empty);

            var hasAttributes = false;
            foreach (var attribute in type.GetCustomAttributes())
            {
                var attributeType = attribute.GetType();

                if (attributeType.Name != nameof(EFTConfigurationPluginAttributes))
                    continue;

                var eftConfigurationPluginExternalAttributesFieldInfos = attributeType.GetFields();

                foreach (var eftConfigurationPluginAttributesFieldInfo in
                         EFTConfigurationPluginAttributesFields)
                {
                    var eftConfigurationPluginExternalAttributesFieldInfo =
                        eftConfigurationPluginExternalAttributesFieldInfos.SingleOrDefault(x =>
                            x.Name == eftConfigurationPluginAttributesFieldInfo.Name && x.FieldType ==
                            eftConfigurationPluginAttributesFieldInfo.FieldType);

                    if (eftConfigurationPluginExternalAttributesFieldInfo == null)
                        continue;

                    eftConfigurationPluginAttributesFieldInfo.SetValue(eftConfigurationPluginAttributes,
                        eftConfigurationPluginExternalAttributesFieldInfo.GetValue(attribute));
                }

                hasAttributes = true;

                break;
            }

            if (!hasAttributes)
            {
                eftConfigurationPluginAttributes.ModURL =
                    FileVersionInfo.GetVersionInfo(pluginInfo.Location).CompanyName;
                eftConfigurationPluginAttributes.HidePlugin =
                    type.GetCustomAttributes<BrowsableAttribute>().Any(x => !x.Browsable);
            }

            LocalizedHelper.LanguageDictionary.Add(metaData.Name,
                GetLanguageDictionary(pluginInfo, eftConfigurationPluginAttributes.LocalizedPath));

            configurationData = new ConfigurationModel(configFile, metaData, eftConfigurationPluginAttributes);

            return true;
        }

        private static ConfigurationModel GetCoreConfigurationData()
        {
            var configFile = (ConfigFile)CoreConfigInfo.GetValue(null);

            var metaData = new BepInPlugin("BepInEx", "BepInEx",
                typeof(BaseUnityPlugin).Assembly.GetName().Version.ToString());

            return new ConfigurationModel(configFile, metaData, new EFTConfigurationPluginAttributes(string.Empty),
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

            var assemblyDirectoryPath = Path.GetDirectoryName(pluginInfo.Location);

            if (string.IsNullOrEmpty(assemblyDirectoryPath))
                return localizedDictionary;

            var localizedDirectory = new DirectoryInfo(Path.Combine(assemblyDirectoryPath, localizedPath));

            if (!localizedDirectory.Exists)
                return localizedDictionary;

            foreach (var localized in localizedDirectory.GetFiles("*.json"))
            {
                localizedDictionary.Add(Path.GetFileNameWithoutExtension(localized.Name),
                    JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(localized.FullName)));
            }

            return localizedDictionary;
        }

#endif
    }
}