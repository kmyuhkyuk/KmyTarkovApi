using System.Collections.Generic;
using System.Reflection;
using Aki.Reflection.Patching;
using EFT.Interactive;
using EFTReflection;
using EFTReflection.Patching;

namespace EFTApi.Patches
{
    public class TriggerWithIdPatch : ModulePatchs
    {
        protected override IEnumerable<MethodBase> GetTargetMethods()
        {
            yield return typeof(TriggerWithId).GetMethod("Awake", RefTool.NonPublic);
            yield return typeof(ExperienceTrigger).GetMethod("Awake", RefTool.NonPublic);
        }

        [PatchPostfix]
        private static void PatchPostfix(TriggerWithId __instance)
        {
            EFTHelpers._GameWorldHelper.ZoneHelper.TriggerPoints.Add(__instance);
        }
    }
}
