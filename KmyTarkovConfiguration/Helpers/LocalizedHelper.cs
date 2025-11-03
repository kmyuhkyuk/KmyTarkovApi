using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace KmyTarkovConfiguration.Helpers
{
    public static class LocalizedHelper
    {
        private static string _currentLanguage = "En";

        public static string CurrentLanguage
        {
            get => _currentLanguage;
            internal set
            {
                if (_currentLanguage == value)
                    return;

                _currentLanguage = value;

                LanguageChange?.Invoke();
            }
        }

        public static string CurrentLanguageLower => LanguageNamesLowerDictionary[CurrentLanguage];

        internal static readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> LanguageDictionary =
            new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

        public static event Action LanguageChange;

        public static event Action LanguageAdd;

        public static string[] LanguageNames => LanguageNamesLowerDictionary.Keys.ToArray();

        private static readonly Dictionary<string, string> LanguageNamesLowerDictionary =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Cz", "cz" },
                { "De", "de" },
                { "En", "en" },
                { "Es", "es" },
                { "Fr", "fr" },
                { "Ge", "ge" },
                { "Hu", "hu" },
                { "It", "it" },
                { "Jp", "jp" },
                { "Ko", "ko" },
                { "Nl", "nl" },
                { "Pl", "pl" },
                { "Pt", "pt" },
                { "Ru", "ru" },
                { "Sk", "sk" },
                { "Sv", "sv" },
                { "Tr", "tr" },
                { "Zh", "zh" }
            };

        public static IReadOnlyDictionary<string, string> LanguageNamesDictionary =>
            LanguageNamesLowerDictionary.ToDictionary(x => x.Value, x => x.Key);

        public static IReadOnlyDictionary<string, string> LanguageNameLowercaseNamesDictionary =>
            LanguageNamesLowerDictionary;

        public static void AddLanguage(string name)
        {
            if (LanguageNamesLowerDictionary.ContainsKey(name))
                return;

            LanguageNamesLowerDictionary.Add(name, name.ToLower());

            LanguageAdd?.Invoke();
        }

        public static string Localized(string modName)
        {
            return Localized(modName, modName);
        }

        public static string Localized(string modName, string key)
        {
            if (LanguageDictionary.TryGetValue(modName, out var language)
                && (language.TryGetValue(CurrentLanguageLower, out var localizedDictionary) ||
                    language.TryGetValue("en", out localizedDictionary))
                && localizedDictionary.TryGetValue(key, out var localized))
            {
                return localized;
            }

            return key;
        }
    }
}