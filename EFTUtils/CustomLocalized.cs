using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedType.Global
// ReSharper disable EventNeverSubscribedTo.Global
// ReSharper disable UnusedMember.Global

namespace EFTUtils
{
    public abstract class CustomLocalized<T, TV>
    {
        protected virtual string CustomCurrentLanguage { get; set; } = "En";

        public string CurrentLanguage
        {
            get => CustomCurrentLanguage;
            set
            {
                if (CustomCurrentLanguage == value)
                    return;

                CustomCurrentLanguage = value;

                LanguageChange?.Invoke();
            }
        }

        public string CurrentLanguageLower => LanguagesLowerDictionary[CurrentLanguage];

        public readonly Dictionary<T, TV> LanguageDictionary =
            new Dictionary<T, TV>();

        public event Action LanguageChange;

        public event Action LanguageAdd;

        public string[] Languages => LanguagesLowerDictionary.Keys.ToArray();

        protected virtual Dictionary<string, string> LanguagesLowerDictionary { get; } = new Dictionary<string, string>
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

        public virtual void AddLanguage(string name)
        {
            if (LanguagesLowerDictionary.Keys.Contains(name, StringComparer.OrdinalIgnoreCase))
                return;

            LanguagesLowerDictionary.Add(name, name.ToLower());

            LanguageAdd?.Invoke();
        }

        public abstract string Localized(T tKey, TV key);
    }
}