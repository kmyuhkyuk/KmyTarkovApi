using System;
using System.Collections.Generic;
using System.Reflection;
using EFT;
using EFT.InventoryLogic;
using EFTReflection;
using HarmonyLib;
using UnityEngine;

namespace EFTApi.Helpers
{
    public class PlayerHelper
    {
        public Player Player { get; private set; }

        /// <summary>
        /// FirearmController Helper
        /// </summary>
        public readonly FirearmControllerData FirearmControllerHelper = new FirearmControllerData();

        /// <summary>
        /// Weapon Helper
        /// </summary>
        public readonly WeaponData WeaponHelper = new WeaponData();

        /// <summary>
        /// ArmorComponent Helper
        /// </summary>
        public readonly ArmorComponentData ArmorComponentHelper = new ArmorComponentData();

        /// <summary>
        /// Role Helper
        /// </summary>
        public readonly RoleData RoleHelper = new RoleData();

        /// <summary>
        /// Inventory Helper
        /// </summary>
        public readonly InventoryData InventoryHelper = new InventoryData();

        /// <summary>
        /// Init Action
        /// </summary>
        public event Action<Player, Quaternion, string, EPointOfView, Profile, object, IHealthController, object, object, object, Player.EVoipState, bool, bool> Init;

        public event Action<Player> Dispose;

        public event Action<Player, EDamageType> OnDead;

        public event Action<Player, DamageInfo, EBodyPart, float, EHeadSegment?> ApplyDamageInfo;

        public event Action<Player, Player, DamageInfo, EBodyPart, EDamageType> OnBeenKilledByAggressor;

        /// <summary>
        /// InfoClass.Settings
        /// </summary>
        public readonly RefHelper.FieldRef<InfoClass, object> RefSettings;

        /// <summary>
        /// InfoClass.Settings.Role
        /// </summary>
        public readonly RefHelper.FieldRef<object, WildSpawnType> RefRole;

        /// <summary>
        /// InfoClass.Settings.Experience
        /// </summary>
        public readonly RefHelper.FieldRef<object, int> RefExperience;

        public object Settings => RefSettings.GetValue(Player.Profile.Info);

        public WildSpawnType Role => RefRole.GetValue(Settings);

        public int Experience => RefExperience.GetValue(Settings);

        public PlayerHelper()
        {
            RefSettings = RefHelper.FieldRef<InfoClass, object>.Create(typeof(InfoClass), "Settings");
            RefRole = RefHelper.FieldRef<object, WildSpawnType>.Create(RefSettings.FieldType, "Role");
            RefExperience = RefHelper.FieldRef<object, int>.Create(RefSettings.FieldType, "Experience");
        }

        internal void Trigger_Init(Player player, Quaternion rotation, string layerName, EPointOfView pointOfView, Profile profile, object inventoryController, IHealthController healthController, object statisticsManager, object questController, object filter, Player.EVoipState voipState, bool aiControlled, bool async)
        {
            if (EFTVersion.Is231Up ? player.IsYourPlayer : player.Id == 1)
            {
                Player = player;
            }

            Init?.Invoke(player, rotation, layerName, pointOfView, profile, inventoryController, healthController, statisticsManager, questController, filter, voipState, aiControlled, async);
        }

        internal void Trigger_Dispose(Player player)
        {
            Dispose?.Invoke(player);
        }

        internal void Trigger_OnDead(Player player, EDamageType damageType)
        {
            OnDead?.Invoke(player, damageType);
        }

        internal void Trigger_ApplyDamageInfo(Player player, DamageInfo damageInfo, EBodyPart bodyPartType, float absorbed, EHeadSegment? headSegment)
        {
            ApplyDamageInfo?.Invoke(player, damageInfo, bodyPartType, absorbed, headSegment);
        }

        internal void Trigger_OnBeenKilledByAggressor(Player player, Player aggressor, DamageInfo damageInfo, EBodyPart bodyPart, EDamageType lethalDamageType)
        {
            OnBeenKilledByAggressor?.Invoke(player, aggressor, damageInfo, bodyPart, lethalDamageType);
        }


