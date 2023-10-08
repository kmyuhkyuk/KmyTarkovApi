using EFT.UI;
using EFTReflection;

// ReSharper disable MemberCanBePrivate.Global

namespace EFTApi.Helpers
{
    public class GameUIHelper
    {
        public static readonly GameUIHelper Instance = new GameUIHelper();

        public GameUI GameUI { get; private set; }

        /// <summary>
        ///     Init Action
        /// </summary>
        public readonly RefHelper.HookRef Awake;

        /// <summary>
        ///     Destroy Action
        /// </summary>
        public readonly RefHelper.HookRef OnDestroy;

        private GameUIHelper()
        {
            var gameUIType = typeof(GameUI);

            Awake = RefHelper.HookRef.Create(gameUIType, "Awake");
            OnDestroy = RefHelper.HookRef.Create(gameUIType, "OnDestroy");

            Awake.Add(this, nameof(OnAwake));
        }

        private static void OnAwake(GameUI __instance)
        {
            Instance.GameUI = __instance;
        }
    }
}