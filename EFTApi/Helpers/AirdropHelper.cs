using System;
using EFTReflection;
using JetBrains.Annotations;

// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable NotAccessedField.Global
// ReSharper disable UnusedMember.Global

namespace EFTApi.Helpers
{
    public class AirdropHelper
    {
        private static readonly Lazy<AirdropHelper> Lazy = new Lazy<AirdropHelper>(() => new AirdropHelper());

        public static AirdropHelper Instance => Lazy.Value;

        public AirdropBoxData AirdropBoxHelper => AirdropBoxData.Instance;

        public AirdropSynchronizableObjectData AirdropSynchronizableObjectHelper =>
            AirdropSynchronizableObjectData.Instance;

        private AirdropHelper()
        {
        }

        public class AirdropBoxData
        {
            private static readonly Lazy<AirdropBoxData> Lazy = new Lazy<AirdropBoxData>(() => new AirdropBoxData());

            public static AirdropBoxData Instance => Lazy.Value;

            [CanBeNull] public readonly RefHelper.HookRef OnBoxLand;

            [CanBeNull] public readonly RefHelper.HookRef CoopOnBoxLand;

            private AirdropBoxData()
            {
                if (EFTVersion.AkiVersion <= EFTVersion.Parse("3.5.0"))
                    return;

                OnBoxLand = RefHelper.HookRef.Create(
                    RefTool.GetPluginType(EFTPlugins.AkiCustom, "Aki.Custom.Airdrops.AirdropBox"),
                    "OnBoxLand");

                if (EFTVersion.IsMPT)
                {
                    CoopOnBoxLand = RefHelper.HookRef.Create(
                        RefTool.GetPluginType(EFTPlugins.MultiplayerTarkov,
                            "MultiplayerTarkov.AkiSupport.Airdrops.MPTAirdropBox"),
                        "OnBoxLand");
                }
            }
        }

        public class AirdropSynchronizableObjectData
        {
            private static readonly Lazy<AirdropSynchronizableObjectData> Lazy =
                new Lazy<AirdropSynchronizableObjectData>(() => new AirdropSynchronizableObjectData());

            public static AirdropSynchronizableObjectData Instance => Lazy.Value;

            /// <summary>
            ///     AirdropSynchronizableObject.AirdropType
            /// </summary>
            [CanBeNull] public readonly RefHelper.FieldRef<object, int> RefAirdropType;

            private AirdropSynchronizableObjectData()
            {
                if (EFTVersion.AkiVersion > EFTVersion.Parse("3.5.0"))
                {
                    RefAirdropType =
                        RefHelper.FieldRef<object, int>.Create(
                            RefTool.GetEftType(x => x.Name == "AirdropSynchronizableObject"), "AirdropType");
                }
            }
        }
    }
}