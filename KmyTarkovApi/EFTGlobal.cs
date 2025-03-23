using EFT;
using EFT.HealthSystem;
using EFT.InventoryLogic;
using EFT.UI;
using KmyTarkovApi.Helpers;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace KmyTarkovApi
{
    public static class EFTGlobal
    {
        /// <summary>
        ///     Current Session
        /// </summary>
        public static ISession Session => SessionHelper.Instance.Session;

        /// <summary>
        ///     Current LevelSettings
        /// </summary>
        public static LevelSettings LevelSettings => LevelSettingsHelper.Instance.LevelSettings;

        /// <summary>
        ///     Current GameWorld
        /// </summary>
        public static GameWorld GameWorld => GameWorldHelper.Instance.GameWorld;

        /// <summary>
        ///     Current Player
        /// </summary>
        public static Player Player => PlayerHelper.Instance.Player;

        /// <summary>
        ///     Current HealthController
        /// </summary>
        public static IHealthController HealthController => PlayerHelper.HealthControllerData.Instance.HealthController;

        /// <summary>
        ///     Current Inventory
        /// </summary>
        public static Inventory Inventory => PlayerHelper.InventoryData.Instance.Inventory;

        /// <summary>
        ///     Current FirearmController
        /// </summary>
        public static Player.FirearmController FirearmController =>
            PlayerHelper.FirearmControllerData.Instance.FirearmController;

        /// <summary>
        ///     Current Weapon
        /// </summary>
        public static Weapon Weapon => PlayerHelper.WeaponData.Instance.Weapon;

        /// <summary>
        ///     Current UnderbarrelWeapon
        /// </summary>
        public static LauncherItemClass UnderbarrelWeapon => PlayerHelper.WeaponData.Instance.UnderbarrelWeapon;

        /// <summary>
        ///     Current GamePlayerOwner
        /// </summary>
        public static GamePlayerOwner GamePlayerOwner => PlayerHelper.GamePlayerOwnerData.Instance.GamePlayerOwner;

        /// <summary>
        ///     Current MainMenuController
        /// </summary>
        public static MainMenuControllerClass MainMenuControllerClass =>
            MainMenuControllerClassHelper.Instance.MainMenuControllerClass;

        /// <summary>
        ///     Current BattleUIScreen
        /// </summary>
        public static EftBattleUIScreen EftBattleUIScreen => EftBattleUIScreenHelper.Instance.EftBattleUIScreen;

        /// <summary>
        ///     Current EnvironmentUIRoot
        /// </summary>
        public static EnvironmentUIRoot EnvironmentUIRoot => EnvironmentUIRootHelper.Instance.EnvironmentUIRoot;

        /// <summary>
        ///     Current AbstractGame
        /// </summary>
        public static AbstractGame AbstractGame => AbstractGameHelper.Instance.AbstractGame;
    }
}