        public class FirearmControllerData
        {
            public Player.FirearmController FirearmController => EFTHelpers._PlayerHelper.Player != null ? EFTHelpers._PlayerHelper.Player.HandsController as Player.FirearmController : null;

            public event Action<Player.FirearmController, Player, BulletClass, Vector3, Vector3, Vector3, int, float> InitiateShot;

            internal void Trigger_InitiateShot(Player.FirearmController firearmController, Player player, BulletClass ammo, Vector3 shotPosition, Vector3 shotDirection, Vector3 fireportPosition, int chamberIndex, float overheat)
            {
                InitiateShot?.Invoke(firearmController, player, ammo, shotPosition, shotDirection, fireportPosition, chamberIndex, overheat);
            }
        }

        public class ArmorComponentData
        {
            public event Action<ArmorComponent, float> ApplyDurabilityDamage;

            public event Action<ArmorComponent, DamageInfo, EBodyPart, bool, object, object> ApplyDamage;

            internal void Trigger_ApplyDurabilityDamage(ArmorComponent armorComponent, float armorDamage)
            {
                ApplyDurabilityDamage?.Invoke(armorComponent, armorDamage);
            }

            internal void Trigger_ApplyDamage(ArmorComponent armorComponent, DamageInfo damageInfo, EBodyPart bodyPartType, bool damageInfoIsLocal, object lightVestsDamageReduction, object heavyVestsDamageReduction)
            {
                ApplyDamage?.Invoke(armorComponent, damageInfo, bodyPartType, damageInfoIsLocal, lightVestsDamageReduction, heavyVestsDamageReduction);
            }
        }

        public class RoleData
        {
            private readonly Func<WildSpawnType, bool> _refIsBoss;

            private readonly Func<WildSpawnType, bool> _refIsFollower;

            private readonly Func<WildSpawnType, bool> _refIsBossOrFollower;

            private readonly Func<WildSpawnType, string> _refGetScavRoleKey;

            public RoleData()
            {
                var flags = BindingFlags.Static | RefTool.Public;

                var roleType = RefTool.GetEftType(x => x.GetMethod("IsBoss", flags) != null && x.GetMethod("Init", flags) != null);

                _refIsBoss = AccessTools.MethodDelegate<Func<WildSpawnType, bool>>(roleType.GetMethod("IsBoss", flags));

                _refIsFollower = AccessTools.MethodDelegate<Func<WildSpawnType, bool>>(roleType.GetMethod("IsFollower", flags));

                _refIsBossOrFollower = AccessTools.MethodDelegate<Func<WildSpawnType, bool>>(roleType.GetMethod("IsBossOrFollower", flags));

                _refGetScavRoleKey = AccessTools.MethodDelegate<Func<WildSpawnType, string>>(roleType.GetMethod("GetScavRoleKey", flags));
            }

            public bool IsBoss(WildSpawnType role)
            {
                return _refIsBoss(role);
            }

            public bool IsFollower(WildSpawnType role)
            {
                return _refIsFollower(role);
            }

            public bool IsBossOrFollower(WildSpawnType role)
            {
                return _refIsBossOrFollower(role);
            }

            public string GetScavRoleKey(WildSpawnType role)
            {
                return _refGetScavRoleKey(role);
            }
        }

        public class InventoryData
        {
            public object Equipment => RefEquipment.GetValue(EFTHelpers._PlayerHelper.Player.Profile.Inventory);

            public object QuestRaidItems => RefQuestRaidItems.GetValue(EFTHelpers._PlayerHelper.Player.Profile.Inventory);

            public Slot[] EquipmentSlots => RefSlots.GetValue(Equipment);

