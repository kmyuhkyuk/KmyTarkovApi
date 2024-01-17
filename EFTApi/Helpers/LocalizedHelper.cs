using System;
using System.Linq;
using System.Reflection;
using EFT;
using EFTReflection;
using HarmonyLib;

// ReSharper disable UnusedMember.Global

namespace EFTApi.Helpers
{
    public class LocalizedHelper
    {
        private static readonly Lazy<LocalizedHelper> Lazy = new Lazy<LocalizedHelper>(() => new LocalizedHelper());

        public static LocalizedHelper Instance => Lazy.Value;

        private readonly Func<string, string, string> _refLocalized;

        private readonly Func<string, EStringCase, string> _refLocalizedCase;

        private readonly Func<string, string> _refTransliterate;

        private LocalizedHelper()
        {
            const BindingFlags flags = BindingFlags.Static | RefTool.Public;

            var localizedType = RefTool.GetEftType(x => x.GetMethod("ParseLocalization", flags) != null);

            _refLocalized = AccessTools.MethodDelegate<Func<string, string, string>>(RefTool.GetEftMethod(localizedType,
                flags,
                x => x.Name == "Localized"
                     && x.GetParameters().Length == 2
                     && x.GetParameters()[0].ParameterType == typeof(string)
                     && x.GetParameters()[1].ParameterType == typeof(string)));

            _refLocalizedCase = AccessTools.MethodDelegate<Func<string, EStringCase, string>>(RefTool.GetEftMethod(
                localizedType,
                flags,
                x => x.Name == "Localized"
                     && x.GetParameters().Length == 2
                     && x.GetParameters()[0].ParameterType == typeof(string)
                     && x.GetParameters()[1].ParameterType == typeof(EStringCase)));

            _refTransliterate = AccessTools.MethodDelegate<Func<string, string>>(RefTool.GetEftMethod(
                x => x.GetMethods(flags).Any(t => t.Name == "Transliterate"), flags,
                x => x.Name == "Transliterate"
                     && x.GetParameters().Length == 1));
        }

        public string Localized(string id, string prefix = null)
        {
            return _refLocalized(id, prefix);
        }

        public string Localized(string id, EStringCase @case)
        {
            return _refLocalizedCase(id, @case);
        }

        public string Transliterate(string text)
        {
            return _refTransliterate(text);
        }
    }
}