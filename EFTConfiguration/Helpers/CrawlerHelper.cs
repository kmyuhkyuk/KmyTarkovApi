#if !UNITY_EDITOR
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace EFTConfiguration.Helpers
{
    public static class CrawlerHelper
    {
        private static readonly string CachePath = Path.Combine(EFTConfigurationPlugin.ModPath, "cache");

        private static readonly string CacheFilePath = Path.Combine(CachePath, "cache.json");

        private static readonly ConcurrentDictionary<string, Task<Texture2D>> IconCacheFile =
            new ConcurrentDictionary<string, Task<Texture2D>>();

        private static readonly ConcurrentDictionary<string, Sprite> IconCache =
            new ConcurrentDictionary<string, Sprite>();

        private static readonly ConcurrentDictionary<string, string> IconURL;

        static CrawlerHelper()
        {
            var directory = new DirectoryInfo(CachePath);

            if (!directory.Exists)
            {
                directory.Create();
            }
            else
            {
                var files = directory.GetFiles("*.png");

                foreach (var file in files)
                {
                    IconCacheFile.TryAdd(Path.GetFileNameWithoutExtension(file.Name), GetAsyncTexture(file.FullName));
                }
            }

            var cacheFileInfo = new FileInfo(CacheFilePath);

            if (!cacheFileInfo.Exists)
            {
                IconURL = new ConcurrentDictionary<string, string>();
            }
            else
            {
                using (var stream = File.OpenText(cacheFileInfo.FullName))
                {
                    IconURL = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(stream.ReadToEnd());
                }
            }
        }

        public static async Task<HtmlDocument> CreateHtmlDocument(string url)
        {
            var web = new HtmlWeb();

            return await web.LoadFromWebAsync(url);
        }

        public static Version GetModVersion(HtmlDocument doc)
        {
            return new Version(new string(doc.DocumentNode.SelectSingleNode("//span[@class='filebaseVersionNumber']")
                .InnerText.Where(x => char.IsDigit(x) || x == '.').ToArray()));
        }

        /*public static DateTime GetModVersionDataTime(HtmlDocument doc)
        {
            return Convert.ToDateTime(doc.DocumentNode.SelectSingleNode("//div[1]/ul/li[2]/time").GetAttributeValue("datetime", string.Empty));
        }*/

        public static int GetModDownloads(HtmlDocument doc)
        {
            return Convert.ToInt32(doc.DocumentNode
                .SelectSingleNode("//meta[@itemprop='userInteractionCount']")
                .GetAttributeValue("content", string.Empty));
        }

        public static string GetModDownloadURL(HtmlDocument doc)
        {
            return doc.DocumentNode.SelectSingleNode("//*[@id=\"content\"]/header/nav/ul/li/a")
                .GetAttributeValue("href", string.Empty);
        }

        public static string GetModIconURL(HtmlDocument doc)
        {
            return doc.DocumentNode.SelectSingleNode("//*[@id=\"content\"]/header/div[1]/img")
                ?.GetAttributeValue("src", string.Empty);
        }

        public static async Task<Sprite> GetModIcon(HtmlDocument doc, string modURL)
        {
            var url = GetModIconURL(doc);

            if (string.IsNullOrEmpty(url))
                return null;

            IconURL.AddOrUpdate(modURL, url, (key, value) => url);

            File.WriteAllText(CacheFilePath, JsonConvert.SerializeObject(IconURL));

            return await LoadModIcon(url);
        }

        public static async Task<Sprite> GetModIcon(string modURL)
        {
            if (string.IsNullOrEmpty(modURL))
                return null;

            return IconURL.TryGetValue(modURL, out var url) ? await LoadModIcon(url) : null;
        }

        private static async Task<Sprite> LoadModIcon(string url)
        {
            var fileName = Path.GetFileNameWithoutExtension(url.Split('/').Last());

            if (IconCache.TryGetValue(fileName, out var cacheSprite))
            {
                return cacheSprite;
            }
            else
            {
                var cacheTexture = IconCacheFile.GetOrAdd(fileName, key2 => GetAsyncTexture(url));

                var texture = await cacheTexture;

                return IconCache.GetOrAdd(fileName, key3 =>
                {
                    File.WriteAllBytes(Path.Combine(CachePath, $"{fileName}.png"), texture.EncodeToPNG());

                    return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f));
                });
            }
        }

        private static async Task<Texture2D> GetAsyncTexture(string url)
        {
            using (var www = UnityWebRequestTexture.GetTexture(url))
            {
                var sendWeb = www.SendWebRequest();

                while (!sendWeb.isDone)
                    await Task.Yield();

                if (www.isNetworkError || www.isHttpError)
                {
                    return null;
                }
                else
                {
                    var texture = DownloadHandlerTexture.GetContent(www);

                    return texture;
                }
            }
        }
    }
}
#endif