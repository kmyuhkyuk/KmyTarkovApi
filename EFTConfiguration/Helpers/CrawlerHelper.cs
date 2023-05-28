#if !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using UnityEngine;
using UnityEngine.Networking;

namespace EFTConfiguration.Helpers
{
    public static class CrawlerHelper
    {
        private static readonly string CachePath = Path.Combine(EFTConfigurationPlugin.ModPath, "cache");

        private static readonly Dictionary<string, Task<Texture2D>> IconCacheFile = new Dictionary<string, Task<Texture2D>>();

        private static readonly Dictionary<string, Sprite> IconCache = new Dictionary<string, Sprite>();

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
        }

        public static async Task<HtmlDocument> CreateHtmlDocument(string url)
        {
            var web = new HtmlWeb();

            return await web.LoadFromWebAsync(url);
        }

        public static Version GetModVersion(HtmlDocument doc)
        {
            return new Version(doc.DocumentNode.SelectSingleNode("//header/div[2]/h1/span[3]").InnerText);
        }

        /*public static DateTime GetModVersionDataTime(HtmlDocument doc)
        {
            return Convert.ToDateTime(doc.DocumentNode.SelectSingleNode("//div[1]/ul/li[2]/time").GetAttributeValue("datetime", ""));
        }*/

        public static int GetModDownloads(HtmlDocument doc)
        {
            return Convert.ToInt32(doc.DocumentNode.SelectSingleNode("//header/div[2]/ul/li[5]/meta[2]").GetAttributeValue("content", ""));
        }

        public static string GetModDownloadUrl(HtmlDocument doc)
        {
            return doc.DocumentNode.SelectSingleNode("//header/nav/ul/li/a").GetAttributeValue("href", "");
        }

        public static string GetModIconUrl(HtmlDocument doc)
        {
            return doc.DocumentNode.SelectSingleNode("//header/div[1]/img").GetAttributeValue("src", "");
        }

        public static async Task<Sprite> GetModIcon(HtmlDocument doc)
        {
            var url = GetModIconUrl(doc);

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
                    texture = await GetAsyncTexture(url);

                    File.WriteAllBytes(Path.Combine(CachePath, fileName + ".png"), texture.EncodeToPNG());
                }

                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                IconCache.Add(fileName, sprite);

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
