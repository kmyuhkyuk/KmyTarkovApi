using System;
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

            const string modPath =
                @"R:\Battlestate Games\Client.0.14.1.0.28744\BepInEx\plugins\kmyuhkyuk-EFTApi";

            var currentPath = Path.Combine(baseDirectory, "../Current");

            var previewName = $"{new DirectoryInfo(modPath).Name}-(Preview).7z";

            var releasePreview = new[]
            {
                "Release",
                "Preview"
            };

            Copy.CopyAssembly(arg, releasePreview, baseDirectory, currentPath, new[]
            {
                "EFTApi",
                "EFTUtils",
                "EFTReflection",
                "EFTConfiguration",
                "HtmlAgilityPack",
                "Crc32.NET",
                "ConfigurationTest"
            });

            Copy.CopyAssembly(arg, releasePreview, baseDirectory, modPath, new[]
            {
                "EFTApi",
                "EFTUtils",
                "EFTReflection",
                "EFTConfiguration",
                "HtmlAgilityPack",
                "Crc32.NET",
                "ConfigurationTest"
            });

            Copy.GenerateSevenZip(arg, "Release", modPath, null, @"BepInEx\plugins", new[]
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

            const string unityEditorPath = @"C:\Users\24516\Documents\EFTConfiguration\Assets\Managed";

            Copy.CopyAssembly(arg, "UNITY_EDITOR", baseDirectory, unityEditorPath, new[]
            {
                "EFTConfiguration"
            });
        }
    }
}