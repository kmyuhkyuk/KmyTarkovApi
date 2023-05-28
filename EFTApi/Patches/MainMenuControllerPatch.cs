using System;
using System.Reflection;
using System.Threading.Tasks;
using Aki.Reflection.Patching;
using EFT.UI;
using EFTReflection;
using EFTReflection.Patching;

namespace EFTApi.Patches
{
    public class MainMenuControllerPatch : BaseModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(MainMenuController).GetMethod("Execute", BindingFlags.Static | RefTool.Public);
        }

        [PatchPostfix]
        private static async void PatchPostfix(MainMenuController __instance, Task<MainMenuController> __result,
            object backEnd, EnvironmentUI environmentUI, MenuUI menuUI, CommonUI commonUI, PreloaderUI preloaderUI,
            object raidSettings, object hideoutController, Action onLogoutPressed, Action reconnectAction)
        {
            EFTHelpers._MainMenuControllerHelper.Trigger_Execute(await __result, backEnd, environmentUI, menuUI,
                commonUI, preloaderUI, raidSettings, hideoutController, onLogoutPressed, reconnectAction);
        }
    }

    public class MainMenuControllerUnsubscribePatch : BaseModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(MainMenuController).GetMethod("Unsubscribe", RefTool.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(MainMenuController __instance)
        {
            EFTHelpers._MainMenuControllerHelper.Trigger_Unsubscribe(__instance);
        }
    }
}