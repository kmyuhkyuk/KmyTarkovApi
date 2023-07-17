using System;
using System.Linq;
using EFTReflection;

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

            public readonly RefHelper.HookRef OnBoxLand;

            private AirdropBoxData()
            {
                if (EFTVersion.Is350Up)
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
            public readonly RefHelper.FieldRef<object, int> RefAirdropType;

            private AirdropSynchronizableObjectData()
            {
                if (EFTVersion.Is350Up)
                {
                    RefAirdropType =
                        RefHelper.FieldRef<object, int>.Create(
                            RefTool.GetEftType(x => x.Name == "AirdropSynchronizableObject"), "AirdropType");
                }
            }
        }
    }
}