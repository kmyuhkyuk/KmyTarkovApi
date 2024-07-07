using System;
using System.Reflection.Emit;
using EFT.Interactive;
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

            [CanBeNull] public readonly RefHelper.PropertyRef<object, LootableContainer> RefCoopContainer;

            private AirdropBoxData()
            {
                if (EFTVersion.AkiVersion > EFTVersion.Parse("3.4.1"))
                {
                    var airdropBoxType = EFTVersion.AkiVersion > EFTVersion.Parse("3.8.3")
                        ? RefTool.GetPluginType(EFTPlugins.AkiCustom, "SPT.Custom.Airdrops.AirdropBox")
                        : RefTool.GetPluginType(EFTPlugins.AkiCustom, "Aki.Custom.Airdrops.AirdropBox");

                    OnBoxLand = RefHelper.HookRef.Create(airdropBoxType,
                        "OnBoxLand");

                    if (!EFTVersion.IsFika)
                        return;

                    var coopAirdropBoxType = RefTool.GetPluginType(EFTPlugins.FikaCore,
                        EFTVersion.FikaVersion > EFTVersion.Parse("0.9.8944.20016")
                            ? "Fika.Core.Coop.Airdrops.FikaAirdropBox"
                            : "Fika.Core.AkiSupport.Airdrops.FikaAirdropBox");

                    RefCoopContainer =
                        RefHelper.PropertyRef<object, LootableContainer>.Create(coopAirdropBoxType, "Container");

                    CoopOnBoxLand = RefHelper.HookRef.Create(coopAirdropBoxType,
                        "OnBoxLand");
                }
            }
        }

        public class AirdropLogicClassData
        {
            private static readonly Lazy<AirdropLogicClassData> Lazy =
                new Lazy<AirdropLogicClassData>(() => new AirdropLogicClassData());

            public static AirdropLogicClassData Instance => Lazy.Value;

            public readonly RefHelper.HookRef RaycastGround;

            private AirdropLogicClassData()
            {
                Type airdropLogicClassType;

                if (EFTVersion.AkiVersion == EFTVersion.Parse("2.3.1"))
                {
                    airdropLogicClassType =
                        RefTool.GetEftType(x => x.GetMethod("ParachuteFadeCoroutine", RefTool.Public) != null);
                }
                else if (EFTVersion.AkiVersion == EFTVersion.Parse("3.0.0"))
                {
                    airdropLogicClassType = RefTool.GetEftType(x => x.Name == "AirdropLogic2Class");
                }
                else
                {
                    airdropLogicClassType = typeof(AirdropLogicClass);
                }

                RaycastGround = RefHelper.HookRef.Create(airdropLogicClassType, x => x
                    .ReadMethodBody().ContainsIL(OpCodes.Ldstr,
                        "Raycast to ground returns no hit. Raycast from position {0} on distance {1}. Choose Concrete sound landing set"));
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