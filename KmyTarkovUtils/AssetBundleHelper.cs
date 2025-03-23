using System;
using System.Threading.Tasks;
using BepInEx.Logging;
using UnityEngine;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace KmyTarkovUtils
{
    public static class AssetBundleHelper
    {
        private static readonly ManualLogSource Logger =
            BepInEx.Logging.Logger.CreateLogSource(nameof(AssetBundleHelper));

        public static AssetBundle LoadBundle(string bundlePath)
        {
            var assetBundle = AssetBundle.LoadFromFile(bundlePath);

            if (assetBundle != null)
                return assetBundle;

            Logger.LogError($"{nameof(LoadBundle)}: {bundlePath} Failed to load AssetBundle!");

            return null;
        }

        public static async Task<AssetBundle> LoadAsyncBundle(string bundlePath)
        {
            var www = AssetBundle.LoadFromFileAsync(bundlePath);

            while (!www.isDone)
                await Task.Yield();

            if (www.assetBundle != null)
                return www.assetBundle;

            Logger.LogError($"{nameof(LoadAsyncBundle)}: {bundlePath} Failed to load AssetBundle!");

            return null;
        }

        public static async Task<T[]> LoadAsyncAllAsset<T>(AssetBundle assetBundle) where T : UnityEngine.Object
        {
            if (assetBundle != null)
            {
                var www = assetBundle.LoadAllAssetsAsync<T>();

                while (!www.isDone)
                    await Task.Yield();

                if (www.allAssets != null)
                    return Array.ConvertAll(www.allAssets, x => (T)x);

                Logger.LogError($"{nameof(LoadAsyncAllAsset)}: {assetBundle.name} Failed to load AllAssets!");

                return null;
            }

            Logger.LogError($"{nameof(LoadAsyncAllAsset)}: {nameof(assetBundle)} is null!");

            return null;
        }
    }
}