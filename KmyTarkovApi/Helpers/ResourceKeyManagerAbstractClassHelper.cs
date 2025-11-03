using System;
using System.Collections.Generic;
using KmyTarkovReflection;

namespace KmyTarkovApi.Helpers
{
    public class ResourceKeyManagerAbstractClassHelper
    {
        private static readonly Lazy<ResourceKeyManagerAbstractClassHelper> Lazy =
            new Lazy<ResourceKeyManagerAbstractClassHelper>(() => new ResourceKeyManagerAbstractClassHelper());

        public static ResourceKeyManagerAbstractClassHelper Instance => Lazy.Value;

        public readonly RefHelper.FieldRef<ResourceKeyManagerAbstractClass, Dictionary<string, string>>
            RefVoiceDictionary;

        public Dictionary<string, string> VoiceDictionary => RefVoiceDictionary.GetValue(null);

        private ResourceKeyManagerAbstractClassHelper()
        {
            RefVoiceDictionary =
                RefHelper.FieldRef<ResourceKeyManagerAbstractClass, Dictionary<string, string>>.Create(
                    EFTVersion.SPTVersion > EFTVersion.Parse("3.11.4") ? "Dictionary_0" : "dictionary_0");
        }
    }
}