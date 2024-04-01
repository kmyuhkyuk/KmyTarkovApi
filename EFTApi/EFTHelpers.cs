using EFTApi.Helpers;
using static EFTApi.Helpers.AirdropHelper;
using static EFTApi.Helpers.GameWorldHelper;
using static EFTApi.Helpers.GameWorldHelper.ExfiltrationControllerData;
using static EFTApi.Helpers.PlayerHelper;
using static EFTApi.Helpers.SessionHelper;
using static EFTApi.Helpers.SessionHelper.TradersData;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace EFTApi
{
    public static class EFTHelpers
    {
        /// <summary>
        ///     GameUI Helper
        /// </summary>
        public static GameUIHelper _GameUIHelper = GameUIHelper.Instance;

        /// <summary>
        ///     LevelSettings Helper
        /// </summary>
        public static LevelSettingsHelper _LevelSettingsHelper = LevelSettingsHelper.Instance;

        /// <summary>
        ///     GameWorld Helper
        /// </summary>
        public static GameWorldHelper _GameWorldHelper = GameWorldHelper.Instance;

        public static ZoneData _ZoneHelper = ZoneData.Instance;

        public static LootableContainerData _LootableContainerHelper = LootableContainerData.Instance;

        public static SearchableItemClassData _SearchableItemClassHelper = SearchableItemClassData.Instance;

        public static ExfiltrationControllerData _ExfiltrationControllerHelper = ExfiltrationControllerData.Instance;

        public static ExfiltrationPointData _ExfiltrationPointHelper = ExfiltrationPointData.Instance;

        /// <summary>
        ///     Localized Helper
        /// </summary>
        public static LocalizedHelper _LocalizedHelper = LocalizedHelper.Instance;

        /// <summary>
        ///     MainMenuController Helper
        /// </summary>
        public static MainMenuControllerHelper _MainMenuControllerHelper = MainMenuControllerHelper.Instance;

        /// <summary>
        ///     Player Helper
        /// </summary>
        public static PlayerHelper _PlayerHelper = PlayerHelper.Instance;

        public static FirearmControllerData _FirearmControllerHelper = FirearmControllerData.Instance;

        public static ArmorComponentData _ArmorComponentHelper = ArmorComponentData.Instance;

        public static RoleData _RoleHelper = RoleData.Instance;

        public static InventoryData _InventoryHelper = InventoryData.Instance;

        public static WeaponData _WeaponHelper = WeaponData.Instance;

        public static DamageInfoData _DamageInfoHelper = DamageInfoData.Instance;

        public static SpeakerData _SpeakerHelper = SpeakerData.Instance;

        public static HealthControllerData _HealthControllerHelper = HealthControllerData.Instance;

        public static GamePlayerOwnerData _GamePlayerOwnerHelper = GamePlayerOwnerData.Instance;

        public static MovementContextData _MovementContextHelper = MovementContextData.Instance;

        /// <summary>
        ///     Session Helper
        /// </summary>
        public static SessionHelper _SessionHelper = SessionHelper.Instance;

        public static TradersData _TradersHelper = TradersData.Instance;

        public static TradersAvatarData _TradersAvatarHelper = TradersAvatarData.Instance;

        public static ExperienceData _ExperienceHelper = ExperienceData.Instance;

        /// <summary>
        ///     Quest Helper
        /// </summary>
        public static QuestHelper _QuestHelper = QuestHelper.Instance;

        /// <summary>
        ///     Airdrop Helper
        /// </summary>
        public static AirdropHelper _AirdropHelper = AirdropHelper.Instance;

        public static AirdropBoxData _AirdropBoxHelper = AirdropBoxData.Instance;

        public static AirdropSynchronizableObjectData _AirdropSynchronizableObjectHelper =
            AirdropSynchronizableObjectData.Instance;

        /// <summary>
        ///     EnvironmentUIRoot Helper
        /// </summary>
        public static EnvironmentUIRootHelper _EnvironmentUIRootHelper = EnvironmentUIRootHelper.Instance;

        /// <summary>
        ///     AbstractGame Helper
        /// </summary>
        public static AbstractGameHelper _AbstractGameHelper = AbstractGameHelper.Instance;
    }
}