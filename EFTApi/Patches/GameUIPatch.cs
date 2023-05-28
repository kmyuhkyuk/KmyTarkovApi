using System.Reflection;
using Aki.Reflection.Patching;
using EFT.UI;
using EFTReflection;
using EFTReflection.Patching;

namespace EFTApi.Patches
{
    public class GameUIPatch : BaseModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameUI).GetMethod("Awake", RefTool.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(GameUI __instance)
        {
            EFTHelpers._GameUIHelper.Trigger_Awake(__instance);
        }
    }

    public class GameUIOnDestroyPatch : BaseModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(GameUI).GetMethod("OnDestroy", RefTool.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(GameUI __instance)
        {
            EFTHelpers._GameUIHelper.Trigger_Destroy(__instance);
        }
    }
}
