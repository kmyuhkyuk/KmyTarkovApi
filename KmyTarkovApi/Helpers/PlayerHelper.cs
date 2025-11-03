using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using EFT;
using EFT.HealthSystem;
using EFT.InventoryLogic;
using EFT.Quests;
using HarmonyLib;
using JetBrains.Annotations;
using KmyTarkovReflection;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeMadeStatic.Global
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable NotAccessedField.Global
// ReSharper disable UnusedMember.Global

namespace KmyTarkovApi.Helpers
{
    public class PlayerHelper
    {
        private static readonly Lazy<PlayerHelper> Lazy = new Lazy<PlayerHelper>(() => new PlayerHelper());

        public static PlayerHelper Instance => Lazy.Value;

        public Player Player { get; private set; }

        public FirearmControllerData FirearmControllerHelper => FirearmControllerData.Instance;

        public WeaponData WeaponHelper => WeaponData.Instance;

        public ArmorComponentData ArmorComponentHelper => ArmorComponentData.Instance;

        public InventoryData InventoryHelper => InventoryData.Instance;

        public HealthControllerData HealthControllerHelper => HealthControllerData.Instance;

        public AbstractQuestControllerClassData AbstractQuestControllerClassHelper =>
            AbstractQuestControllerClassData.Instance;

        public ConditionCounterCreatorData ConditionCounterCreatorHelper => ConditionCounterCreatorData.Instance;

        public ConditionCounterTemplateData ConditionCounterTemplateHelper => ConditionCounterTemplateData.Instance;

        public GamePlayerOwnerData GamePlayerOwnerHelper => GamePlayerOwnerData.Instance;

        /// <summary>
        ///     Init Action
        /// </summary>
        public readonly RefHelper.HookRef Init;

        /// <summary>
        ///     Dispose Action
        /// </summary>
        public readonly RefHelper.HookRef Dispose;

        public readonly RefHelper.HookRef OnDead;

        public readonly RefHelper.HookRef ApplyDamageInfo;

        private readonly Func<Player, EBodyPartColliderType, bool> _refGetBleedBlock;

        /// <summary>
        ///     Fika.Core.Main.Players.ObservedPlayer.ApplyShot
        /// </summary>
        [CanBeNull] public readonly RefHelper.HookRef ObservedCoopApplyShot;

        public readonly RefHelper.HookRef OnBeenKilledByAggressor;

        public readonly RefHelper.HookRef OnPhraseTold;

        /// <summary>
        ///     Fika.Core.Main.Players.ObservedPlayer.OnPhraseTold
        /// </summary>
        [CanBeNull] public readonly RefHelper.HookRef ObservedCoopOnPhraseTold;

        public readonly RefHelper.HookRef SetPropVisibility;

        private PlayerHelper()
        {
            var playerType = typeof(Player);

            Init = RefHelper.HookRef.Create(playerType, "Init");
            Dispose = RefHelper.HookRef.Create(playerType, "Dispose");
            OnDead = RefHelper.HookRef.Create(playerType, "OnDead");
            ApplyDamageInfo = RefHelper.HookRef.Create(playerType, "ApplyDamageInfo");
            OnBeenKilledByAggressor = RefHelper.HookRef.Create(playerType, "OnBeenKilledByAggressor");
            OnPhraseTold = RefHelper.HookRef.Create(playerType, "OnPhraseTold");
            SetPropVisibility = RefHelper.HookRef.Create(playerType, "SetPropVisibility");

            _refGetBleedBlock =
                AccessTools.MethodDelegate<Func<Player, EBodyPartColliderType, bool>>(
                    RefTool.GetEftMethod(playerType, AccessTools.allDeclared,
                        x => x.ReturnType == typeof(bool) && x.ReadMethodBody().ContainsIL(OpCodes.Ldfld,
                            AccessTools.Field(typeof(SkillManager), "LightVestBleedingProtection"))));

            if (EFTVersion.IsFika)
            {
                var observedCoopPlayerType = RefTool.GetPluginType(EFTPlugins.FikaCore,
                    "Fika.Core.Main.Players.ObservedPlayer");

                ObservedCoopApplyShot = RefHelper.HookRef.Create(observedCoopPlayerType, "ApplyShot");
                ObservedCoopOnPhraseTold = RefHelper.HookRef.Create(observedCoopPlayerType, "OnPhraseTold");
            }
        }

        [EFTHelperHook]
        private void Hook()
        {
            Init.Add(this, nameof(OnInit));
        }

