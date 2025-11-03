using System;
using System.Reflection;
using BepInEx.Logging;
using JetBrains.Annotations;
using KmyTarkovApi.Helpers;
using KmyTarkovReflection;
using static KmyTarkovApi.Helpers.GameWorldHelper;
using static KmyTarkovApi.Helpers.PlayerHelper;
using static KmyTarkovApi.Helpers.PoolManagerClassHelper;
using static KmyTarkovApi.Helpers.SessionHelper;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace KmyTarkovApi
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    internal class EFTHelperHookAttribute : Attribute
    {
    }

    public static class EFTHelpers
    {
        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(EFTHelpers));

        /// <summary>
        ///     EftBattleUIScreen Helper
        /// </summary>
        public static EftBattleUIScreenHelper _EftBattleUIScreenHelper => EftBattleUIScreenHelper.Instance;

        /// <summary>
        ///     LevelSettings Helper
        /// </summary>
        public static LevelSettingsHelper _LevelSettingsHelper => LevelSettingsHelper.Instance;

        /// <summary>
        ///     GameWorld Helper
        /// </summary>
        public static GameWorldHelper _GameWorldHelper => GameWorldHelper.Instance;

        public static ZoneData _ZoneHelper => ZoneData.Instance;

        public static ExfiltrationPointData _ExfiltrationPointHelper => ExfiltrationPointData.Instance;

        /// <summary>
        ///     Localized Helper
        /// </summary>
        public static LocalizedHelper _LocalizedHelper => LocalizedHelper.Instance;

        /// <summary>
        ///     MainMenuControllerClass Helper
        /// </summary>
        public static MainMenuControllerClassHelper _MainMenuControllerClassHelper =>
            MainMenuControllerClassHelper.Instance;

        /// <summary>
        ///     Player Helper
        /// </summary>
        public static PlayerHelper _PlayerHelper => PlayerHelper.Instance;

        public static FirearmControllerData _FirearmControllerHelper => FirearmControllerData.Instance;

        public static ArmorComponentData _ArmorComponentHelper => ArmorComponentData.Instance;

        public static InventoryData _InventoryHelper => InventoryData.Instance;

        public static WeaponData _WeaponHelper => WeaponData.Instance;

        public static HealthControllerData _HealthControllerHelper => HealthControllerData.Instance;

        public static AbstractQuestControllerClassData _AbstractQuestControllerClassHelper =>
            AbstractQuestControllerClassData.Instance;

        public static ConditionCounterCreatorData _ConditionCounterCreatorHelper =>
            ConditionCounterCreatorData.Instance;

        public static ConditionCounterTemplateData _ConditionCounterTemplateHelper =>
            ConditionCounterTemplateData.Instance;

        public static GamePlayerOwnerData _GamePlayerOwnerHelper => GamePlayerOwnerData.Instance;

        /// <summary>
        ///     Session Helper
        /// </summary>
        public static SessionHelper _SessionHelper => SessionHelper.Instance;

        public static TraderSettingsData _TraderSettingsHelper => TraderSettingsData.Instance;

        public static ExperienceData _ExperienceHelper => ExperienceData.Instance;

        /// <summary>
        ///     AirdropLogicClass Helper
        /// </summary>
        public static AirdropLogicClassHelper _AirdropLogicClassHelper => AirdropLogicClassHelper.Instance;

        /// <summary>
        ///     EnvironmentUIRoot Helper
        /// </summary>
        public static EnvironmentUIRootHelper _EnvironmentUIRootHelper => EnvironmentUIRootHelper.Instance;

        /// <summary>
        ///     AbstractGame Helper
        /// </summary>
        public static AbstractGameHelper _AbstractGameHelper => AbstractGameHelper.Instance;

        /// <summary>
        ///     PoolManagerClass Helper
        /// </summary>
        public static PoolManagerClassHelper _PoolManagerClassHelper => PoolManagerClassHelper.Instance;

        public static JobPriorityData _JobPriorityHelper => JobPriorityData.Instance;

        /// <summary>
        ///     ResourceKeyManagerAbstractClass Helper
        /// </summary>
        public static ResourceKeyManagerAbstractClassHelper _ResourceKeyManagerAbstractClassHelper =>
            ResourceKeyManagerAbstractClassHelper.Instance;

        /// <summary>
        ///     EasyAssets Helper
        /// </summary>
        public static EasyAssetsHelper _EasyAssetsHelper => EasyAssetsHelper.Instance;

        public static EasyAssetsHelper.EasyAssetsExtensionData _EasyAssetsExtensionHelper =>
            EasyAssetsHelper.EasyAssetsExtensionData.Instance;

        internal static void InitHooks()
        {
            foreach (var propertyInfo in typeof(EFTHelpers).GetProperties(BindingFlags.Static | RefTool.Public))
            {
                foreach (var hookMethodInfo in propertyInfo.PropertyType.GetMethods(RefTool.NonPublic))
                {
                    if (hookMethodInfo.GetCustomAttribute<EFTHelperHookAttribute>() == null)
                        continue;

                    try
                    {
                        var instance = propertyInfo.PropertyType
                            .GetProperty("Instance", BindingFlags.Static | RefTool.Public)
                            ?.GetValue(null);

                        hookMethodInfo.Invoke(instance, null);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex);
                    }
                }
            }
        }
    }
}