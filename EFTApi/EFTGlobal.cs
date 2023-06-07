using System.Collections.Generic;
using EFT;
using EFT.InventoryLogic;
using EFT.UI;
using EFTApi.Helpers;

namespace EFTApi
{
    public static class EFTGlobal
    {
        /// <summary>
        ///     Current Session
        /// </summary>
        public static ISession Session => SessionHelper.Instance.Session;

        /// <summary>
        ///     Current GameWorld
        /// </summary>
        public static GameWorld GameWorld => GameWorldHelper.Instance.GameWorld;

        /// <summary>
        ///     Current LevelSettings
        /// </summary>
        public static LevelSettings LevelSettings => GameWorldHelper.LevelSettingsData.Instance.LevelSettings;

        /// <summary>
        ///     Current Traders
        /// </summary>
        public static object[] Traders => SessionHelper.TradersData.Instance.Traders;

        /// <summary>
        ///     Current AllBot
        /// </summary>
        public static List<Player> AllBot => GameWorldHelper.Instance.AllBot;

        /// <summary>
        ///     Current Player
        /// </summary>
        public static Player Player => PlayerHelper.Instance.Player;

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
        public static Item UnderbarrelWeapon => PlayerHelper.WeaponData.Instance.UnderbarrelWeapon;

        /// <summary>
        ///     Current MainMenuController
        /// </summary>
        public static MainMenuController MainMenuController => MainMenuControllerHelper.Instance.MainMenuController;

        /// <summary>
        ///     Current GameUI
        /// </summary>
        public static GameUI GameUI => GameUIHelper.Instance.GameUI;
    }
}