            public List<object> EquipmentGrids
            {
                get
                {
                    var equipmentSlots = EquipmentSlots;

                    if (equipmentSlots == null)
                        return null;

                    var list = new List<object>();

                    foreach (var slot in new[] { equipmentSlots[6], equipmentSlots[7], equipmentSlots[8], equipmentSlots[10] })
                    {
                        var gear = slot.ContainedItem;

                        if (gear == null)
                            continue;

                        foreach (var grid in RefGrids.GetValue(gear))
                        {
                            list.Add(grid);
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
                        foreach (var item in RefItems.GetValue(grid))
                        {
                            list.Add(item);
                        }
                    }

                    return list;
                }
            }

            public HashSet<string> EquipmentHash
            {
                get
                {
                    var equipmentGrids = EquipmentGrids;

                    if (equipmentGrids == null)
                        return null;

                    var hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    foreach (var grid in equipmentGrids)
                    {
                        foreach (var item in RefItems.GetValue(grid))
                        {
                            hashSet.Add(item.TemplateId);
                        }
                    }

                    return hashSet;
                }
            }

            public List<object> QuestRaidItemsGrids
            {
                get
                {
                    var questRaidItems = QuestRaidItems;

                    if (questRaidItems == null)
                        return null;

                    var list = new List<object>();

                    foreach (var grid in RefGrids.GetValue(questRaidItems))
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
                        foreach (var item in RefItems.GetValue(grid))
                        {
                            list.Add(item);
                        }
                    }

                    return list;
                }
            }

            public HashSet<string> QuestRaidItemsHash
            {
                get
                {
                    var questRaidItemsGrids = QuestRaidItemsGrids;

                    if (questRaidItemsGrids == null)
                        return null;

                    var hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    foreach (var grid in questRaidItemsGrids)
                    {
                        foreach (var item in RefItems.GetValue(grid))
                        {
                            hashSet.Add(item.TemplateId);
                        }
                    }

                    return hashSet;
                }
            }

            /// <summary>
            /// InventoryClass.Equipment
            /// </summary>
            public readonly RefHelper.FieldRef<InventoryClass, object> RefEquipment;

            /// <summary>
            /// InventoryClass.QuestRaidItems
            /// </summary>
            public readonly RefHelper.FieldRef<InventoryClass, object> RefQuestRaidItems;

            /// <summary>
            /// InventoryClass.Equipment.Slots
            /// </summary>
            public readonly RefHelper.FieldRef<object, Slot[]> RefSlots;

            // ReSharper disable once InvalidXmlDocComment
            /// <summary>
            /// InventoryClass.Equipment.Slots.Grids
            /// </summary>
            public readonly RefHelper.FieldRef<object, object[]> RefGrids;

            /// <summary>
            /// InventoryClass.Equipment.Slots.Grids.Items
            /// </summary>
            public readonly RefHelper.PropertyRef<object, IEnumerable<Item>> RefItems;

            public InventoryData()
            {
                RefEquipment = RefHelper.FieldRef<InventoryClass, object>.Create("Equipment");
                RefQuestRaidItems = RefHelper.FieldRef<InventoryClass, object>.Create("QuestRaidItems");
                RefSlots = RefHelper.FieldRef<object, Slot[]>.Create(RefEquipment.FieldType, "Slots");
                RefGrids = RefHelper.FieldRef<object, object[]>.Create(RefTool.GetEftType(x => x.GetMethod("TryGetLastForbiddenItem", BindingFlags.DeclaredOnly | RefTool.Public) != null), "Grids");

                RefItems = RefHelper.PropertyRef<object, IEnumerable<Item>>.Create(RefGrids.FieldType.GetElementType(), "Items");
            }
        }

        public class WeaponData
        {
            public Weapon Weapon => EFTHelpers._PlayerHelper.FirearmControllerHelper.FirearmController != null ? EFTHelpers._PlayerHelper.FirearmControllerHelper.FirearmController.Item : null;

            public object CurrentMagazine => GetCurrentMagazine(Weapon);

            public Item UnderbarrelWeapon => RefUnderbarrelWeapon.GetValue(EFTHelpers._PlayerHelper.FirearmControllerHelper.FirearmController);

