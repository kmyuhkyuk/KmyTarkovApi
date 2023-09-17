using System;
using System.Linq;
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
                if (EFTVersion.AkiVersion > new Version("3.5.0"))
                {
                    OnBoxLand = new RefHelper.HookRef(AppDomain.CurrentDomain.GetAssemblies()
                        .Single(x => x.ManifestModule.Name == "aki-custom.dll")
                        .GetTypes().Single(x => x.Name == "AirdropBox"), "OnBoxLand");
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
                if (EFTVersion.AkiVersion > new Version("3.5.0"))
                {
                    RefAirdropType =
                        RefHelper.FieldRef<object, int>.Create(
                            RefTool.GetEftType(x => x.Name == "AirdropSynchronizableObject"), "AirdropType");
                }
            }
        }
    }
}