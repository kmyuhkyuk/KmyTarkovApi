using System;
using System.Linq;
using System.Reflection;
using Aki.Reflection.Patching;
using EFTReflection;
using EFTReflection.Patching;
using UnityEngine;

namespace EFTApi.Patches
{
    public class AirdropBoxPatch : BaseModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AppDomain.CurrentDomain.GetAssemblies().Single(x => x.ManifestModule.Name == "aki-custom.dll")
                .GetTypes().Single(x => x.Name == "AirdropBox").GetMethod("OnBoxLand", RefTool.NonPublic);
        }

        [PatchPostfix]
        private static void PatchPostfix(MonoBehaviour __instance, object ___boxSync, float clipLength)
        {
            EFTHelpers._AirdropHelper.AirdropBoxHelper.Trigger_OnBoxLand(__instance, ___boxSync, clipLength);
        }
    }
}