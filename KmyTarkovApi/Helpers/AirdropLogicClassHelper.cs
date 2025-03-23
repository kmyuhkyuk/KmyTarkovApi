using System;
using System.Reflection.Emit;
using KmyTarkovReflection;

// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable NotAccessedField.Global
// ReSharper disable UnusedMember.Global

namespace KmyTarkovApi.Helpers
{
    public class AirdropLogicClassHelper
    {
        private static readonly Lazy<AirdropLogicClassHelper> Lazy =
            new Lazy<AirdropLogicClassHelper>(() => new AirdropLogicClassHelper());

        public static AirdropLogicClassHelper Instance => Lazy.Value;

        public readonly RefHelper.HookRef RaycastGround;

        private AirdropLogicClassHelper()
        {
            var airdropLogicClassType = typeof(AirdropLogicClass);

            RaycastGround = RefHelper.HookRef.Create(airdropLogicClassType, x => x
                .ReadMethodBody().ContainsIL(OpCodes.Ldstr,
                    "Raycast to ground returns no hit. Raycast from position {0} on distance {1}. Choose Concrete sound landing set"));
        }
    }
}