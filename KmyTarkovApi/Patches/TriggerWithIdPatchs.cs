using System.Collections.Generic;
using System.Reflection;
using EFT.Interactive;
using HarmonyLib;
using KmyTarkovApi.Helpers;
using KmyTarkovReflection.Patching;

namespace KmyTarkovApi.Patches
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