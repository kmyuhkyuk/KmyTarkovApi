using System.Linq;
using System.Threading.Tasks;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.Networking;

namespace EFTUtils
{
    public static class UnityWebRequestHelper
    {
        private static readonly ManualLogSource Logger =
            BepInEx.Logging.Logger.CreateLogSource(nameof(UnityWebRequestHelper));

        public static async Task<Texture2D> GetAsyncTexture(string url)
        {
            using (var www = UnityWebRequestTexture.GetTexture(url))
            {
                var sendWeb = www.SendWebRequest();

                while (!sendWeb.isDone)
                    await Task.Yield();

                if (www.isNetworkError || www.isHttpError)
                {
                    Logger.LogError(nameof(GetAsyncTexture) + ": " + "Network Error");
                    return null;
                }
                else
                {
                    var texture = DownloadHandlerTexture.GetContent(www);

                    return texture;
                }
            }
        }

        public static async Task<AudioClip> GetAsyncAudioClip(string url)
        {
            switch (url.Split('.').Last().ToLower())
            {
                case "wav":
                    return await GetAsyncAudioClip(url, AudioType.WAV);
                case "mp3":
                    return await GetAsyncAudioClip(url, AudioType.MPEG);
                case "ogg":
                    return await GetAsyncAudioClip(url, AudioType.OGGVORBIS);
                case "aif":
                    return await GetAsyncAudioClip(url, AudioType.AIFF);
                default:
                    return null;
            }
        }

        public static async Task<AudioClip> GetAsyncAudioClip(string url, AudioType audioType)
        {
            using (var www = UnityWebRequestMultimedia.GetAudioClip(url, audioType))
            {
                var sendWeb = www.SendWebRequest();

                while (!sendWeb.isDone)
                    await Task.Yield();

                if (www.isNetworkError || www.isHttpError)
                {
                    Logger.LogError(nameof(GetAsyncAudioClip) + ": " + "Network Error");
                    return null;
                }
                else
                {
                    return DownloadHandlerAudioClip.GetContent(www);
                }
            }
        }
    }
}