using System;
using EFT.UI;
using KmyTarkovReflection;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Global

namespace KmyTarkovApi.Helpers
{
    public class EftBattleUIScreenHelper
    {
        private static readonly Lazy<EftBattleUIScreenHelper> Lazy =
            new Lazy<EftBattleUIScreenHelper>(() => new EftBattleUIScreenHelper());

        public static EftBattleUIScreenHelper Instance => Lazy.Value;

        public EftBattleUIScreen EftBattleUIScreen { get; private set; }

        public AmmoCountPanel AmmoCountPanel => RefAmmoCountPanel.GetValue(EftBattleUIScreen);

        /// <summary>
        ///     Init Action
        /// </summary>
        public readonly RefHelper.HookRef Constructor;

        public readonly RefHelper.FieldRef<EftBattleUIScreen, AmmoCountPanel> RefAmmoCountPanel;

        private EftBattleUIScreenHelper()
        {
            var battleUIScreenBaseType = typeof(EftBattleUIScreen).BaseType;

            RefAmmoCountPanel =
                RefHelper.FieldRef<EftBattleUIScreen, AmmoCountPanel>.Create(battleUIScreenBaseType, "_ammoCountPanel");

            Constructor = RefHelper.HookRef.Create(battleUIScreenBaseType?.GetConstructors()[0]);
        }

        [EFTHelperHook]
        private void Hook()
        {
            Constructor.Add(this, nameof(OnConstructor));
        }

        private static void OnConstructor(EftBattleUIScreen __instance)
        {
            Instance.EftBattleUIScreen = __instance;
        }
    }
}