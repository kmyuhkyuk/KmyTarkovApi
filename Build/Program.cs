using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CopyBuildAssembly;

// ReSharper disable ClassNeverInstantiated.Global

namespace Build
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var arg = args.ElementAtOrDefault(0);
            var sha = Copy.GetTipSha(args.ElementAtOrDefault(1));

            const string modPath =
                @"R:\Battlestate Games\Client.0.16.9.0.40087\BepInEx\plugins\kmyuhkyuk-KmyTarkovApi";

            var modDirectoryName = new DirectoryInfo(modPath).Name;

            Console.WriteLine(modDirectoryName);

            var currentPath = Path.Combine(baseDirectory, "../Current");

            const string versionName = "1.4.1";

            var releaseName = $"{modDirectoryName}(Release_{versionName}).7z";

            var previewName = $"{modDirectoryName}(Preview).7z";

            var releasePreview = new[]
            {
                "Release",
                "Preview"
            };

            try
            {
                Copy.CopyFolder(arg, releasePreview, Path.Combine(baseDirectory, "localized"),
                    Path.Combine(modPath, "localized"));

                Copy.CopyAssembly(arg, releasePreview, baseDirectory, currentPath, new[]
                {
                    "KmyTarkovApi",
                    "KmyTarkovUtils",
                    "KmyTarkovReflection",
                    "KmyTarkovConfiguration",
                    "HtmlAgilityPack",
                    "Crc32.NET",
                    "ConfigurationTest"
                });

                Copy.CopyAssembly(arg, releasePreview, currentPath, modPath, new[]
                {
                    "HtmlAgilityPack",
                    "Crc32.NET",
                    "ConfigurationTest"
                });

                Copy.CopyAssembly(arg, releasePreview, currentPath, modPath, new[]
                {
                    "KmyTarkovApi",
                    "KmyTarkovUtils",
                    "KmyTarkovReflection",
                    "KmyTarkovConfiguration"
                }, sha);

                Copy.GenerateSevenZip(arg, "Release", modPath, releaseName, @"BepInEx\plugins", new[]
                {
                    "ConfigurationTest.dll"
                }, new[]
                {
                    "cache"
                }, Array.Empty<string>(), Array.Empty<string>());

                Copy.GenerateSevenZip(arg, "Preview", modPath, previewName, @"BepInEx\plugins", new[]
                {
                    "ConfigurationTest.dll"
                }, new[]
                {
                    "cache"
                }, Array.Empty<string>(), Array.Empty<string>());

                //Unity

                const string unityEditorPath = @"C:\Users\24516\Documents\KmyTarkovConfiguration\Assets\Managed";

                Copy.CopyAssembly(arg, "UNITY_EDITOR", baseDirectory, unityEditorPath, new[]
                {
                    "KmyTarkovConfiguration"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                Console.ReadKey();

                Process.GetCurrentProcess().Kill();
            }
        }
    }
}