        private static async void OnInit(Player __instance, Task __result)
        {
            await __result;

            if (__instance.IsYourPlayer)
            {
                Instance.Player = __instance;
            }
        }

        public bool CoopGetBleedBlock(Player instance, EBodyPartColliderType colliderType)
        {
            return _refGetBleedBlock(instance, colliderType);
        }

        public class FirearmControllerData
        {
            private static readonly Lazy<FirearmControllerData> Lazy =
                new Lazy<FirearmControllerData>(() => new FirearmControllerData());

            public static FirearmControllerData Instance => Lazy.Value;

            public Player.FirearmController FirearmController =>
                PlayerHelper.Instance.Player?.HandsController as Player.FirearmController;

            public readonly RefHelper.HookRef InitiateShot;

            private FirearmControllerData()
            {
                var playerFirearmControllerType = typeof(Player.FirearmController);

                InitiateShot = RefHelper.HookRef.Create(playerFirearmControllerType, "InitiateShot");
            }
        }

        public class ArmorComponentData
        {
            private static readonly Lazy<ArmorComponentData> Lazy =
                new Lazy<ArmorComponentData>(() => new ArmorComponentData());

            public static ArmorComponentData Instance => Lazy.Value;

            public readonly RefHelper.HookRef ApplyDamage;

            private ArmorComponentData()
            {
                var armorComponentType = typeof(ArmorComponent);

                ApplyDamage = RefHelper.HookRef.Create(armorComponentType, "ApplyDamage");
            }
        }

        public class InventoryData
        {
            private static readonly Lazy<InventoryData> Lazy = new Lazy<InventoryData>(() => new InventoryData());

            public static InventoryData Instance => Lazy.Value;

            public Inventory Inventory => PlayerHelper.Instance.Player?.Inventory;

            public List<StashGridClass> EquipmentGrids
            {
                get
                {
                    var equipmentSlots = Inventory.Equipment.Slots;

                    if (equipmentSlots == null)
                        return null;

                    var list = new List<StashGridClass>();

                    foreach (var slot in new[]
                                 { equipmentSlots[6], equipmentSlots[7], equipmentSlots[8], equipmentSlots[10] })
                    {
                        if (slot.ContainedItem is CompoundItem gear)
                        {
                            foreach (var grid in gear.Grids)
                            {
                                list.Add(grid);
                            }
                        }
                    }

                    return list;
                }
            }

            public List<Item> EquipmentItems
            {
                get
                {
                    var equipmentGrids = EquipmentGrids;

                    if (equipmentGrids == null)
                        return null;

                    var list = new List<Item>();

                    foreach (var grid in equipmentGrids)
                    {
                        foreach (var item in grid.Items)
                        {
                            list.Add(item);
                        }
                    }

                    return list;
                }
            }

            public HashSet<MongoID> EquipmentItemHashSet
            {
                get
                {
                    var equipmentGrids = EquipmentGrids;

                    if (equipmentGrids == null)
                        return null;

                    var hashSet = new HashSet<MongoID>();

                    // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                    foreach (var grid in equipmentGrids)
                    {
                        foreach (var item in grid.Items)
                        {
                            hashSet.Add(item.TemplateId);
                        }
                    }

                    return hashSet;
                }
            }

            public List<StashGridClass> QuestRaidItemsGrids
            {
                get
                {
                    var questRaidItems = Inventory.QuestRaidItems;

                    if (questRaidItems == null)
                        return null;

                    var list = new List<StashGridClass>();

                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var grid in questRaidItems.Grids)
                    {
                        list.Add(grid);
                    }

                    return list;
                }
            }

            public List<Item> QuestRaidItemsItems
            {
                get
                {
                    var questRaidItemsGrids = QuestRaidItemsGrids;

                    if (questRaidItemsGrids == null)
                        return null;

                    var list = new List<Item>();

                    foreach (var grid in questRaidItemsGrids)
                    {
                        foreach (var item in grid.Items)
                        {
                            list.Add(item);
                        }
                    }

                    return list;
                }
            }

            public HashSet<MongoID> QuestRaidItemHashSet
            {
                get
                {
                    var questRaidItemsGrids = QuestRaidItemsGrids;

                    if (questRaidItemsGrids == null)
                        return null;

                    var hashSet = new HashSet<MongoID>();

                    // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                    foreach (var grid in questRaidItemsGrids)
                    {
                        foreach (var item in grid.Items)
                        {
                            hashSet.Add(item.TemplateId);
                        }
                    }

                    return hashSet;
                }
            }

