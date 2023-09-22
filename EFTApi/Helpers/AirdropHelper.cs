using System;
using EFTReflection;
using JetBrains.Annotations;

namespace EFTApi.Helpers
{
    public class AirdropHelper
    {
        public static readonly AirdropHelper Instance = new AirdropHelper();

        public readonly AirdropBoxData AirdropBoxHelper = AirdropBoxData.Instance;

        public readonly AirdropSynchronizableObjectData AirdropSynchronizableObjectHelper =
            AirdropSynchronizableObjectData.Instance;

        private AirdropHelper()
        {
        }

        public class AirdropBoxData
        {
            public static readonly AirdropBoxData Instance = new AirdropBoxData();

            [CanBeNull] public readonly RefHelper.HookRef OnBoxLand;

            private AirdropBoxData()
            {
                if (EFTVersion.AkiVersion > Version.Parse("3.5.0") &&
                    RefTool.TryGetPlugin("com.spt-aki.custom", out var plugin))
                {
                    OnBoxLand = RefHelper.HookRef.Create(
                        RefTool.GetPluginType(plugin, "Aki.Custom.Airdrops.AirdropBox"),
                        "OnBoxLand");
                }
            }
        }

        public class AirdropSynchronizableObjectData
        {
            public static readonly AirdropSynchronizableObjectData Instance = new AirdropSynchronizableObjectData();

            /// <summary>
            ///     AirdropSynchronizableObject.AirdropType
            /// </summary>
            [CanBeNull] public readonly RefHelper.FieldRef<object, int> RefAirdropType;

            private AirdropSynchronizableObjectData()
            {
                if (EFTVersion.AkiVersion > Version.Parse("3.5.0"))
                {
                    RefAirdropType =
                        RefHelper.FieldRef<object, int>.Create(
                            RefTool.GetEftType(x => x.Name == "AirdropSynchronizableObject"), "AirdropType");
                }
            }
        }
    }
}