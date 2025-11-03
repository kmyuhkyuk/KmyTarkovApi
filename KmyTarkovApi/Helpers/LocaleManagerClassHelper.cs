using System;
using KmyTarkovReflection;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Global

namespace KmyTarkovApi.Helpers
{
    public class LocaleManagerClassHelper
    {
        private static readonly Lazy<LocaleManagerClassHelper> Lazy =
            new Lazy<LocaleManagerClassHelper>(() => new LocaleManagerClassHelper());

        public static LocaleManagerClassHelper Instance => Lazy.Value;

        public LocaleManagerClass LocaleManagerClass => RefLocaleManagerClass.GetValue(null);

        public string CurrentLanguage => LocaleManagerClass.String_0;

        public readonly RefHelper.PropertyRef<LocaleManagerClass, LocaleManagerClass> RefLocaleManagerClass;

        private LocaleManagerClassHelper()
        {
            RefLocaleManagerClass =
                RefHelper.PropertyRef<LocaleManagerClass, LocaleManagerClass>.Create("LocaleManagerClass");
        }
    }
}