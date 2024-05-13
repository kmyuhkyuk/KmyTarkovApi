using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Diz.Resources;
using EFT;
using EFTReflection;
using UnityEngine;

// ReSharper disable UnusedMember.Global

namespace EFTApi.Helpers
{
    public class EasyAssetsHelper
    {
        private static readonly Lazy<EasyAssetsHelper> Lazy =
            new Lazy<EasyAssetsHelper>(() => new EasyAssetsHelper());

        public static EasyAssetsHelper Instance => Lazy.Value;

        public static EasyAssetsExtensionData EasyAssetsExtensionHelper => EasyAssetsExtensionData.Instance;

        public EasyAssets EasyAssets { get; private set; }

        /// <summary>
        ///     Init Action
        /// </summary>
        public readonly RefHelper.HookRef Create;

        private EasyAssetsHelper()
        {
            var easyAssetsType = typeof(EasyAssets);

            Create = RefHelper.HookRef.Create(easyAssetsType, "Create");

            Create.Add(this, nameof(OnCreate));
        }

        private static async void OnCreate(Task<EasyAssets> __result)
        {
            Instance.EasyAssets = await __result;
        }

        public class EasyAssetsExtensionData
        {
            private static readonly Lazy<EasyAssetsExtensionData> Lazy =
                new Lazy<EasyAssetsExtensionData>(() => new EasyAssetsExtensionData());

            public static EasyAssetsExtensionData Instance => Lazy.Value;

            private readonly Func<EasyAssets, IEnumerable<string>, IProgress<float>, CancellationToken, object>
                _refRetain;

            private readonly Func<object, Task> _refLoadBundles;

            private readonly Func<EasyAssets, ResourceKey, GameObject> _refGetAsset;

            private EasyAssetsExtensionData()
            {
                var easyAssetsExtensionType = RefTool.GetEftType(x =>
                    x.GetMethod("WaitForAllBundlesJob", BindingFlags.Static | RefTool.Public) != null);

                _refRetain = RefHelper
                    .ObjectMethodDelegate<
                        Func<EasyAssets, IEnumerable<string>, IProgress<float>, CancellationToken, object>>(
                        RefTool.GetEftMethod(easyAssetsExtensionType, BindingFlags.Static | RefTool.Public,
                            x => x.Name == "Retain" &&
                                 x.GetParameters()[1].ParameterType == typeof(IEnumerable<string>)));

                _refLoadBundles =
                    RefHelper.ObjectMethodDelegate<Func<object, Task>>(
                        RefTool.GetEftMethod(easyAssetsExtensionType, BindingFlags.Static | RefTool.Public,
                            x => x.Name == "LoadBundles"
                                 && !x.GetParameters()[0].ParameterType.IsArray));

                _refGetAsset = RefHelper.ObjectMethodDelegate<Func<EasyAssets, ResourceKey, GameObject>>(
                    RefTool.GetEftMethod(easyAssetsExtensionType, BindingFlags.Static | RefTool.Public,
                            x => x.Name == "GetAsset" && x.GetParameters().Length == 2
                                                      && x.GetParameters()[1].ParameterType == typeof(ResourceKey))
                        .MakeGenericMethod(typeof(GameObject)));
            }

            public object Retain(EasyAssets easyAssets, IEnumerable<string> keys, IProgress<float> progress = null,
                CancellationToken ct = default)
            {
                return _refRetain(easyAssets, keys, progress, ct);
            }

            public Task LoadBundles(object bundles)
            {
                return _refLoadBundles(bundles);
            }

            public GameObject GetAsset(EasyAssets easyAssets, ResourceKey key)
            {
                return _refGetAsset(easyAssets, key);
            }
        }
    }
}