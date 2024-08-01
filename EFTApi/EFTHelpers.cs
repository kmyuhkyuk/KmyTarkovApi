using System;
using System.Reflection;
using BepInEx.Logging;
using EFTApi.Helpers;
using EFTReflection;
using static EFTApi.Helpers.AirdropHelper;
using static EFTApi.Helpers.GameWorldHelper;
using static EFTApi.Helpers.GameWorldHelper.ExfiltrationControllerData;
using static EFTApi.Helpers.PlayerHelper;
using static EFTApi.Helpers.PoolManagerHelper;
using static EFTApi.Helpers.SessionHelper;
using static EFTApi.Helpers.SessionHelper.TradersData;

// ReSharper disable UnassignedReadonlyField
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace EFTApi
{
    public static class EFTHelpers
    {
        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(EFTHelpers));

        /// <summary>
        ///     BattleUIScreen Helper
        /// </summary>
        public static readonly BattleUIScreenHelper _BattleUIScreenHelper;

        /// <summary>
        ///     LevelSettings Helper
        /// </summary>
        public static readonly LevelSettingsHelper _LevelSettingsHelper;

        /// <summary>
        ///     GameWorld Helper
        /// </summary>
        public static readonly GameWorldHelper _GameWorldHelper;

        public static readonly ZoneData _ZoneHelper;

        public static readonly LootableContainerData _LootableContainerHelper;

        public static readonly SearchableItemClassData _SearchableItemClassHelper;

        public static readonly ExfiltrationControllerData _ExfiltrationControllerHelper;

        public static readonly ExfiltrationPointData _ExfiltrationPointHelper;

        /// <summary>
        ///     Localized Helper
        /// </summary>
        public static readonly LocalizedHelper _LocalizedHelper;

        /// <summary>
        ///     MainMenuController Helper
        /// </summary>
        public static readonly MainMenuControllerHelper _MainMenuControllerHelper;

        /// <summary>
        ///     Player Helper
        /// </summary>
        public static readonly PlayerHelper _PlayerHelper;

        public static readonly FirearmControllerData _FirearmControllerHelper;

        public static readonly ArmorComponentData _ArmorComponentHelper;

        public static readonly RoleData _RoleHelper;

        public static readonly InventoryData _InventoryHelper;

        public static readonly WeaponData _WeaponHelper;

        public static readonly DamageInfoData _DamageInfoHelper;

        public static readonly SpeakerData _SpeakerHelper;

        public static readonly HealthControllerData _HealthControllerHelper;

        public static readonly GamePlayerOwnerData _GamePlayerOwnerHelper;

        public static readonly MovementContextData _MovementContextHelper;

        public static readonly QuestControllerData _QuestControllerHelper;

        public static readonly InventoryControllerData _InventoryControllerHelper;

        /// <summary>
        ///     Session Helper
        /// </summary>
        public static readonly SessionHelper _SessionHelper;

        public static readonly TradersData _TradersHelper;

        public static readonly TradersAvatarData _TradersAvatarHelper;

        public static readonly ExperienceData _ExperienceHelper;

        /// <summary>
        ///     Quest Helper
        /// </summary>
        public static readonly QuestHelper _QuestHelper;

        /// <summary>
        ///     Airdrop Helper
        /// </summary>
        public static readonly AirdropHelper _AirdropHelper;

        public static readonly AirdropBoxData _AirdropBoxHelper;

        public static readonly AirdropSynchronizableObjectData _AirdropSynchronizableObjectHelper;

        public static readonly AirdropLogicClassData _AirdropLogicClassHelper;

        /// <summary>
        ///     EnvironmentUIRoot Helper
        /// </summary>
        public static readonly EnvironmentUIRootHelper _EnvironmentUIRootHelper;

        /// <summary>
        ///     AbstractGame Helper
        /// </summary>
        public static readonly AbstractGameHelper _AbstractGameHelper;

        /// <summary>
        ///     PoolManager Helper
        /// </summary>
        public static readonly PoolManagerHelper _PoolManagerHelper;

        public static readonly JobPriorityData _JobPriorityHelper;

        /// <summary>
        ///     Voice Helper
        /// </summary>
        public static readonly VoiceHelper _VoiceHelper;

        /// <summary>
        ///     EasyAssets Helper
        /// </summary>
        public static readonly EasyAssetsHelper _EasyAssetsHelper;

        public static readonly EasyAssetsHelper.EasyAssetsExtensionData _EasyAssetsExtensionHelper;

        /// <summary>
        ///     RequestHandler Helper
        /// </summary>
        public static readonly RequestHandlerHelper _RequestHandlerHelper;

        static EFTHelpers()
        {
            foreach (var fieldInfo in typeof(EFTHelpers).GetFields(BindingFlags.Static | RefTool.Public))
            {
                try
                {
                    var instance = fieldInfo.FieldType.GetProperty("Instance", BindingFlags.Static | RefTool.Public)
                        ?.GetValue(null);

                    fieldInfo.SetValue(null, instance);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                }
            }
        }
    }
}