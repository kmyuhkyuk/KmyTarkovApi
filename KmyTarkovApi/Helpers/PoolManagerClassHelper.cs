using System;
using System.Threading;
using System.Threading.Tasks;
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

        public PoolManagerClass PoolManagerClass { get; private set; }

        public readonly RefHelper.HookRef Constructor;

        private readonly
            Func<PoolManagerClass, PoolManagerClass.PoolsCategory, PoolManagerClass.AssemblyType, ResourceKey[], object,
                IProgress<LoadingProgressStruct>,
                CancellationToken, Task> _refLoadBundlesAndCreatePools;

        private PoolManagerClassHelper()
        {
            var poolManagerClassType = typeof(PoolManagerClass);

            _refLoadBundlesAndCreatePools = RefHelper
                .ObjectMethodDelegate<Func<PoolManagerClass, PoolManagerClass.PoolsCategory,
                    PoolManagerClass.AssemblyType, ResourceKey
                    [], object, IProgress<LoadingProgressStruct>,
                    CancellationToken, Task>>(poolManagerClassType.GetMethod("LoadBundlesAndCreatePools",
                    RefTool.Public));

            Constructor = RefHelper.HookRef.Create(poolManagerClassType.GetConstructors()[0]);
        }

        [EFTHelperHook]
        private void Hook()
        {
            Constructor.Add(this, nameof(OnConstructor));
        }

        private static void OnConstructor(PoolManagerClass __instance)
        {
            Instance.PoolManagerClass = __instance;
        }

        public Task LoadBundlesAndCreatePools(PoolManagerClass instance, PoolManagerClass.PoolsCategory poolsCategory,
            PoolManagerClass.AssemblyType assemblyType, ResourceKey[] resources, object yield,
            IProgress<LoadingProgressStruct> progress = null,
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

            public readonly RefHelper.PropertyRef<JobPriorityClass, object> RefGeneral;

            public readonly RefHelper.PropertyRef<JobPriorityClass, object> RefLow;

            public readonly RefHelper.PropertyRef<JobPriorityClass, object> RefImmediate;

            private JobPriorityData()
            {
                RefGeneral = RefHelper.PropertyRef<JobPriorityClass, object>.Create("General");
                RefLow = RefHelper.PropertyRef<JobPriorityClass, object>.Create("Low");
                RefImmediate = RefHelper.PropertyRef<JobPriorityClass, object>.Create("Immediate");
            }
        }
    }
}