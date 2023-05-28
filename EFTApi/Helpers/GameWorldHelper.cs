using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using EFTReflection;

namespace EFTApi.Helpers
{
    public class GameWorldHelper
    {
        public GameWorld GameWorld { get; private set; }

        public readonly LevelSettingsData LevelSettingsHelper = new LevelSettingsData();

        public readonly LootableContainerData LootableContainerHelper = new LootableContainerData();

        public readonly SearchableItemClassData SearchableItemClassHelper = new SearchableItemClassData();

        /// <summary>
        /// Zone Helper
        /// </summary>
        public readonly ZoneData ZoneHelper = new ZoneData();

        public List<Player> AllBot
        {
            get
            {
                if (GameWorld == null)
                    return null;

                var list = new List<Player>();

                foreach (var player in GameWorld.AllPlayers)
                {
                    if (player != EFTHelpers._PlayerHelper.Player)
                    {
                        list.Add(player);
                    }
                }

                return list;
            }
        }

        /// <summary>
        /// Init Action
        /// </summary>
        public event Action<GameWorld> Awake;

        public event Action<GameWorld> OnGameStarted;

        public event Action<GameWorld> Dispose;

        internal void Trigger_Awake(GameWorld gameWorld)
        {
            GameWorld = gameWorld;

            Awake?.Invoke(gameWorld);
        }

        internal void Trigger_OnGameStarted(GameWorld gameWorld)
        {
            OnGameStarted?.Invoke(gameWorld);
        }

        internal void Trigger_Dispose(GameWorld gameWorld)
        {
            ZoneHelper.TriggerPoints.Clear();

            Dispose?.Invoke(gameWorld);
        }

        public class LevelSettingsData
        {
            public LevelSettings LevelSettings { get; private set; }

            public event Action<LevelSettings> Awake;

            public event Action<LevelSettings> OnDestroy;

            internal void Trigger_Awake(LevelSettings levelSettings)
            {
                LevelSettings = levelSettings;

                Awake?.Invoke(levelSettings);
            }

            internal void Trigger_OnDestroy(LevelSettings levelSettings)
            {
                OnDestroy?.Invoke(levelSettings);
            }
        }

        public class ZoneData
        {
            internal readonly List<TriggerWithId> TriggerPoints = new List<TriggerWithId>();

            public IEnumerable<ExperienceTrigger> ExperienceTriggers => TriggerPoints.OfType<ExperienceTrigger>();

            public IEnumerable<PlaceItemTrigger> PlaceItemTriggers => TriggerPoints.OfType<PlaceItemTrigger>();

            public IEnumerable<QuestTrigger> QuestTriggers => TriggerPoints.OfType<QuestTrigger>();

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
            /// <summary>
            /// LootableContainer.ItemOwner
            /// </summary>
            public readonly RefHelper.FieldRef<LootableContainer, object> RefItemOwner;

            /// <summary>
            /// LootableContainer.ItemOwner.RootItem
            /// </summary>
            public readonly RefHelper.PropertyRef<object, Item> RefRootItem;

            public LootableContainerData()
            {
                RefItemOwner = RefHelper.FieldRef<LootableContainer, object>.Create("ItemOwner");
                RefRootItem = RefHelper.PropertyRef<object, Item>.Create(RefItemOwner.FieldType, "RootItem");
            }
        }

        public class SearchableItemClassData
        {
            /// <summary>
            /// SearchableItemClass.ItemOwner
            /// </summary>
            public readonly RefHelper.FieldRef<Item, List<string>> RefAllSearchersIds;

            public SearchableItemClassData()
            {
                if (EFTVersion.Is350Up)
                {
                    RefAllSearchersIds = RefHelper.FieldRef<Item, List<string>>.Create(RefTool.GetEftType(x => x.GetMethod("AddNewSearcher", BindingFlags.DeclaredOnly | RefTool.Public) != null), "_allSearchersIds");
                }
            }
        }
    }
}
