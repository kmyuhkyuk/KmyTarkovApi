#if !UNITY_EDITOR

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BepInEx.Logging;
using EFTConfiguration.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

// ReSharper disable MemberCanBePrivate.Global

namespace EFTConfiguration.Helpers
{
    public static class CrawlerHelper
    {
        private static readonly string CachePath = Path.Combine(EFTConfigurationModel.Instance.ModPath, "cache");

        private static readonly string CacheFilePath = Path.Combine(CachePath, "cache.json");

        private static readonly ConcurrentDictionary<string, string> IconURL;

        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource(nameof(CrawlerHelper));

        static CrawlerHelper()
        {
            Directory.CreateDirectory(CachePath);

            if (!File.Exists(CacheFilePath))
            {
                IconURL = new ConcurrentDictionary<string, string>();
            }
            else
            {
                IconURL = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(
                    File.ReadAllText(CacheFilePath));
            }
        }

        public static async Task<HtmlDocument> CreateHtmlDocument(string url)
        {
            try
            {
                return await new HtmlWeb().LoadFromWebAsync(url);
            }
            catch
            {
                return null;
            }
        }

        public static Version GetModVersion(HtmlDocument doc)
        {
            try
            {
                return Version.Parse(new Regex(@"[^\d.]").Replace(doc.DocumentNode
                    .SelectSingleNode("//span[@class='filebaseVersionNumber']")
                    .InnerText, string.Empty));
            }
            catch
            {
                return null;
            }
        }

        public static int GetModDownloadCount(HtmlDocument doc)
        {
            try
            {
                return Convert.ToInt32(doc.DocumentNode
                    .SelectSingleNode("//meta[@itemprop='userInteractionCount']")
                    .GetAttributeValue("content", string.Empty));
            }
            catch
            {
                return 0;
            }
        }

        public static string GetModDownloadURL(HtmlDocument doc)
        {
            try
            {
                return doc.DocumentNode.SelectSingleNode("//*[@id=\"content\"]/header/nav/ul/li/a")
                    .GetAttributeValue("href", string.Empty);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string GetModIconURL(HtmlDocument doc)
        {
            try
            {
                return doc.DocumentNode.SelectSingleNode("//*[@id=\"content\"]/header/div[1]/img")
                    ?.GetAttributeValue("src", string.Empty);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static Task<Sprite> GetModIcon(HtmlDocument doc, string modURL)
        {
            var url = GetModIconURL(doc);

            if (string.IsNullOrEmpty(url))
                return LoadModIcon(modURL, true);

            IconURL.AddOrUpdate(modURL, url, (key, value) => url);

            try
            {
                File.WriteAllText(CacheFilePath, JsonConvert.SerializeObject(IconURL));
            }
            catch
            {
                Logger.LogWarning($"Can't write {CacheFilePath}");
            }

            return LoadModIcon(url, false);
        }

        private static async Task<Sprite> LoadModIcon(string url, bool isLocal)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(url.Split('/').Last());

            var filePath = Path.Combine(CachePath, $"{fileNameWithoutExtension}.png");

            var texture = await GetAsyncTexture(isLocal ? filePath : url);

            if (texture == null)
                return null;

            if (isLocal)
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));

            try
            {
                File.WriteAllBytes(filePath, texture.EncodeToPNG());
            }
            catch
            {
                Logger.LogWarning($"Can't write {filePath}");
            }

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));
        }

        private static async Task<Texture2D> GetAsyncTexture(string url)
        {
            using (var www = UnityWebRequestTexture.GetTexture(url))
            {
                var sendWeb = www.SendWebRequest();

                while (!sendWeb.isDone)
                    await Task.Yield();

                if (www.isNetworkError || www.isHttpError)
                    return null;

                return DownloadHandlerTexture.GetContent(www);
            }
        }
    }
}

#endif