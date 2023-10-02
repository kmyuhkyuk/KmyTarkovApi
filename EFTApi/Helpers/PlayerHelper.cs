using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using EFT;
using EFT.InventoryLogic;
using EFTReflection;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Global

namespace EFTApi.Helpers
{
    public class PlayerHelper
    {
        public static readonly PlayerHelper Instance = new PlayerHelper();

        public Player Player { get; private set; }

        public readonly FirearmControllerData FirearmControllerHelper = FirearmControllerData.Instance;

        public readonly WeaponData WeaponHelper = WeaponData.Instance;

        public readonly ArmorComponentData ArmorComponentHelper = ArmorComponentData.Instance;

        public readonly RoleData RoleHelper = RoleData.Instance;

        public readonly InventoryData InventoryHelper = InventoryData.Instance;

        public readonly DamageInfoData DamageInfoHelper = DamageInfoData.Instance;

        public readonly SpeakerData SpeakerHelper = SpeakerData.Instance;

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

        public readonly RefHelper.HookRef OnBeenKilledByAggressor;

        public readonly RefHelper.HookRef OnPhraseTold;

        /// <summary>
        ///     InfoClass.Settings
        /// </summary>
        public readonly RefHelper.FieldRef<InfoClass, object> RefSettings;

        /// <summary>
        ///     InfoClass.Settings.Role
        /// </summary>
        public readonly RefHelper.FieldRef<object, WildSpawnType> RefRole;

        /// <summary>
        ///     InfoClass.Settings.Experience
        /// </summary>
        public readonly RefHelper.FieldRef<object, int> RefExperience;

        public object Settings => RefSettings.GetValue(Player.Profile.Info);

        public WildSpawnType Role => RefRole.GetValue(Settings);

        public int Experience => RefExperience.GetValue(Settings);

        private PlayerHelper()
        {
            var playerType = typeof(Player);

            Init = RefHelper.HookRef.Create(playerType, "Init");
            Dispose = RefHelper.HookRef.Create(playerType, "Dispose");
            OnDead = RefHelper.HookRef.Create(playerType, "OnDead");
            ApplyDamageInfo = RefHelper.HookRef.Create(playerType, "ApplyDamageInfo");
            OnBeenKilledByAggressor = RefHelper.HookRef.Create(playerType, "OnBeenKilledByAggressor");
            OnPhraseTold = RefHelper.HookRef.Create(playerType, "OnPhraseTold");

            Init.Add(this, nameof(OnInit));

            RefSettings = RefHelper.FieldRef<InfoClass, object>.Create("Settings");
            RefRole = RefHelper.FieldRef<object, WildSpawnType>.Create(RefSettings.FieldType, "Role");
            RefExperience = RefHelper.FieldRef<object, int>.Create(RefSettings.FieldType, "Experience");
        }

        private static async void OnInit(Player __instance, Task __result)
        {
            await __result;

            if (EFTVersion.AkiVersion > Version.Parse("2.3.1") ? __instance.IsYourPlayer : __instance.Id == 1)
            {
                Instance.Player = __instance;
            }
        }

        public class FirearmControllerData
        {
            public static readonly FirearmControllerData Instance = new FirearmControllerData();

            public Player.FirearmController FirearmController => PlayerHelper.Instance.Player != null
                ? PlayerHelper.Instance.Player.HandsController as Player.FirearmController
                : null;

            public readonly RefHelper.HookRef InitiateShot;

            private FirearmControllerData()
            {
                InitiateShot = RefHelper.HookRef.Create(typeof(Player.FirearmController), "InitiateShot");
            }
        }

        public class ArmorComponentData
        {
            public static readonly ArmorComponentData Instance = new ArmorComponentData();

            public readonly RefHelper.HookRef ApplyDamage;

            private ArmorComponentData()
            {
                ApplyDamage = RefHelper.HookRef.Create(typeof(ArmorComponent), "ApplyDamage");
            }
        }