            private InventoryData()
            {
            }
        }

        public class WeaponData
        {
            private static readonly Lazy<WeaponData> Lazy = new Lazy<WeaponData>(() => new WeaponData());

            public static WeaponData Instance => Lazy.Value;

            public Weapon Weapon => FirearmControllerData.Instance.FirearmController?.Item;

            public LauncherItemClass UnderbarrelWeapon =>
                FirearmControllerData.Instance.FirearmController?.UnderbarrelWeapon;

            public Animator WeaponAnimator =>
                RefAnimator.GetValue(PlayerHelper.Instance.Player?.ArmsAnimatorCommon);

            public Animator LauncherAnimator =>
                RefAnimator.GetValue(PlayerHelper.Instance.Player?.UnderbarrelWeaponArmsAnimator);

            /// <summary>
            ///     IAnimator.Animator
            /// </summary>
            public readonly RefHelper.PropertyRef<object, Animator> RefAnimator;

            private WeaponData()
            {
                RefAnimator = RefHelper.PropertyRef<object, Animator>.Create(
                    RefTool.GetEftType(x =>
                        x.GetMethod("CreateAnimatorStateInfoWrapper", RefTool.Public | BindingFlags.Static) != null),
                    "Animator");
            }
        }

        public class HealthControllerData
        {
            private static readonly Lazy<HealthControllerData> Lazy =
                new Lazy<HealthControllerData>(() => new HealthControllerData());

            public static HealthControllerData Instance => Lazy.Value;

            public IHealthController HealthController => PlayerHelper.Instance.Player?.HealthController;

            public readonly RefHelper.PropertyRef<IHealthController, ValueStruct> RefHydration;

            public readonly RefHelper.PropertyRef<IHealthController, ValueStruct> RefEnergy;

            private readonly Func<IHealthController, EBodyPart, bool, ValueStruct> _refGetBodyPartHealth;

            /// <summary>
            ///     Fika.Core.Main.ClientClasses.ClientHealthController.ApplyDamage
            /// </summary>
            private readonly Func<ActiveHealthController, EBodyPart, float, DamageInfoStruct, float>
                _refCoopApplyDamage;

            /// <summary>
            ///     Fika.Core.Main.ObservedClasses.ObservedHealthController.Store
            /// </summary>
            private readonly
                Func<NetworkHealthControllerAbstractClass, Profile.ProfileHealthClass, Profile.ProfileHealthClass>
                _refObservedCoopStore;

            private readonly Type _coopHealthControllerType;

            private HealthControllerData()
            {
                var healthControllerBaseInterfaceType = typeof(IHealthController).GetInterfaces()[0];

                RefHydration =
                    RefHelper.PropertyRef<IHealthController, ValueStruct>.Create(healthControllerBaseInterfaceType,
                        "Hydration");
                RefEnergy = RefHelper.PropertyRef<IHealthController, ValueStruct>.Create(
                    healthControllerBaseInterfaceType, "Energy");

                var activeHealthControllerBaseType = typeof(ActiveHealthController).BaseType;

                _refGetBodyPartHealth =
                    RefHelper.ObjectMethodDelegate<Func<IHealthController, EBodyPart, bool, ValueStruct>>(
                        activeHealthControllerBaseType?.GetMethod("GetBodyPartHealth", RefTool.Public));

                if (!EFTVersion.IsFika)
                    return;

                _coopHealthControllerType = RefTool.GetPluginType(EFTPlugins.FikaCore,
                    "Fika.Core.Main.ClientClasses.ClientHealthController");

                _refObservedCoopStore =
                    RefHelper
                        .ObjectMethodDelegate<Func<NetworkHealthControllerAbstractClass, Profile.ProfileHealthClass,
                            Profile.ProfileHealthClass>>(RefTool
                            .GetPluginType(EFTPlugins.FikaCore,
                                "Fika.Core.Main.ObservedClasses.ObservedHealthController")
                            .GetMethod("Store", RefTool.Public));

                _refCoopApplyDamage =
                    RefHelper
                        .ObjectMethodDelegate<Func<ActiveHealthController, EBodyPart, float, DamageInfoStruct, float>>(
                            _coopHealthControllerType.GetMethod("ApplyDamage", RefTool.Public));
            }

            public ValueStruct GetBodyPartHealth(IHealthController instance, EBodyPart bodyPart, bool rounded = false)
            {
                return _refGetBodyPartHealth(instance, bodyPart, rounded);
            }

