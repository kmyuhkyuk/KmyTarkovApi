using System;
using System.Collections.Generic;
using System.Linq;

namespace EFTUtils
{
    public class CustomLocalized<T>
    {
        public string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                _currentLanguage = value;

                LanguageChange?.Invoke();
            }
        }

        private string _currentLanguage;

        public string CurrentLanguageLower => _languagesLowerDictionary[CurrentLanguage];

        public readonly Dictionary<T, Dictionary<string, Dictionary<string, string>>> LanguageDictionary =
            new Dictionary<T, Dictionary<string, Dictionary<string, string>>>();

        public event Action LanguageChange;

        public event Action LanguageAdd;

        public string[] Languages => _languagesLowerDictionary.Keys.ToArray();

        private readonly Dictionary<string, string> _languagesLowerDictionary = new Dictionary<string, string>
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

        public void AddLanguage(string name)
        {
            if (!_languagesLowerDictionary.Keys.Contains(name, StringComparer.OrdinalIgnoreCase))
            {
                _languagesLowerDictionary.Add(name, name.ToLower());

                LanguageAdd?.Invoke();
            }
        }

        public virtual string Localized(T tKey, string key)
        {
            if (LanguageDictionary.TryGetValue(tKey, out var language)
                && (language.TryGetValue(CurrentLanguageLower, out var localizedDictionary) ||
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