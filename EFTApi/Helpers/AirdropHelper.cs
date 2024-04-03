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

        public AirdropLogicClassData AirdropLogicClassHelper => AirdropLogicClassData.Instance;

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
                if (EFTVersion.AkiVersion > EFTVersion.Parse("3.4.1"))
                {
                    var airdropBoxType = RefTool.GetPluginType(EFTPlugins.AkiCustom, "Aki.Custom.Airdrops.AirdropBox");

                    OnBoxLand = RefHelper.HookRef.Create(airdropBoxType,
                        "OnBoxLand");

                    if (EFTVersion.IsMPT)
                    {
                        var mptAirdropBoxType = RefTool.GetPluginType(EFTPlugins.MPTCore,
                            "MPT.Core.AkiSupport.Airdrops.MPTAirdropBox");

                        CoopOnBoxLand = RefHelper.HookRef.Create(mptAirdropBoxType,
                            "OnBoxLand");
                    }
                }
            }
        }

        public class AirdropLogicClassData
        {
            private static readonly Lazy<AirdropLogicClassData> Lazy =
                new Lazy<AirdropLogicClassData>(() => new AirdropLogicClassData());

            public static AirdropLogicClassData Instance => Lazy.Value;

            public readonly RefHelper.HookRef OnBoxLand;

            private AirdropLogicClassData()
            {
                OnBoxLand = RefHelper.HookRef.Create(
                    EFTVersion.GameVersion == EFTVersion.Parse("3.0.0")
                        ? RefTool.GetEftType(x => x.Name == "AirdropLogic2Class")
                        : typeof(AirdropLogicClass),
                    x => x.GetParameters().Length == 2 &&
                         x.GetParameters()[0].ParameterType == typeof(TaggedClip) &&
                         x.GetParameters()[1].ParameterType == typeof(bool));
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
                if (EFTVersion.AkiVersion > EFTVersion.Parse("3.0.0"))
                {
                    RefAirdropType =
                        RefHelper.FieldRef<object, int>.Create(
                            RefTool.GetEftType(x => x.Name == "AirdropSynchronizableObject"), "AirdropType");
                }
            }
        }
    }
}