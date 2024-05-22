using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Diz.Resources;
using EFT;
using EFTReflection;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

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

            private readonly MethodInfo _getAssetMethodInfo;

            private readonly Func<EasyAssets, string, string, GameObject> _refGetAssets;

            private readonly Dictionary<Type, Func<EasyAssets, string, string, Object>> _getAssetsDictionary;

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

                _getAssetMethodInfo = RefTool.GetEftMethod(easyAssetsExtensionType,
                    BindingFlags.Static | RefTool.Public,
                    x => x.Name == "GetAsset" && x.GetParameters().Length == 3
                                              && x.GetParameters()[1].ParameterType == typeof(string));

                _refGetAssets = AccessTools.MethodDelegate<Func<EasyAssets, string, string, GameObject>>(
                    _getAssetMethodInfo.MakeGenericMethod(typeof(GameObject)));

                _getAssetsDictionary = new Dictionary<Type, Func<EasyAssets, string, string, Object>>
                {
                    { typeof(GameObject), _refGetAssets }
                };
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
                return GetAsset(easyAssets, key.path, key.rcid);
            }

            public GameObject GetAsset(EasyAssets easyAssets, string key, string assetName = null)
            {
                return _refGetAssets(easyAssets, key, assetName);
            }

            public Func<EasyAssets, string, string, T> GetAssetDelegate<T>() where T : Object
            {
                if (_getAssetsDictionary.TryGetValue(typeof(T), out var value))
                {
                    return (Func<EasyAssets, string, string, T>)value;
                }

                var newValue = AccessTools.MethodDelegate<Func<EasyAssets, string, string, T>>(
                    _getAssetMethodInfo.MakeGenericMethod(typeof(T)));

                _getAssetsDictionary.Add(typeof(T), newValue);

                return newValue;
            }
        }
    }
}