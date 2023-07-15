using EFT.UI;
using EFTReflection;

namespace EFTApi.Helpers
{
    public class GameUIHelper
    {
        public static readonly GameUIHelper Instance = new GameUIHelper();

        public GameUI GameUI { get; private set; }

        public readonly RefHelper.HookRef Awake = new RefHelper.HookRef(typeof(GameUI), "Awake");

        public readonly RefHelper.HookRef OnDestroy = new RefHelper.HookRef(typeof(GameUI), "OnDestroy");

        private GameUIHelper()
        {
            Awake.Add(this, nameof(OnAwake));
        }

        private static void OnAwake(GameUI __instance)
        {
            Instance.GameUI = __instance;
        }
    }
}