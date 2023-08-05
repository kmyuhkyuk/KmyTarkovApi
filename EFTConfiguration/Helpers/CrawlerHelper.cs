#if !UNITY_EDITOR
using System;
using System.Collections.Generic;
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

        private static readonly Dictionary<string, Task<Texture2D>> IconCacheFile =
            new Dictionary<string, Task<Texture2D>>();

        private static readonly Dictionary<string, Sprite> IconCache = new Dictionary<string, Sprite>();

        private static readonly Dictionary<string, string> IconURL;

        static CrawlerHelper()
        {
            var directory = new DirectoryInfo(CachePath);

            if (!directory.Exists)
            {
                directory.Create();
            }

            var files = directory.GetFiles("*.png");

            foreach (var file in files)
            {
                IconCacheFile.Add(Path.GetFileNameWithoutExtension(file.Name), GetAsyncTexture(file.FullName));
            }

            var cacheFileInfo = new FileInfo(CacheFilePath);

            if (!cacheFileInfo.Exists)
            {
                IconURL = new Dictionary<string, string>();
            }
            else
            {
                using (var stream = new StreamReader(cacheFileInfo.FullName))
                {
                    IconURL = JsonConvert.DeserializeObject<Dictionary<string, string>>(stream.ReadToEnd());
                }
            }
        }

        private static void Save()
        {
            using (var fs = new FileStream(CacheFilePath, FileMode.Create))
            {
                using (var write = new StreamWriter(fs))
                {
                    write.Write(JsonConvert.SerializeObject(IconURL));
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

            if (IconURL.TryGetValue(url, out var iconURL))
            {
                if (iconURL != url)
                {
                    IconURL.Remove(modURL);
                    IconURL.Add(modURL, url);
                }
            }
            else
            {
                IconURL.Add(modURL, url);
            }

            Save();

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
                Texture2D texture;
                if (IconCacheFile.TryGetValue(fileName, out var cacheTexture))
                {
                    texture = await cacheTexture;
                }
                else
                {
                    cacheTexture = GetAsyncTexture(url);

                    IconCacheFile.Add(fileName, cacheTexture);

                    texture = await cacheTexture;

                    if (texture == null)
                        return null;

                    File.WriteAllBytes(Path.Combine(CachePath, $"{fileName}.png"), texture.EncodeToPNG());
                }

                if (!IconCache.TryGetValue(fileName, out var sprite))
                {
                    sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f));

                    IconCache.Add(fileName, sprite);
                }

                return sprite;
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