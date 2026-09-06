using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Diz.Jobs;
using EFT;
using KmyTarkovReflection;

// ReSharper disable UnusedMember.Global

namespace KmyTarkovApi.Helpers
{
    public class PoolManagerClassHelper
    {
        private static readonly Lazy<PoolManagerClassHelper> Lazy =
            new Lazy<PoolManagerClassHelper>(() => new PoolManagerClassHelper());

        public static PoolManagerClassHelper Instance => Lazy.Value;

        public static JobPriorityData JobPriorityHelper => JobPriorityData.Instance;

        public ObjectsFactory PoolManagerClass { get; private set; }

        public readonly RefHelper.HookRef Constructor;

        private readonly
            Func<ObjectsFactory, ObjectsFactory.PoolsCategory, ObjectsFactory.AssemblyType, ICollection<ResourceKey>, YieldDelegate,
                IProgress<InitLevelProgress>,
                CancellationToken, Task> _refLoadBundlesAndCreatePools;

        private PoolManagerClassHelper()
        {
            var poolManagerClassType = typeof(ObjectsFactory);

            _refLoadBundlesAndCreatePools = RefHelper
                .ObjectMethodDelegate<Func<ObjectsFactory, ObjectsFactory.PoolsCategory,
                    ObjectsFactory.AssemblyType, ICollection<ResourceKey>, YieldDelegate, IProgress<InitLevelProgress>,
                    CancellationToken, Task>>(poolManagerClassType.GetMethod("LoadBundlesAndCreatePools",
                    RefTool.Public, null,
                    new[]
                    {
                        typeof(ObjectsFactory.PoolsCategory), typeof(ObjectsFactory.AssemblyType),
                        typeof(ICollection<ResourceKey>), typeof(YieldDelegate), typeof(IProgress<InitLevelProgress>),
                        typeof(CancellationToken)
                    }, null));

            Constructor = RefHelper.HookRef.Create(poolManagerClassType.GetConstructors()[0]);
        }

        [EFTHelperHook]
        private void Hook()
        {
            Constructor.Add(this, nameof(OnConstructor));
        }

        private static void OnConstructor(ObjectsFactory __instance)
        {
            Instance.PoolManagerClass = __instance;
        }

        public Task LoadBundlesAndCreatePools(ObjectsFactory instance, ObjectsFactory.PoolsCategory poolsCategory,
            ObjectsFactory.AssemblyType assemblyType, ICollection<ResourceKey> resources, YieldDelegate yield,
            IProgress<InitLevelProgress> progress = null,
            CancellationToken ct = default)
        {
            return _refLoadBundlesAndCreatePools(instance, poolsCategory, assemblyType, resources, yield, progress, ct);
        }

        public class JobPriorityData
        {
            private static readonly Lazy<JobPriorityData> Lazy =
                new Lazy<JobPriorityData>(() => new JobPriorityData());

            public static JobPriorityData Instance => Lazy.Value;

            public YieldDelegate General => JobYieldPriority.General;

            public YieldDelegate Low => JobYieldPriority.Low;

            public YieldDelegate Immediate => JobYieldPriority.Immediate;
        }
    }
}