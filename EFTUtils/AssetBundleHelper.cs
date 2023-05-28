using System;
using System.Threading.Tasks;
using BepInEx.Logging;
using UnityEngine;

namespace EFTUtils
{
    public static class AssetBundleHelper
    {
        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(AssetBundleHelper));

        public static AssetBundle LoadBundle(string bundlePath)
        {
            var assetBundle = AssetBundle.LoadFromFile(bundlePath);

            if (assetBundle == null)
            {
                Logger.LogError(nameof(LoadBundle) + ": " + "Failed to load AssetBundle!");

                return null;
            }
            else
            {
                return assetBundle;
            }
        }

        public static async Task<AssetBundle> LoadAsyncBundle(string bundlePath)
        {
            var www = AssetBundle.LoadFromFileAsync(bundlePath);

            while (!www.isDone)
                await Task.Yield();

            if (www.assetBundle == null)
            {
                Logger.LogError(nameof(LoadAsyncBundle) + ": " + "Failed to load AssetBundle!");

                return null;
            }
            else
            {
                return www.assetBundle;
            }
        }

        public static async Task<T[]> LoadAsyncAllAsset<T>(AssetBundle assetBundle) where T : UnityEngine.Object
        {
            if (assetBundle != null)
            {
                var www = assetBundle.LoadAllAssetsAsync<T>();

                while (!www.isDone)
                    await Task.Yield();

                if (www.allAssets != null)
                {
                    return Array.ConvertAll(www.allAssets, x => (T)x);
                }
                else
                {
                    Logger.LogError(nameof(LoadAsyncAllAsset) + ": " + "Failed to load AllAssets");
                    return null;
                }
            }
            else
            {
                Logger.LogError(nameof(LoadAsyncAllAsset) + ": " + "Failed to load AssetBundle");
                return null;
            }
        }
    }
}
