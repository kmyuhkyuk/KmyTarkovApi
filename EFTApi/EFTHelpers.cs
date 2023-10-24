using EFTApi.Helpers;

// ReSharper disable UnusedType.Global

// ReSharper disable InconsistentNaming

namespace EFTApi
{
    public static class EFTHelpers
    {
        /// <summary>
        ///     GameUI Helper
        /// </summary>
        public static readonly GameUIHelper _GameUIHelper = GameUIHelper.Instance;

        /// <summary>
        ///     GameWorld Helper
        /// </summary>
        public static readonly GameWorldHelper _GameWorldHelper = GameWorldHelper.Instance;

        /// <summary>
        ///     Localized Helper
        /// </summary>
        public static readonly LocalizedHelper _LocalizedHelper = LocalizedHelper.Instance;

        /// <summary>
        ///     MainMenuController Helper
        /// </summary>
        public static readonly MainMenuControllerHelper _MainMenuControllerHelper = MainMenuControllerHelper.Instance;

        /// <summary>
        ///     Player Helper
        /// </summary>
        public static readonly PlayerHelper _PlayerHelper = PlayerHelper.Instance;

        /// <summary>
        ///     Session Helper
        /// </summary>
        public static readonly SessionHelper _SessionHelper = SessionHelper.Instance;

        /// <summary>
        ///     Quest Helper
        /// </summary>
        public static readonly QuestHelper _QuestHelper = QuestHelper.Instance;

        /// <summary>
        ///     Airdrop Helper
        /// </summary>
        public static readonly AirdropHelper _AirdropHelper = AirdropHelper.Instance;

        /// <summary>
        ///     EnvironmentUIRoot Helper
        /// </summary>
        public static readonly EnvironmentUIRootHelper _EnvironmentUIRootHelper = EnvironmentUIRootHelper.Instance;

        /// <summary>
        ///     AbstractGame Helper
        /// </summary>
        public static readonly AbstractGameHelper _AbstractGameHelper = AbstractGameHelper.Instance;
    }
}