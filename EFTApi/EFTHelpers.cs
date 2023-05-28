using EFTApi.Helpers;
// ReSharper disable InconsistentNaming

namespace EFTApi
{
    public static class EFTHelpers
    {
        /// <summary>
        /// GameUI Helper
        /// </summary>
        public static readonly GameUIHelper _GameUIHelper = new GameUIHelper();

        /// <summary>
        /// GameWorld Helper
        /// </summary>
        public static readonly GameWorldHelper _GameWorldHelper = new GameWorldHelper();

        /// <summary>
        /// Localized Helper
        /// </summary>
        public static readonly LocalizedHelper _LocalizedHelper = new LocalizedHelper();

        /// <summary>
        /// MainMenuController Helper
        /// </summary>
        public static readonly MainMenuControllerHelper _MainMenuControllerHelper = new MainMenuControllerHelper();

        /// <summary>
        /// Player Helper
        /// </summary>
        public static readonly PlayerHelper _PlayerHelper = new PlayerHelper();

        /// <summary>
        /// Session Helper
        /// </summary>
        public static readonly SessionHelper _SessionHelper = new SessionHelper();

        /// <summary>
        /// Quest Helper
        /// </summary>
        public static readonly QuestHelper _QuestHelper = new QuestHelper();

        /// <summary>
        /// Airdrop Helper
        /// </summary>
        public static readonly AirdropHelper _AirdropHelper = new AirdropHelper();
    }
}
