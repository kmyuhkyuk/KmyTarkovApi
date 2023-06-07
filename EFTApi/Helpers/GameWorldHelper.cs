using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using EFTReflection;
using EFTReflection.Patching;

namespace EFTApi.Helpers
{
    public class GameWorldHelper
    {
        public static readonly GameWorldHelper Instance = new GameWorldHelper();

        public GameWorld GameWorld { get; private set; }

        public readonly LevelSettingsData LevelSettingsHelper = LevelSettingsData.Instance;

        public readonly LootableContainerData LootableContainerHelper = LootableContainerData.Instance;

        public readonly SearchableItemClassData SearchableItemClassHelper = SearchableItemClassData.Instance;

        public readonly ZoneData ZoneHelper = ZoneData.Instance;

        public List<Player> AllBot
        {
            get
            {
                if (GameWorld == null)
                    return null;

                var list = new List<Player>();

                foreach (var player in GameWorld.AllPlayers)
                {
                    if (player != PlayerHelper.Instance.Player)
                    {
                        list.Add(player);
                    }
                }

                return list;
            }
        }

        /// <summary>
        ///     Init Action
        /// </summary>
        public event hook_Awake Awake
        {
            add => HookPatch.Add(typeof(GameWorld).GetMethod("Awake", RefTool.NonPublic), value);
            remove => HookPatch.Remove(typeof(GameWorld).GetMethod("Awake", RefTool.NonPublic), value);
        }

        public delegate void hook_Awake(GameWorld __instance);

        public event hook_OnGameStarted OnGameStarted
        {
            add => HookPatch.Add(typeof(GameWorld).GetMethod("OnGameStarted", RefTool.Public), value);
            remove => HookPatch.Remove(typeof(GameWorld).GetMethod("OnGameStarted", RefTool.Public), value);
        }

        public delegate void hook_OnGameStarted(GameWorld __instance);

        public event hook_Dispose Dispose
        {
            add => HookPatch.Add(typeof(GameWorld).GetMethod("Dispose", RefTool.Public), value);
            remove => HookPatch.Remove(typeof(GameWorld).GetMethod("Dispose", RefTool.Public), value);
        }

        public delegate void hook_Dispose(GameWorld __instance);

        private GameWorldHelper()
        {
            Awake += OnAwake;
            Dispose += OnDispose;
        }

        private static void OnDispose(GameWorld __instance)
        {
            ZoneData.Instance.TriggerPoints.Clear();
        }

        private static void OnAwake(GameWorld __instance)
        {
            Instance.GameWorld = __instance;
        }

        public class LevelSettingsData
        {
            public static readonly LevelSettingsData Instance = new LevelSettingsData();

            public LevelSettings LevelSettings { get; private set; }

            public event hook_Awake Awake
            {
                add => HookPatch.Add(typeof(LevelSettings).GetMethod("Awake", RefTool.NonPublic), value);
                remove => HookPatch.Remove(typeof(LevelSettings).GetMethod("Awake", RefTool.NonPublic), value);
            }

            public delegate void hook_Awake(LevelSettings __instance);

            public event hook_OnDestroy OnDestroy
            {
                add => HookPatch.Add(typeof(LevelSettings).GetMethod("OnDestroy", RefTool.Public), value);
                remove => HookPatch.Remove(typeof(LevelSettings).GetMethod("OnDestroy", RefTool.Public), value);
            }

            public delegate void hook_OnDestroy(LevelSettings __instance);

            private LevelSettingsData()
            {
                Awake += OnAwake;
            }

            private static void OnAwake(LevelSettings __instance)
            {
                Instance.LevelSettings = __instance;
            }
        }

        public class ZoneData
        {
            public static readonly ZoneData Instance = new ZoneData();

            internal readonly List<TriggerWithId> TriggerPoints = new List<TriggerWithId>();

            public IEnumerable<ExperienceTrigger> ExperienceTriggers => TriggerPoints.OfType<ExperienceTrigger>();

            public IEnumerable<PlaceItemTrigger> PlaceItemTriggers => TriggerPoints.OfType<PlaceItemTrigger>();

            public IEnumerable<QuestTrigger> QuestTriggers => TriggerPoints.OfType<QuestTrigger>();

            private ZoneData()
            {
            }

            public bool TryGetValues<T>(string id, out IEnumerable<T> triggers) where T : TriggerWithId
            {
                if (typeof(T) == typeof(ExperienceTrigger))
                {
                    triggers = ExperienceTriggers.Where(x => x.Id == id) as IEnumerable<T>;
                }
                else if (typeof(T) == typeof(PlaceItemTrigger))
                {
                    triggers = PlaceItemTriggers.Where(x => x.Id == id) as IEnumerable<T>;
                }
                else if (typeof(T) == typeof(QuestTrigger))
                {
                    triggers = QuestTriggers.Where(x => x.Id == id) as IEnumerable<T>;
                }
                else
                {
                    triggers = null;
                }

                return triggers != null && triggers.Any();
            }
        }

        public class LootableContainerData
        {
            public static readonly LootableContainerData Instance = new LootableContainerData();

            /// <summary>
            ///     LootableContainer.ItemOwner
            /// </summary>
            public readonly RefHelper.FieldRef<LootableContainer, object> RefItemOwner;

            /// <summary>
            ///     LootableContainer.ItemOwner.RootItem
            /// </summary>
            public readonly RefHelper.PropertyRef<object, Item> RefRootItem;

            private LootableContainerData()
            {
                RefItemOwner = RefHelper.FieldRef<LootableContainer, object>.Create("ItemOwner");
                RefRootItem = RefHelper.PropertyRef<object, Item>.Create(RefItemOwner.FieldType, "RootItem");
            }
        }

        public class SearchableItemClassData
        {
            public static readonly SearchableItemClassData Instance = new SearchableItemClassData();

            /// <summary>
            ///     SearchableItemClass.ItemOwner
            /// </summary>
            public readonly RefHelper.FieldRef<Item, List<string>> RefAllSearchersIds;

            private SearchableItemClassData()
            {
                if (EFTVersion.Is350Up)
                {
                    RefAllSearchersIds = RefHelper.FieldRef<Item, List<string>>.Create(
                        RefTool.GetEftType(x =>
                            x.GetMethod("AddNewSearcher", BindingFlags.DeclaredOnly | RefTool.Public) != null),
                        "_allSearchersIds");
                }
            }
        }
    }
}