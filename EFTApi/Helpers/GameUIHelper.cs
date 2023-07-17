using EFT.UI;
using EFTReflection;

namespace EFTApi.Helpers
{
    public class GameUIHelper
    {
        public static readonly GameUIHelper Instance = new GameUIHelper();

        public GameUI GameUI { get; private set; }

        public readonly RefHelper.HookRef Awake;

        public readonly RefHelper.HookRef OnDestroy;

        private GameUIHelper()
        {
            Awake = new RefHelper.HookRef(typeof(GameUI), "Awake");
            OnDestroy = new RefHelper.HookRef(typeof(GameUI), "OnDestroy");

            Awake.Add(this, nameof(OnAwake));
        }

        private static void OnAwake(GameUI __instance)
        {
            Instance.GameUI = __instance;
        }
    }
}