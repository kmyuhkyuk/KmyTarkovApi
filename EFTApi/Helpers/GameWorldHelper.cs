using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using EFTReflection;
using JetBrains.Annotations;

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
        public readonly RefHelper.HookRef Awake;

        public readonly RefHelper.HookRef OnGameStarted;

        public readonly RefHelper.HookRef Dispose;

        private GameWorldHelper()
        {
            Awake = new RefHelper.HookRef(typeof(GameWorld), "Awake");
            OnGameStarted = new RefHelper.HookRef(typeof(GameWorld), "OnGameStarted");
            Dispose = new RefHelper.HookRef(typeof(GameWorld), "Dispose");

            Awake.Add(this, nameof(OnAwake));
            Dispose.Add(this, nameof(OnDispose));
        }

        private static void OnDispose()
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

            public readonly RefHelper.HookRef Awake;

            public readonly RefHelper.HookRef OnDestroy;

            private LevelSettingsData()
            {
                Awake = new RefHelper.HookRef(typeof(LevelSettings), "Awake");
                OnDestroy = new RefHelper.HookRef(typeof(LevelSettings), "OnDestroy");

                Awake.Add(this, nameof(OnAwake));
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
                    triggers = (IEnumerable<T>)ExperienceTriggers.Where(x => x.Id == id);
                }
                else if (typeof(T) == typeof(PlaceItemTrigger))
                {
                    triggers = (IEnumerable<T>)PlaceItemTriggers.Where(x => x.Id == id);
                }
                else if (typeof(T) == typeof(QuestTrigger))
                {
                    triggers = (IEnumerable<T>)QuestTriggers.Where(x => x.Id == id);
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
            [CanBeNull] public readonly RefHelper.FieldRef<Item, List<string>> RefAllSearchersIds;

            private SearchableItemClassData()
            {
                if (EFTVersion.AkiVersion > new Version("3.5.0"))
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