            public Animator WeaponAnimator => RefAnimator.GetValue(RefWeaponIAnimator.GetValue(EFTHelpers._PlayerHelper.Player));

            public Animator LauncherIAnimator => RefAnimator.GetValue(RefUnderbarrelWeaponIAnimator.GetValue(EFTHelpers._PlayerHelper.Player));

            public Slot[] UnderbarrelChambers => RefUnderbarrelChambers.GetValue(UnderbarrelWeapon);

            public WeaponTemplate UnderbarrelWeaponTemplate => RefUnderbarrelWeaponTemplate.GetValue(UnderbarrelWeapon);

            public int UnderbarrelChamberAmmoCount => RefUnderbarrelChamberAmmoCount.GetValue(UnderbarrelWeapon);

            private readonly Func<Weapon, object> _refGetCurrentMagazine;

            /// <summary>
            /// Player.FirearmController.UnderbarrelWeapon
            /// </summary>
            public readonly RefHelper.FieldRef<Player.FirearmController, Item> RefUnderbarrelWeapon;

            /// <summary>
            /// Player.ArmsAnimatorCommon
            /// </summary>
            public readonly RefHelper.PropertyRef<Player, object> RefWeaponIAnimator;

            /// <summary>
            /// Player.UnderbarrelWeaponArmsAnimator
            /// </summary>
            public readonly RefHelper.PropertyRef<Player, object> RefUnderbarrelWeaponIAnimator;

            /// <summary>
            /// IAnimator.Animator
            /// </summary>
            public readonly RefHelper.PropertyRef<object, Animator> RefAnimator;

            /// <summary>
            /// Player.FirearmController.UnderbarrelWeapon.Chambers
            /// </summary>
            public readonly RefHelper.FieldRef<object, Slot[]> RefUnderbarrelChambers;

            /// <summary>
            /// Player.FirearmController.UnderbarrelWeapon.WeaponTemplate
            /// </summary>
            public readonly RefHelper.PropertyRef<object, WeaponTemplate> RefUnderbarrelWeaponTemplate;

            /// <summary>
            /// Player.FirearmController.UnderbarrelWeapon.ChamberAmmoCount
            /// </summary>
            public readonly RefHelper.PropertyRef<object, int> RefUnderbarrelChamberAmmoCount;

            public WeaponData()
            {
                _refGetCurrentMagazine = AccessTools.MethodDelegate<Func<Weapon, object>>(typeof(Weapon).GetMethod("GetCurrentMagazine", RefTool.Public));

                RefWeaponIAnimator = RefHelper.PropertyRef<Player, object>.Create("ArmsAnimatorCommon");
                RefAnimator = RefHelper.PropertyRef<object, Animator>.Create(RefTool.GetEftType(x => x.GetMethod("CreateAnimatorStateInfoWrapper", RefTool.Public | BindingFlags.Static) != null), "Animator");

                if (EFTVersion.Is341Up)
                {
                    RefUnderbarrelWeapon = RefHelper.FieldRef<Player.FirearmController, Item>.Create("UnderbarrelWeapon");
                    RefUnderbarrelWeaponIAnimator = RefHelper.PropertyRef<Player, object>.Create("UnderbarrelWeaponArmsAnimator");

                    var launcherType = RefTool.GetEftType(x => x.GetMethod("GetCenterOfImpact", RefTool.Public) != null);

                    RefUnderbarrelChambers = RefHelper.FieldRef<object, Slot[]>.Create(launcherType, "Chambers");
                    RefUnderbarrelWeaponTemplate = RefHelper.PropertyRef<object, WeaponTemplate>.Create(launcherType, "WeaponTemplate");
                    RefUnderbarrelChamberAmmoCount = RefHelper.PropertyRef<object, int>.Create(launcherType, "ChamberAmmoCount");
                }
            }

            public object GetCurrentMagazine(Weapon weapon)
            {
                return _refGetCurrentMagazine(weapon);
            }
        }
    }
}
