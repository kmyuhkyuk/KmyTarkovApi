using System;
using System.Collections.Generic;

namespace EFTConfiguration.Helpers
{
    public static class CustomLocalizedHelper
    {
        internal static Language CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                _currentLanguage = value;

                LanguageChange?.Invoke();
            }
        }

        private static Language _currentLanguage;

        internal static readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> LanguageList = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

        internal static event Action LanguageChange;

        public enum Language
        {
            Cz,
            De,
            En,
            Es,
            Fr,
            Ge,
            Hu,
            It,
            Jp,
            Ko,
            Nl,
            Pl,
            Pt,
            Ru,
            Sk,
            Sv,
            Tr,
            Zh,
        }

        public static string Localized(string modName)
        {
            return Localized(modName, modName);
        }

        public static string Localized(string modName, string key)
        {
            if (LanguageList.TryGetValue(modName, out var language) 
                && (language.TryGetValue(CurrentLanguage.ToString().ToLower(), out var localizedDictionary) || language.TryGetValue(nameof(Language.En).ToLower(), out localizedDictionary)) 
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
