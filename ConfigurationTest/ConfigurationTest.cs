using System;
using BepInEx;
using BepInEx.Configuration;
using EFTConfiguration.Attributes;
using EFTConfiguration.Helpers;
using UnityEngine;

namespace ConfigurationTest
{
    [BepInPlugin("com.kmyuhkyuk.ConfigurationTest", "kmyuhkyuk-ConfigurationTest", "1.1.6")]
    [BepInDependency("com.kmyuhkyuk.EFTConfiguration", "1.1.6")]
    public class ConfigurationTest : BaseUnityPlugin
    {
        private bool _testLoopThrow;

        private ConfigurationTest()
        {
            const string testSettings = "Test Settings";
            var eftConfigurationAttributes = new EFTConfigurationAttributes { Advanced = true };

            Config.Bind<bool>(testSettings, "Bool", false,
                new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind<int>(testSettings, "Int", 0,
                new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind<int>(testSettings, "Int Slider", 0,
                new ConfigDescription(string.Empty, new AcceptableValueRange<int>(0, 100), eftConfigurationAttributes));
            Config.Bind<float>(testSettings, "Float", 0f,
                new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind<float>(testSettings, "Float Slider", 0f,
                new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0f, 100f),
                    eftConfigurationAttributes));
            Config.Bind<string>(testSettings, "String", string.Empty,
                new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind<string>(testSettings, "String Dropdown", string.Empty,
                new ConfigDescription("123", new AcceptableValueList<string>("123", "234"),
                    eftConfigurationAttributes));
            Config.Bind<Vector2>(testSettings, "Vector2", Vector2.zero,
                new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind<Vector3>(testSettings, "Vector3", Vector3.zero,
                new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind<Vector4>(testSettings, "Vector4", Vector4.zero,
                new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind<Quaternion>(testSettings, "Quaternion", Quaternion.identity,
                new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind<Color>(testSettings, "Color", Color.white,
                new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind<CustomLocalizedHelper.Language>(testSettings, "Enum", CustomLocalizedHelper.Language.En,
                new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind<KeyboardShortcut>(testSettings, "KeyboardShortcut", KeyboardShortcut.Empty,
                new ConfigDescription(string.Empty, null, eftConfigurationAttributes));

            Config.Bind<double>(testSettings, "Double", 0d,
                new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind(testSettings, "Action", string.Empty,
                new ConfigDescription(string.Empty, null,
                    new EFTConfigurationAttributes { Advanced = true, ButtonAction = () => Logger.LogError("Work") }));
        }

        private void Update()
        {
            if (_testLoopThrow)
            {
                Test();
            }
        }

        private static void Test()
        {
            throw new NotImplementedException();
        }

        /*private void Start()
        {
            throw new NotImplementedException();
        }*/
    }
}