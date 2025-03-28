﻿using System;
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

        public static string CurrentLanguageLower => LanguagesLowerDictionary[CurrentLanguage];

        internal static readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> LanguageDictionary =
            new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

        public static event Action LanguageChange;

        public static event Action LanguageAdd;

        public static string[] Languages => LanguagesLowerDictionary.Keys.ToArray();

        private static readonly Dictionary<string, string> LanguagesLowerDictionary =
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

        public static void AddLanguage(string name)
        {
            if (LanguagesLowerDictionary.ContainsKey(name))
                return;

            LanguagesLowerDictionary.Add(name, name.ToLower());

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