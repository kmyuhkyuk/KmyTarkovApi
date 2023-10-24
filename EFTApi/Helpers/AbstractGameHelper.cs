using EFT;
using EFTReflection;

// ReSharper disable MemberCanBePrivate.Global

namespace EFTApi.Helpers
{
    public class AbstractGameHelper
    {
        public static readonly AbstractGameHelper Instance = new AbstractGameHelper();

        public AbstractGame AbstractGame { get; private set; }

        /// <summary>
        ///     Init Action
        /// </summary>
        public readonly RefHelper.HookRef Constructor;

        private AbstractGameHelper()
        {
            Constructor = RefHelper.HookRef.Create(typeof(AbstractGame).GetConstructors(RefTool.NonPublic)[0]);

            Constructor.Add(this, nameof(OnConstructor));
        }

        private static void OnConstructor(AbstractGame __instance)
        {
            Instance.AbstractGame = __instance;
        }
    }
}