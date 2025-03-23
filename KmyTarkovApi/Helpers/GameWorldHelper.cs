using System;
using System.Collections.Generic;
using System.Linq;
using EFT;
using EFT.Interactive;
using KmyTarkovReflection;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberHidesStaticFromOuterClass
// ReSharper disable NotAccessedField.Global
// ReSharper disable UnusedMember.Global

namespace KmyTarkovApi.Helpers
{
    public class GameWorldHelper
    {
        private static readonly Lazy<GameWorldHelper> Lazy = new Lazy<GameWorldHelper>(() => new GameWorldHelper());

        public static GameWorldHelper Instance => Lazy.Value;

        public GameWorld GameWorld { get; private set; }

        public ZoneData ZoneHelper => ZoneData.Instance;

        public ExfiltrationPointData ExfiltrationPointHelper => ExfiltrationPointData.Instance;

        /// <summary>
        ///     Init Action
        /// </summary>
        public readonly RefHelper.HookRef Awake;

        /// <summary>
        ///     Dispose Action
        /// </summary>
        public readonly RefHelper.HookRef Dispose;

        public readonly RefHelper.HookRef OnGameStarted;

        public List<Player> AllBot
        {
            get
            {
                if (GameWorld == null)
                    return null;

                var list = new List<Player>();

                // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                foreach (var player in GameWorld.AllAlivePlayersList)
                {
                    if (player.IsAI)
                    {
                        list.Add(player);
                    }
                }

                return list;
            }
        }

        public List<Player> AllOtherPlayer
        {
            get
            {
                if (GameWorld == null)
                    return null;

                var list = new List<Player>();

                // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                foreach (var player in GameWorld.AllAlivePlayersList)
                {
                    if (player != PlayerHelper.Instance.Player)
                    {
                        list.Add(player);
                    }
                }

                return list;
            }
        }

        private GameWorldHelper()
        {
            var gameWorldType = typeof(GameWorld);

            Awake = RefHelper.HookRef.Create(gameWorldType, "Awake");
            Dispose = RefHelper.HookRef.Create(gameWorldType, "Dispose");
            OnGameStarted = RefHelper.HookRef.Create(gameWorldType, "OnGameStarted");
        }

        [EFTHelperHook]
        private void Hook()
        {
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

                    return false;
                }

                return triggers.Any();
            }
        }

        public class ExfiltrationPointData
        {
            private static readonly Lazy<ExfiltrationPointData> Lazy =
                new Lazy<ExfiltrationPointData>(() => new ExfiltrationPointData());

            public static ExfiltrationPointData Instance => Lazy.Value;

            public readonly RefHelper.FieldRef<ExfiltrationPoint, List<Switch>> RefSwitchs;

            private ExfiltrationPointData()
            {
                RefSwitchs =
                    RefHelper.FieldRef<ExfiltrationPoint, List<Switch>>.Create("_switches");
            }
        }
    }
}