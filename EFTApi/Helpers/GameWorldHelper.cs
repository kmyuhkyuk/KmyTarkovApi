using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using EFTReflection;
using JetBrains.Annotations;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable NotAccessedField.Global
// ReSharper disable UnusedMember.Global

namespace EFTApi.Helpers
{
    public class GameWorldHelper
    {
        private static readonly Lazy<GameWorldHelper> Lazy = new Lazy<GameWorldHelper>(() => new GameWorldHelper());

        public static GameWorldHelper Instance => Lazy.Value;

        public GameWorld GameWorld { get; private set; }

        public LootableContainerData LootableContainerHelper => LootableContainerData.Instance;

        public SearchableItemClassData SearchableItemClassHelper => SearchableItemClassData.Instance;

        public ZoneData ZoneHelper => ZoneData.Instance;

        public List<Player> AllBot
        {
            get
            {
                if (GameWorld == null)
                    return null;

                var list = new List<Player>();

                // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
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

        /// <summary>
        ///     Dispose Action
        /// </summary>
        public readonly RefHelper.HookRef Dispose;

        public readonly RefHelper.HookRef OnGameStarted;

        private GameWorldHelper()
        {
            var gameWorldType = typeof(GameWorld);

            Awake = RefHelper.HookRef.Create(gameWorldType, "Awake");
            Dispose = RefHelper.HookRef.Create(gameWorldType, "Dispose");
            OnGameStarted = RefHelper.HookRef.Create(gameWorldType, "OnGameStarted");

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

        public class ZoneData
        {
            private static readonly Lazy<ZoneData> Lazy = new Lazy<ZoneData>(() => new ZoneData());

            public static ZoneData Instance => Lazy.Value;

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
            private static readonly Lazy<LootableContainerData> Lazy =
                new Lazy<LootableContainerData>(() => new LootableContainerData());

            public static LootableContainerData Instance => Lazy.Value;

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
            private static readonly Lazy<SearchableItemClassData> Lazy =
                new Lazy<SearchableItemClassData>(() => new SearchableItemClassData());

            public static SearchableItemClassData Instance => Lazy.Value;

            /// <summary>
            ///     SearchableItemClass.ItemOwner
            /// </summary>
            [CanBeNull] public readonly RefHelper.FieldRef<Item, List<string>> RefAllSearchersIds;

            private SearchableItemClassData()
            {
                if (EFTVersion.AkiVersion > EFTVersion.Parse("3.5.0"))
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