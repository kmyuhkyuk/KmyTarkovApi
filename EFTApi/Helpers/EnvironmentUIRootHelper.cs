using System;
using EFT.UI;
using EFTReflection;

// ReSharper disable MemberCanBePrivate.Global

namespace EFTApi.Helpers
{
    public class EnvironmentUIRootHelper
    {
        private static readonly Lazy<EnvironmentUIRootHelper> Lazy =
            new Lazy<EnvironmentUIRootHelper>(() => new EnvironmentUIRootHelper());

        public static EnvironmentUIRootHelper Instance => Lazy.Value;

        public EnvironmentUIRoot EnvironmentUIRoot { get; private set; }

        /// <summary>
        ///     Init Action
        /// </summary>
        public readonly RefHelper.HookRef Init;

        private EnvironmentUIRootHelper()
        {
            var environmentUIRootType = typeof(EnvironmentUIRoot);

            Init = RefHelper.HookRef.Create(environmentUIRootType, "Init");

            Init.Add(this, nameof(OnInit));
        }

        private static void OnInit(EnvironmentUIRoot __instance)
        {
            Instance.EnvironmentUIRoot = __instance;
        }
    }
}