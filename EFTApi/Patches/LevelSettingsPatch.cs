using System.Reflection;
using Aki.Reflection.Patching;
using EFTReflection;
using EFTReflection.Patching;

namespace EFTApi.Patches
{
    public class LevelSettingsPatch : BaseModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(LevelSettings).GetMethod("Awake", RefTool.NonPublic);
        }

        [PatchPostfix]
        private static void PatchPostfix(LevelSettings __instance)
        {
            EFTHelpers._GameWorldHelper.LevelSettingsHelper.Trigger_Awake(__instance);
        }
    }

    public class LevelSettingsOnDestroyPatch : BaseModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(LevelSettings).GetMethod("OnDestroy", RefTool.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(LevelSettings __instance)
        {
            EFTHelpers._GameWorldHelper.LevelSettingsHelper.Trigger_OnDestroy(__instance);
        }
    }
}