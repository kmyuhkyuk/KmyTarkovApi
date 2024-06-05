using System;
using EFT.UI;
using EFTReflection;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Global

namespace EFTApi.Helpers
{
    public class BattleUIScreenHelper
    {
        private static readonly Lazy<BattleUIScreenHelper> Lazy =
            new Lazy<BattleUIScreenHelper>(() => new BattleUIScreenHelper());

        public static BattleUIScreenHelper Instance => Lazy.Value;

        public MonoBehaviour BattleUIScreen { get; private set; }

        public AmmoCountPanel AmmoCountPanel => RefAmmoCountPanel.GetValue(BattleUIScreen);

        /// <summary>
        ///     Init Action
        /// </summary>
        public readonly RefHelper.HookRef Constructor;

        public readonly RefHelper.FieldRef<MonoBehaviour, AmmoCountPanel> RefAmmoCountPanel;

        private BattleUIScreenHelper()
        {
            var battleUIScreenType = EFTVersion.AkiVersion > EFTVersion.Parse("3.8.3")
                ? RefTool.GetEftType(x => x.Name == "EftBattleUIScreen").BaseType
                : RefTool.GetEftType(x => x.Name == "BattleUIScreen");

            RefAmmoCountPanel =
                RefHelper.FieldRef<MonoBehaviour, AmmoCountPanel>.Create(battleUIScreenType, "_ammoCountPanel");

            Constructor = RefHelper.HookRef.Create(battleUIScreenType?.GetConstructors()[0]);

            Constructor.Add(this, nameof(OnConstructor));
        }

        private static void OnConstructor(MonoBehaviour __instance)
        {
            Instance.BattleUIScreen = __instance;
        }
    }
}