            public float CoopApplyDamage(ActiveHealthController instance, EBodyPart bodyPart, float damage,
                DamageInfoStruct damageInfo)
            {
                return _refCoopApplyDamage(instance, bodyPart, damage, damageInfo);
            }

            public ActiveHealthController CoopHealthControllerCreate(Profile.ProfileHealthClass healthInfo,
                Player player, InventoryController inventoryController,
                SkillManager skillManager, bool aiHealth)
            {
                return (ActiveHealthController)Activator.CreateInstance(_coopHealthControllerType, healthInfo, player,
                    inventoryController,
                    skillManager, aiHealth);
            }

            public Profile.ProfileHealthClass ObservedCoopStore(NetworkHealthControllerAbstractClass instance,
                Profile.ProfileHealthClass healthInfo = null)
            {
                return _refObservedCoopStore(instance, healthInfo);
            }
        }

        public class AbstractQuestControllerClassData
        {
            private static readonly Lazy<AbstractQuestControllerClassData> Lazy =
                new Lazy<AbstractQuestControllerClassData>(() => new AbstractQuestControllerClassData());

            public static AbstractQuestControllerClassData Instance => Lazy.Value;

            public AbstractQuestControllerClass AbstractQuestControllerClass =>
                RefAbstractQuestControllerClass.GetValue(PlayerHelper.Instance.Player);

            public IEnumerable<QuestClass> Quests => RefQuests.GetValue(AbstractQuestControllerClass);

            public readonly RefHelper.PropertyRef<Player, AbstractQuestControllerClass> RefAbstractQuestControllerClass;

            public readonly RefHelper.PropertyRef<AbstractQuestControllerClass, IEnumerable<QuestClass>> RefQuests;

            public readonly RefHelper.HookRef OnConditionValueChanged;

            private AbstractQuestControllerClassData()
            {
                RefAbstractQuestControllerClass =
                    RefHelper.PropertyRef<Player, AbstractQuestControllerClass>.Create("AbstractQuestControllerClass");

                RefQuests =
                    RefHelper.PropertyRef<AbstractQuestControllerClass, IEnumerable<QuestClass>>.Create("Quests");

                var abstractQuestControllerClassBaseType = typeof(AbstractQuestControllerClass).BaseType;

                OnConditionValueChanged =
                    RefHelper.HookRef.Create(abstractQuestControllerClassBaseType, "OnConditionValueChanged");
            }
        }

        public class ConditionCounterCreatorData
        {
            private static readonly Lazy<ConditionCounterCreatorData> Lazy =
                new Lazy<ConditionCounterCreatorData>(() => new ConditionCounterCreatorData());

            public static ConditionCounterCreatorData Instance => Lazy.Value;

            public readonly
                RefHelper.FieldRef<ConditionCounterCreator, ConditionCounterCreator.ConditionCounterTemplate>
                RefTemplateConditions;

            private ConditionCounterCreatorData()
            {
                RefTemplateConditions =
                    RefHelper.FieldRef<ConditionCounterCreator, ConditionCounterCreator.ConditionCounterTemplate>
                        .Create(EFTVersion.SPTVersion > EFTVersion.Parse("3.11.4")
                            ? "TemplateConditions"
                            : "_templateConditions");
            }
        }

        public class ConditionCounterTemplateData
        {
            private static readonly Lazy<ConditionCounterTemplateData> Lazy =
                new Lazy<ConditionCounterTemplateData>(() => new ConditionCounterTemplateData());

            public static ConditionCounterTemplateData Instance => Lazy.Value;

            public readonly RefHelper.FieldRef<ConditionCounterCreator.ConditionCounterTemplate, IEnumerable<Condition>>
                RefConditions;

            private ConditionCounterTemplateData()
            {
                RefConditions =
                    RefHelper.FieldRef<ConditionCounterCreator.ConditionCounterTemplate, IEnumerable<Condition>>.Create(
                        "Conditions");
            }
        }

        public class GamePlayerOwnerData
        {
            private static readonly Lazy<GamePlayerOwnerData> Lazy =
                new Lazy<GamePlayerOwnerData>(() => new GamePlayerOwnerData());

            public static GamePlayerOwnerData Instance => Lazy.Value;

            public GamePlayerOwner GamePlayerOwner { get; internal set; }

            private GamePlayerOwnerData()
            {
            }
        }
    }
}