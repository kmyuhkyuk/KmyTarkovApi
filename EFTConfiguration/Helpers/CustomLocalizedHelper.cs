using System;
using System.Collections.Generic;
using System.Linq;

namespace EFTConfiguration.Helpers
{
    public static class CustomLocalizedHelper
    {
        public static string CurrentLanguage
        {
            get => _currentLanguage;
            internal set
            {
                _currentLanguage = value;

                LanguageChange?.Invoke();
            }
        }

        private static string _currentLanguage;

        internal static readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> LanguageList =
            new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

        public static event Action LanguageChange;

        public static event Action LanguageAdd;

        public static string[] Languages => LanguagesHashSet.ToArray();

        private static readonly HashSet<string> LanguagesHashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Cz",
            "De",
            "En",
            "Es",
            "Fr",
            "Ge",
            "Hu",
            "It",
            "Jp",
            "Ko",
            "Nl",
            "Pl",
            "Pt",
            "Ru",
            "Sk",
            "Sv",
            "Tr",
            "Zh"
        };

        public static void AddLanguage(string name)
        {
            if (LanguagesHashSet.Add(name))
            {
                LanguageAdd?.Invoke();
            }
        }

        public static string Localized(string modName)
        {
            return Localized(modName, modName);
        }

        public static string Localized(string modName, string key)
        {
            if (LanguageList.TryGetValue(modName, out var language)
                && (language.TryGetValue(CurrentLanguage.ToLower(), out var localizedDictionary) ||
                    language.TryGetValue("en", out localizedDictionary))
                && localizedDictionary.TryGetValue(key, out var localized))
            {
                return localized;
            }
            else
            {
                return key;
            }
        }
    }
}