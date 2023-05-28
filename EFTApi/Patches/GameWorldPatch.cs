using System.Reflection;
using Aki.Reflection.Patching;
using EFT;
using EFTReflection;
using EFTReflection.Patching;

namespace EFTApi.Patches
{
    public class GameWorldAwakePatch : BaseModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod("Awake", RefTool.NonPublic);
        }

        [PatchPostfix]
        private static void PatchPostfix(GameWorld __instance)
        {
            EFTHelpers._GameWorldHelper.Trigger_Awake(__instance);
        }
    }

    public class GameWorldOnGameStartedPatch : BaseModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod("OnGameStarted", RefTool.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(GameWorld __instance)
        {
            EFTHelpers._GameWorldHelper.Trigger_OnGameStarted(__instance);
        }
    }

    public class GameWorldDisposePatch : BaseModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameWorld).GetMethod("Dispose", RefTool.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(GameWorld __instance)
        {
            EFTHelpers._GameWorldHelper.Trigger_Dispose(__instance);
        }
    }
}