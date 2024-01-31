using System;
using EFT;
using EFTReflection;

// ReSharper disable MemberCanBePrivate.Global

namespace EFTApi.Helpers
{
    public class AbstractGameHelper
    {
        private static readonly Lazy<AbstractGameHelper> Lazy =
            new Lazy<AbstractGameHelper>(() => new AbstractGameHelper());

        public static AbstractGameHelper Instance => Lazy.Value;

        public AbstractGame AbstractGame { get; private set; }

        /// <summary>
        ///     Init Action
        /// </summary>
        public readonly RefHelper.HookRef Constructor;

        private AbstractGameHelper()
        {
            var abstractGameType = typeof(AbstractGame);

            Constructor = RefHelper.HookRef.Create(abstractGameType.GetConstructors(RefTool.NonPublic)[0]);

            Constructor.Add(this, nameof(OnConstructor));
        }

        private static void OnConstructor(AbstractGame __instance)
        {
            Instance.AbstractGame = __instance;
        }
    }
}