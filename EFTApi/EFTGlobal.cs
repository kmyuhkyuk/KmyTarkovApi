using System.Collections.Generic;
using EFT;
using EFT.InventoryLogic;
using EFT.UI;

namespace EFTApi
{
    public static class EFTGlobal
    {
        /// <summary>
        /// Current Session
        /// </summary>
        public static ISession Session => EFTHelpers._SessionHelper.Session;

        /// <summary>
        /// Current GameWorld
        /// </summary>
        public static GameWorld GameWorld => EFTHelpers._GameWorldHelper.GameWorld;

        /// <summary>
        /// Current LevelSettings
        /// </summary>
        public static LevelSettings LevelSettings => EFTHelpers._GameWorldHelper.LevelSettingsHelper.LevelSettings;

        /// <summary>
        /// Current Traders
        /// </summary>
        public static object[] Traders => EFTHelpers._SessionHelper.TradersHelper.Traders;

        /// <summary>
        /// Current AllBot
        /// </summary>
        public static List<Player> AllBot => EFTHelpers._GameWorldHelper.AllBot;

        /// <summary>
        /// Current Player
        /// </summary>
        public static Player Player => EFTHelpers._PlayerHelper.Player;

        /// <summary>
        /// Current FirearmController
        /// </summary>
        public static Player.FirearmController FirearmController => EFTHelpers._PlayerHelper.FirearmControllerHelper.FirearmController;

        /// <summary>
        /// Current Weapon
        /// </summary>
        public static Weapon Weapon => EFTHelpers._PlayerHelper.WeaponHelper.Weapon;

        /// <summary>
        /// Current UnderbarrelWeapon
        /// </summary>
        public static Item UnderbarrelWeapon => EFTHelpers._PlayerHelper.WeaponHelper.UnderbarrelWeapon;

        /// <summary>
        /// Current MainMenuController
        /// </summary>
        public static MainMenuController MainMenuController => EFTHelpers._MainMenuControllerHelper.MainMenuController;

        /// <summary>
        /// Current GameUI
        /// </summary>
        public static GameUI GameUI => EFTHelpers._GameUIHelper.GameUI;
    }
}
