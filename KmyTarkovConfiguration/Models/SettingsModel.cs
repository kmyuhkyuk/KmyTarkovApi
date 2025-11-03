#if !UNITY_EDITOR

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using BepInEx.Configuration;
using HarmonyLib;
using KmyTarkovConfiguration.AcceptableValue;
using KmyTarkovConfiguration.Helpers;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace KmyTarkovConfiguration.Models
{
    internal class SettingsModel
    {
        public static SettingsModel Instance { get; private set; }

        public readonly ConfigEntry<KeyboardShortcut> KeyConfigurationShortcut;

        public readonly ConfigEntry<Vector2> KeyDefaultPosition;
        public readonly ConfigEntry<Vector2> KeyDescriptionPositionOffset;

        public readonly ConfigEntry<string> KeyLanguage;

        public readonly ConfigEntry<string> KeySearch;

        public readonly ConfigEntry<bool> KeyAdvanced;

        public readonly ConfigEntry<int> KeySortingOrder;

        [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
        private SettingsModel(ConfigFile configFile)
        {
            const string mainSettings = "Main Settings";

            KeyConfigurationShortcut = configFile.Bind<KeyboardShortcut>(mainSettings, "Configuration Shortcut",
                new KeyboardShortcut(KeyCode.Home));
            KeyDefaultPosition = configFile.Bind<Vector2>(mainSettings, "Default Position", Vector2.zero);
            KeyDescriptionPositionOffset = configFile.Bind<Vector2>(mainSettings, "Description Position Offset",
                new Vector2(0, -25), new ConfigDescription("Description position offset from Mouse position"));
            KeyLanguage = configFile.Bind<string>(mainSettings, "Language",
                "Default",
                new ConfigDescription(
                    "Preferred language, if not available will tried English, if still not available than return original text",
                    new AcceptableValueCustomList<string>(new[] { "Default" }
                        .Concat(LocalizedHelper.LanguageNames)
                        .ToArray())));
            KeySearch = configFile.Bind<string>(mainSettings, "Search", string.Empty);
            KeyAdvanced = configFile.Bind<bool>(mainSettings, "Advanced", false);
            KeySortingOrder = configFile.Bind<int>(mainSettings, "Sorting Order", 29997);

            var acceptableValueCustomList =
                (AcceptableValueCustomList<string>)KeyLanguage.Description.AcceptableValues;
            LocalizedHelper.LanguageAdd +=
                () => acceptableValueCustomList.AcceptableValuesCustom =
                    new[] { "Default" }.Concat(LocalizedHelper.LanguageNames).ToArray();

            var localeManagerClass = Traverse.Create(typeof(LocaleManagerClass)).Property("LocaleManagerClass")
                .GetValue<LocaleManagerClass>();

            SwitchLanguage(localeManagerClass.String_0);

            KeyLanguage.SettingChanged += (value, value2) => SwitchLanguage(localeManagerClass.String_0);

            localeManagerClass.AddLocaleUpdateListener(() => SwitchLanguageFromGame(localeManagerClass.String_0));
        }

        private void SwitchLanguage(string language)
        {
            if (KeyLanguage.Value == "Default")
            {
                SwitchLanguageFromGame(language);
            }
            else
            {
                LocalizedHelper.CurrentLanguage = KeyLanguage.Value;
            }
        }

        private static void SwitchLanguageFromGame(string gameLanguage)
        {
            switch (gameLanguage)
            {
                case "ch":
                    LocalizedHelper.CurrentLanguage = "Zh";
                    break;
                default:
                    LocalizedHelper.CurrentLanguage = LocalizedHelper.LanguageNamesDictionary[gameLanguage];
                    break;
            }
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public static SettingsModel Create(ConfigFile configFile)
        {
            if (Instance != null)
                return Instance;

            return Instance = new SettingsModel(configFile);
        }
    }
}

#endif