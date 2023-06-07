using System;
using System.Reflection;
using System.Threading.Tasks;
using EFT.UI;
using EFTReflection;
using EFTReflection.Patching;

namespace EFTApi.Helpers
{
    public class MainMenuControllerHelper
    {
        public static readonly MainMenuControllerHelper Instance = new MainMenuControllerHelper();

        public MainMenuController MainMenuController { get; private set; }

        /// <summary>
        ///     Init Action
        /// </summary>
        /*public event Action<MainMenuController, object, EnvironmentUI, MenuUI, CommonUI, PreloaderUI, object, object,
            Action, Action> Execute;*/
        public event hook_Unsubscribe Unsubscribe
        {
            add => HookPatch.Add(typeof(MainMenuController).GetMethod("Unsubscribe", RefTool.Public), value);
            remove => HookPatch.Remove(typeof(MainMenuController).GetMethod("Unsubscribe", RefTool.Public), value);
        }

        public delegate void hook_Unsubscribe(MainMenuController __instance);

        public event hook_Execute Execute
        {
            add => HookPatch.Add(typeof(MainMenuController).GetMethod("Execute", BindingFlags.Static | RefTool.Public),
                value);
            remove => HookPatch.Remove(
                typeof(MainMenuController).GetMethod("Execute", BindingFlags.Static | RefTool.Public), value);
        }

        public delegate void hook_Execute(MainMenuController __instance, Task<MainMenuController> __result,
            object backEnd, EnvironmentUI environmentUI, MenuUI menuUI, CommonUI commonUI, PreloaderUI preloaderUI,
            object raidSettings, object hideoutController, Action onLogoutPressed, Action reconnectAction);

        private MainMenuControllerHelper()
        {
            Execute += OnExecute;
        }

        private static async void OnExecute(MainMenuController __instance, Task<MainMenuController> __result,
            object backEnd, EnvironmentUI environmentUI, MenuUI menuUI, CommonUI commonUI, PreloaderUI preloaderUI,
            object raidSettings, object hideoutController, Action onLogoutPressed, Action reconnectAction)
        {
            Instance.MainMenuController = await __result;
        }
    }
}