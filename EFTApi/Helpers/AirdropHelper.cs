using System;
using EFTReflection;
using UnityEngine;

namespace EFTApi.Helpers
{
    public class AirdropHelper
    {
        /// <summary>
        /// AirdropBox Helper
        /// </summary>
        public readonly AirdropBoxData AirdropBoxHelper = new AirdropBoxData();

        /// <summary>
        /// AirdropSynchronizableObject Helper
        /// </summary>
        public readonly AirdropSynchronizableObjectData AirdropSynchronizableObjectHelper = new AirdropSynchronizableObjectData();

        public class AirdropBoxData
        {
            public event Action<MonoBehaviour, object, float> OnBoxLand;

            internal void Trigger_OnBoxLand(MonoBehaviour airdropBox, object boxSync, float clipLength)
            {
                OnBoxLand?.Invoke(airdropBox, boxSync, clipLength);
            }
        }

        public class AirdropSynchronizableObjectData
        {
            /// <summary>
            /// AirdropSynchronizableObject.AirdropType
            /// </summary>
            public readonly RefHelper.FieldRef<object, int> RefAirdropType;

            public AirdropSynchronizableObjectData()
            {
                if (EFTVersion.Is350Up)
                {
                    RefAirdropType = RefHelper.FieldRef<object, int>.Create(RefTool.GetEftType(x => x.Name == "AirdropSynchronizableObject"), "AirdropType");
                }
            }
        }
    }
}
