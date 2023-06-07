using EFT.UI;
using EFTReflection;
using EFTReflection.Patching;

namespace EFTApi.Helpers
{
    public class GameUIHelper
    {
        public static readonly GameUIHelper Instance = new GameUIHelper();

        public GameUI GameUI { get; private set; }

        public event hook_Awake Awake
        {
            add => HookPatch.Add(typeof(GameUI).GetMethod("Awake", RefTool.Public), value);
            remove => HookPatch.Remove(typeof(GameUI).GetMethod("Awake", RefTool.Public), value);
        }

        public delegate void hook_Awake(GameUI __instance);

        public event hook_OnDestroy OnDestroy
        {
            add => HookPatch.Add(typeof(GameUI).GetMethod("OnDestroy", RefTool.Public), value);
            remove => HookPatch.Remove(typeof(GameUI).GetMethod("OnDestroy", RefTool.Public), value);
        }

        public delegate void hook_OnDestroy(GameUI __instance);

        private GameUIHelper()
        {
            Awake += OnAwake;
        }

        private static void OnAwake(GameUI __instance)
        {
            Instance.GameUI = __instance;
        }
    }
}