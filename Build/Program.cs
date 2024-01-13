using System;
using System.Linq;
using CopyBuildAssembly;

namespace Build
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Program
    {
        private static void Main(string[] args)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var arg = args.ElementAtOrDefault(0);

            const string releasePath =
                "R:\\Battlestate Games\\Client.0.13.5.3.26535\\BepInEx\\plugins\\kmyuhkyuk-EFTApi";

            Copy.CopyAssembly(arg, "Release", baseDirectory, releasePath,
                new[]
                {
                    "EFTApi",
                    "EFTUtils",
                    "EFTReflection",
                    "EFTConfiguration",
                    "HtmlAgilityPack",
                    "Crc32.NET",
                    "ConfigurationTest"
                });

            const string unityEditorPath = "C:\\Users\\24516\\Documents\\EFTConfiguration\\Assets\\Managed";

            Copy.CopyAssembly(arg, "UNITY_EDITOR", baseDirectory, unityEditorPath,
                new[]
                {
                    "EFTConfiguration"
                });
        }
    }
}