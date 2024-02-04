using System.Collections.Generic;
using System.Reflection;
using Aki.Reflection.Patching;
using EFT.Interactive;
using EFTApi.Helpers;
using EFTReflection.Patching;
using HarmonyLib;

namespace EFTApi.Patches
{
    public class TriggerWithIdPatchs : ModulePatchs
    {
        protected override IEnumerable<MethodBase> GetTargetMethods()
        {
            yield return typeof(TriggerWithId).GetMethod("Awake", AccessTools.all);
            yield return typeof(ExperienceTrigger).GetMethod("Awake", AccessTools.all);
        }

        [PatchPostfix]
        private static void PatchPostfix(TriggerWithId __instance)
        {
            GameWorldHelper.ZoneData.Instance.TriggerPoints.Add(__instance);
        }
    }
}