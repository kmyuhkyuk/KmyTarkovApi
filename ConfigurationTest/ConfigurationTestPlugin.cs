using System;
using System.Diagnostics.CodeAnalysis;
using BepInEx;
using BepInEx.Configuration;
using KmyTarkovConfiguration.Attributes;
using UnityEngine;

// ReSharper disable UnusedMember.Local

namespace ConfigurationTest
{
    [BepInPlugin("com.kmyuhkyuk.ConfigurationTest", "ConfigurationTest", "1.4.1")]
    [BepInDependency("com.kmyuhkyuk.KmyTarkovConfiguration", "1.4.1")]
    public class ConfigurationTestPlugin : BaseUnityPlugin
    {
        private bool _testLoopThrow;

        private enum E
        {
            A,
            B,
            C
        }

        [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
        private void Awake()
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
            Config.Bind<E>(testSettings, "Enum", E.A,
                new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind<KeyboardShortcut>(testSettings, "KeyboardShortcut", KeyboardShortcut.Empty,
                new ConfigDescription(string.Empty, null, eftConfigurationAttributes));

            Config.Bind<double>(testSettings, "Double", 0d,
                new ConfigDescription(string.Empty, null, eftConfigurationAttributes));
            Config.Bind(testSettings, "Action", string.Empty,
                new ConfigDescription(string.Empty, null,
                    new EFTConfigurationAttributes { Advanced = true, ButtonAction = () => Logger.LogError("Work") }));

            var keyTestLoopThrow = Config.Bind<bool>(testSettings, "Test Loop Throw", false,
                new ConfigDescription(string.Empty, null, eftConfigurationAttributes));

            keyTestLoopThrow.SettingChanged += (sender, args) => _testLoopThrow = keyTestLoopThrow.Value;

            Config.Bind(testSettings, "MemberAccessException", string.Empty,
                new ConfigDescription(string.Empty, null,
                    new EFTConfigurationAttributes { Advanced = true, ButtonAction = MemberAccessException }));

            Config.Bind(testSettings, "MissingMemberException", string.Empty,
                new ConfigDescription(string.Empty, null,
                    new EFTConfigurationAttributes { Advanced = true, ButtonAction = MissingMemberException }));

            Config.Bind(testSettings, "MethodAccessException", string.Empty,
                new ConfigDescription(string.Empty, null,
                    new EFTConfigurationAttributes { Advanced = true, ButtonAction = MethodAccessException }));

            Config.Bind(testSettings, "MissingMethodException", string.Empty,
                new ConfigDescription(string.Empty, null,
                    new EFTConfigurationAttributes { Advanced = true, ButtonAction = MissingMethodException }));

            Config.Bind(testSettings, "MissingFieldException", string.Empty,
                new ConfigDescription(string.Empty, null,
                    new EFTConfigurationAttributes { Advanced = true, ButtonAction = MissingFieldException }));

            Config.Bind(testSettings, "FieldAccessException", string.Empty,
                new ConfigDescription(string.Empty, null,
                    new EFTConfigurationAttributes { Advanced = true, ButtonAction = FieldAccessException }));
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

        private static void MemberAccessException()
        {
            throw new MemberAccessException();
        }

        private static void MissingMemberException()
        {
            throw new MissingMemberException();
        }

        private static void MethodAccessException()
        {
            throw new MethodAccessException();
        }

        private static void MissingMethodException()
        {
            throw new MissingMethodException();
        }

        private static void MissingFieldException()
        {
            throw new MissingFieldException();
        }

        private static void FieldAccessException()
        {
            throw new FieldAccessException();
        }

        /*private void Start()
        {
            throw new NotImplementedException();
        }*/
    }
}