        public class RoleData
        {
            public static readonly RoleData Instance = new RoleData();

            private readonly Func<WildSpawnType, bool> _refIsBoss;

            private readonly Func<WildSpawnType, bool> _refIsFollower;

            private readonly Func<WildSpawnType, bool> _refIsBossOrFollower;

            private readonly Func<WildSpawnType, string> _refGetScavRoleKey;

            private RoleData()
            {
                var flags = BindingFlags.Static | RefTool.Public;

                var roleType = RefTool.GetEftType(x =>
                    x.GetMethod("IsBoss", flags) != null && x.GetMethod("Init", flags) != null);

                _refIsBoss = AccessTools.MethodDelegate<Func<WildSpawnType, bool>>(roleType.GetMethod("IsBoss", flags));

                _refIsFollower =
                    AccessTools.MethodDelegate<Func<WildSpawnType, bool>>(roleType.GetMethod("IsFollower", flags));

                _refIsBossOrFollower =
                    AccessTools.MethodDelegate<Func<WildSpawnType, bool>>(roleType.GetMethod("IsBossOrFollower",
                        flags));

                _refGetScavRoleKey =
                    AccessTools.MethodDelegate<Func<WildSpawnType, string>>(roleType.GetMethod("GetScavRoleKey",
                        flags));
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
            public static readonly InventoryData Instance = new InventoryData();

            public object Equipment => RefEquipment.GetValue(PlayerHelper.Instance.Player.Profile.Inventory);

            public object QuestRaidItems =>
                RefQuestRaidItems.GetValue(PlayerHelper.Instance.Player.Profile.Inventory);

            public Slot[] EquipmentSlots => RefSlots.GetValue(Equipment);

            public List<object> EquipmentGrids
            {
                get
                {
                    var equipmentSlots = EquipmentSlots;

                    if (equipmentSlots == null)
                        return null;

                    var list = new List<object>();

                    foreach (var slot in new[]
                                 { equipmentSlots[6], equipmentSlots[7], equipmentSlots[8], equipmentSlots[10] })
                    {
                        var gear = slot.ContainedItem;

                        if (gear == null)
                            continue;

                        list.AddRange(RefGrids.GetValue(gear));
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
                        list.AddRange(RefItems.GetValue(grid));
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
                        list.AddRange(RefItems.GetValue(grid));
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
            ///     InventoryClass.Equipment
            /// </summary>
            public readonly RefHelper.FieldRef<InventoryClass, object> RefEquipment;

            /// <summary>
            ///     InventoryClass.QuestRaidItems
            /// </summary>
            public readonly RefHelper.FieldRef<InventoryClass, object> RefQuestRaidItems;

            /// <summary>
            ///     InventoryClass.Equipment.Slots
            /// </summary>
            public readonly RefHelper.FieldRef<object, Slot[]> RefSlots;

            // ReSharper disable once InvalidXmlDocComment
            /// <summary>
            ///     InventoryClass.Equipment.Slots.Grids
            /// </summary>
            public readonly RefHelper.FieldRef<object, object[]> RefGrids;

            /// <summary>
            ///     InventoryClass.Equipment.Slots.Grids.Items
            /// </summary>
            public readonly RefHelper.PropertyRef<object, IEnumerable<Item>> RefItems;

            private InventoryData()
            {
                RefEquipment = RefHelper.FieldRef<InventoryClass, object>.Create("Equipment");
                RefQuestRaidItems = RefHelper.FieldRef<InventoryClass, object>.Create("QuestRaidItems");
                RefSlots = RefHelper.FieldRef<object, Slot[]>.Create(RefEquipment.FieldType, "Slots");
                RefGrids = RefHelper.FieldRef<object, object[]>.Create(
                    RefTool.GetEftType(x =>
                        x.GetMethod("TryGetLastForbiddenItem", BindingFlags.DeclaredOnly | RefTool.Public) != null),
                    "Grids");

                RefItems = RefHelper.PropertyRef<object, IEnumerable<Item>>.Create(RefGrids.FieldType.GetElementType(),
                    "Items");
            }
        }

        public class WeaponData
        {
            public static readonly WeaponData Instance = new WeaponData();

            public Weapon Weapon => FirearmControllerData.Instance.FirearmController != null
                ? FirearmControllerData.Instance.FirearmController.Item
                : null;

            public object CurrentMagazine => GetCurrentMagazine(Weapon);

            public Item UnderbarrelWeapon =>
                RefUnderbarrelWeapon?.GetValue(FirearmControllerData.Instance.FirearmController);

            public Animator WeaponAnimator =>
                RefAnimator.GetValue(RefWeaponIAnimator.GetValue(PlayerHelper.Instance.Player));

            public Animator LauncherIAnimator =>
                RefAnimator.GetValue(RefUnderbarrelWeaponIAnimator?.GetValue(PlayerHelper.Instance.Player));

            public Slot[] UnderbarrelChambers => RefUnderbarrelChambers?.GetValue(UnderbarrelWeapon);

            public WeaponTemplate UnderbarrelWeaponTemplate =>
                RefUnderbarrelWeaponTemplate?.GetValue(UnderbarrelWeapon);

            public int UnderbarrelChamberAmmoCount => RefUnderbarrelChamberAmmoCount?.GetValue(UnderbarrelWeapon) ?? 0;

            private readonly Func<Weapon, object> _refGetCurrentMagazine;

            /// <summary>
            ///     Player.FirearmController.UnderbarrelWeapon
            /// </summary>
            [CanBeNull] public readonly RefHelper.FieldRef<Player.FirearmController, Item> RefUnderbarrelWeapon;

            /// <summary>
            ///     Player.ArmsAnimatorCommon
            /// </summary>
            public readonly RefHelper.PropertyRef<Player, object> RefWeaponIAnimator;

            /// <summary>
            ///     Player.UnderbarrelWeaponArmsAnimator
            /// </summary>
            [CanBeNull] public readonly RefHelper.PropertyRef<Player, object> RefUnderbarrelWeaponIAnimator;

            /// <summary>
            ///     IAnimator.Animator
            /// </summary>
            public readonly RefHelper.PropertyRef<object, Animator> RefAnimator;

            /// <summary>
            ///     Player.FirearmController.UnderbarrelWeapon.Chambers
            /// </summary>
            [CanBeNull] public readonly RefHelper.FieldRef<object, Slot[]> RefUnderbarrelChambers;

            /// <summary>
            ///     Player.FirearmController.UnderbarrelWeapon.WeaponTemplate
            /// </summary>
            [CanBeNull] public readonly RefHelper.PropertyRef<object, WeaponTemplate> RefUnderbarrelWeaponTemplate;

            /// <summary>
            ///     Player.FirearmController.UnderbarrelWeapon.ChamberAmmoCount
            /// </summary>
            [CanBeNull] public readonly RefHelper.PropertyRef<object, int> RefUnderbarrelChamberAmmoCount;

            private WeaponData()
            {
                _refGetCurrentMagazine =
                    AccessTools.MethodDelegate<Func<Weapon, object>>(
                        typeof(Weapon).GetMethod("GetCurrentMagazine", RefTool.Public));

                RefWeaponIAnimator = RefHelper.PropertyRef<Player, object>.Create("ArmsAnimatorCommon");
                RefAnimator = RefHelper.PropertyRef<object, Animator>.Create(
                    RefTool.GetEftType(x =>
                        x.GetMethod("CreateAnimatorStateInfoWrapper", RefTool.Public | BindingFlags.Static) != null),
                    "Animator");

                if (EFTVersion.AkiVersion > Version.Parse("3.4.1"))
                {
                    RefUnderbarrelWeapon =
                        RefHelper.FieldRef<Player.FirearmController, Item>.Create("UnderbarrelWeapon");
                    RefUnderbarrelWeaponIAnimator =
                        RefHelper.PropertyRef<Player, object>.Create("UnderbarrelWeaponArmsAnimator");

                    var launcherType =
                        RefTool.GetEftType(x => x.GetMethod("GetCenterOfImpact", RefTool.Public) != null);

                    RefUnderbarrelChambers = RefHelper.FieldRef<object, Slot[]>.Create(launcherType, "Chambers");
                    RefUnderbarrelWeaponTemplate =
                        RefHelper.PropertyRef<object, WeaponTemplate>.Create(launcherType, "WeaponTemplate");
                    RefUnderbarrelChamberAmmoCount =
                        RefHelper.PropertyRef<object, int>.Create(launcherType, "ChamberAmmoCount");
                }
            }

            public object GetCurrentMagazine(Weapon weapon)
            {
                return _refGetCurrentMagazine(weapon);
            }
        }

        public class DamageInfoData
        {
            public static readonly DamageInfoData Instance = new DamageInfoData();

            /// <summary>
            ///     DamageInfo.Player
            /// </summary>
            public readonly RefHelper.FieldRef<DamageInfo, object> RefPlayer;

            /// <summary>
            ///     DamageInfo.Player.iPlayer
            /// </summary>
            [CanBeNull] public readonly RefHelper.PropertyRef<object, IAIDetails> RefIPlayer;

            private DamageInfoData()
            {
                RefPlayer = RefHelper.FieldRef<DamageInfo, object>.Create(typeof(DamageInfo).GetField("Player"));

                if (EFTVersion.AkiVersion > Version.Parse("3.5.8"))
                {
                    RefIPlayer = RefHelper.PropertyRef<object, IAIDetails>.Create(RefPlayer.FieldType, "iPlayer");
                }
            }

            public Player GetPlayer(DamageInfo damageInfo)
            {
                if (EFTVersion.AkiVersion > Version.Parse("3.5.8"))
                {
                    return (Player)RefIPlayer?.GetValue(RefPlayer.GetValue(damageInfo));
                }
                else
                {
                    return (Player)RefPlayer.GetValue(damageInfo);
                }
            }
        }

        public class SpeakerData
        {
            public static readonly SpeakerData Instance = new SpeakerData();

            /// <summary>
            ///     Player.Speaker
            /// </summary>
            public readonly RefHelper.FieldRef<Player, object> RefSpeaker;

            /// <summary>
            ///     Player.Speaker.Speaking
            /// </summary>
            public readonly RefHelper.FieldRef<object, bool> RefSpeaking;

            /// <summary>
            /// Player.Speaker.Clip
            /// </summary>
            public readonly RefHelper.FieldRef<object, TaggedClip> RefClip;

            /// <summary>
            /// Player.Speaker.PlayerVoice
            /// </summary>
            public readonly RefHelper.PropertyRef<object, string> RefPlayerVoice;

            public object Speaker => RefSpeaker.GetValue(PlayerHelper.Instance.Player);

            public bool Speaking => RefSpeaking.GetValue(Speaker);

            public TaggedClip Clip => RefClip.GetValue(Speaker);

            public string PlayerVoice => RefPlayerVoice.GetValue(Speaker);

            private SpeakerData()
            {
                RefSpeaker = RefHelper.FieldRef<Player, object>.Create(typeof(Player), "Speaker");
                RefSpeaking = RefHelper.FieldRef<object, bool>.Create(RefSpeaker.FieldType, "Speaking");
                RefClip = RefHelper.FieldRef<object, TaggedClip>.Create(RefSpeaker.FieldType, "Clip");

                RefPlayerVoice = RefHelper.PropertyRef<object, string>.Create(RefSpeaker.FieldType, "PlayerVoice");
            }
        }
    }
}