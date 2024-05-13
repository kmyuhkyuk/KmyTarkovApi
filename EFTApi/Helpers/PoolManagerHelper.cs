using System;
using System.Threading;
using System.Threading.Tasks;
using EFT;
using EFTReflection;

// ReSharper disable UnusedMember.Global

namespace EFTApi.Helpers
{
    public class PoolManagerHelper
    {
        private static readonly Lazy<PoolManagerHelper> Lazy =
            new Lazy<PoolManagerHelper>(() => new PoolManagerHelper());

        public static PoolManagerHelper Instance => Lazy.Value;

        public static JobPriorityData JobPriorityHelper => JobPriorityData.Instance;

        public PoolManager PoolManager { get; private set; }

        public readonly RefHelper.HookRef PoolManagerConstructor;

        private readonly
            Func<PoolManager, PoolManager.PoolsCategory, PoolManager.AssemblyType, ResourceKey[], object, object,
                CancellationToken, Task> _refLoadBundlesAndCreatePools;

        private PoolManagerHelper()
        {
            var poolManagerType = typeof(PoolManager);

            _refLoadBundlesAndCreatePools = RefHelper
                .ObjectMethodDelegate<Func<PoolManager, PoolManager.PoolsCategory, PoolManager.AssemblyType, ResourceKey
                    [], object, object,
                    CancellationToken, Task>>(poolManagerType.GetMethod("LoadBundlesAndCreatePools", RefTool.Public));

            PoolManagerConstructor = RefHelper.HookRef.Create(poolManagerType.GetConstructors()[0]);

            PoolManagerConstructor.Add(this, nameof(OnPoolManagerConstructor));
        }

        private static void OnPoolManagerConstructor(PoolManager __instance)
        {
            Instance.PoolManager = __instance;
        }

        public Task LoadBundlesAndCreatePools(PoolManager instance, PoolManager.PoolsCategory poolsCategory,
            PoolManager.AssemblyType assemblyType, ResourceKey[] resources, object yield, object progress = null,
            CancellationToken ct = default)
        {
            return _refLoadBundlesAndCreatePools(instance, poolsCategory, assemblyType, resources, yield, progress, ct);
        }

        public class JobPriorityData
        {
            private static readonly Lazy<JobPriorityData> Lazy =
                new Lazy<JobPriorityData>(() => new JobPriorityData());

            public static JobPriorityData Instance => Lazy.Value;

            public object General => RefGeneral.GetValue(null);

            public object Low => RefLow.GetValue(null);

            public object Immediate => RefImmediate.GetValue(null);

            public readonly RefHelper.PropertyRef<object, object> RefGeneral;

            public readonly RefHelper.PropertyRef<object, object> RefLow;

            public readonly RefHelper.PropertyRef<object, object> RefImmediate;

            private JobPriorityData()
            {
                var jobPriorityType = typeof(JobPriority);

                RefGeneral = RefHelper.PropertyRef<object, object>.Create(jobPriorityType, "General");
                RefLow = RefHelper.PropertyRef<object, object>.Create(jobPriorityType, "Low");
                RefImmediate = RefHelper.PropertyRef<object, object>.Create(jobPriorityType, "Immediate");
            }
        }
    }
}