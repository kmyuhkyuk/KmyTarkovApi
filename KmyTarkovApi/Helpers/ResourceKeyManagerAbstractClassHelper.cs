using System;
using System.Collections.Generic;
using Comfort.Common;

namespace KmyTarkovApi.Helpers
{
    public class ResourceKeyManagerAbstractClassHelper
    {
        private static readonly Lazy<ResourceKeyManagerAbstractClassHelper> Lazy =
            new Lazy<ResourceKeyManagerAbstractClassHelper>(() => new ResourceKeyManagerAbstractClassHelper());

        public static ResourceKeyManagerAbstractClassHelper Instance => Lazy.Value;

        public Dictionary<string, Voice> VoiceDictionary =>
            Singleton<PlayerVoiceLoader>.Instantiated ? Singleton<PlayerVoiceLoader>.Instance._voices : null;
    }
}
