using System;
using System.Collections.Generic;
using System.Reflection;
using EFTReflection;
using HarmonyLib;

namespace EFTApi.Helpers
{
    public class VoiceHelper
    {
        private static readonly Lazy<VoiceHelper> Lazy =
            new Lazy<VoiceHelper>(() => new VoiceHelper());

        public static VoiceHelper Instance => Lazy.Value;

        public readonly RefHelper.FieldRef<object, Dictionary<string, string>> RefVoiceDictionary;

        public Dictionary<string, string> VoiceDictionary => RefVoiceDictionary.GetValue(null);

        private readonly Func<string, string> _refTakePhrasePath;

        private VoiceHelper()
        {
            var voiceClassType = RefTool.GetEftType(x =>
                x.GetMethod("TakePhrasePath", BindingFlags.Static | RefTool.Public) != null);

            RefVoiceDictionary =
                RefHelper.FieldRef<object, Dictionary<string, string>>.Create(voiceClassType, "dictionary_0");

            _refTakePhrasePath =
                AccessTools.MethodDelegate<Func<string, string>>(voiceClassType.GetMethod("TakePhrasePath",
                    BindingFlags.Static | RefTool.Public));
        }

        public string TakePhrasePath(string name)
        {
            return _refTakePhrasePath(name);
        }
    }
}