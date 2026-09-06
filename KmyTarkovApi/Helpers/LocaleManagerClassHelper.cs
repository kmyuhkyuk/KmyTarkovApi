using System;
using EFT;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Global

namespace KmyTarkovApi.Helpers
{
    public class LocaleManagerClassHelper
    {
        private static readonly Lazy<LocaleManagerClassHelper> Lazy =
            new Lazy<LocaleManagerClassHelper>(() => new LocaleManagerClassHelper());

        public static LocaleManagerClassHelper Instance => Lazy.Value;

        public LocalizationManager LocaleManagerClass => LocalizationManager.Instance;

        public string CurrentLanguage => LocaleManagerClass.Culture;
    }
}
