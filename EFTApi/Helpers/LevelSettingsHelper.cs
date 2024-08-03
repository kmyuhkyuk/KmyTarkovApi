using System;
using EFTReflection;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Global

namespace EFTApi.Helpers
{
    public class LevelSettingsHelper
    {
        private static readonly Lazy<LevelSettingsHelper> Lazy =
            new Lazy<LevelSettingsHelper>(() => new LevelSettingsHelper());

        public static LevelSettingsHelper Instance => Lazy.Value;

        public LevelSettings LevelSettings { get; private set; }

        /// <summary>
        ///     Init Action
        /// </summary>
        public readonly RefHelper.HookRef Awake;

        /// <summary>
        ///     Destroy Action
        /// </summary>
        public readonly RefHelper.HookRef OnDestroy;

        private LevelSettingsHelper()
        {
            var levelSettingsType = typeof(LevelSettings);

            Awake = RefHelper.HookRef.Create(levelSettingsType, "Awake");
            OnDestroy = RefHelper.HookRef.Create(levelSettingsType, "OnDestroy");
        }

        [EFTHelperHook]
        private void Hook()
        {
            Awake.Add(this, nameof(OnAwake));
        }

        private static void OnAwake(LevelSettings __instance)
        {
            Instance.LevelSettings = __instance;
        }
    }
}