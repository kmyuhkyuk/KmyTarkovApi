using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SevenZip;

namespace Build
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Program
    {
        private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        private static void Main(string[] args)
        {
            var hasArgs = args.Length > 0;

            var configurationName = hasArgs ? args[0] : "Release";

            switch (configurationName)
            {
                case "Release":
                    const string releasePath =
                        "R:\\Battlestate Games\\Client.0.13.5.3.26535\\BepInEx\\plugins\\kmyuhkyuk-EFTApi";

                    Copy(releasePath, new[]
                    {
                        "EFTApi",
                        "EFTUtils",
                        "EFTReflection",
                        "EFTConfiguration",
                        "HtmlAgilityPack",
                        "Crc32.NET",
                        "ConfigurationTest"
                    });

                    SevenZip(releasePath, null, new[] { "cache" }, new[] { "ConfigurationTest.dll" });
                    break;
                case "UNITY_EDITOR":
                    const string unityEditorPath = "C:\\Users\\24516\\Documents\\EFTConfiguration\\Assets\\Managed";

                    Copy(unityEditorPath,
                        new[] { "EFTConfiguration" });
                    break;
            }

            if (!hasArgs)
            {
                Console.WriteLine("\nPress any key to close console app...");
                Console.ReadKey();
            }
        }

        private static void SevenZip(string path)
        {
            SevenZip(path, null, Array.Empty<string>(), Array.Empty<string>());
        }

        private static void SevenZip(string path, Dictionary<string, string> addFileDictionary,
            string[] excludeDirectoryNames, string[] excludeFileNames)
        {
            var directory = new DirectoryInfo(path);

            if (directory.Parent == null)
            {
                throw new ArgumentNullException(nameof(directory.Parent));
            }

            var directoryFullName = $"{directory.FullName}\\";

            SevenZipBase.SetLibraryPath(
                $@"{Environment.CurrentDirectory}\{(IntPtr.Size == 4 ? "x86" : "x64")}\7z.dll");

            var compressor = new SevenZipCompressor();

            var filesDictionary = addFileDictionary ?? new Dictionary<string, string>();
            foreach (var file in directory.GetFiles("*", SearchOption.AllDirectories))
            {
                if (file.Directory == null)
                {
                    throw new ArgumentNullException(nameof(file.Directory));
                }

                var fileDirectoryName = file.Directory.FullName.Replace(directoryFullName, string.Empty);

                if (!string.IsNullOrEmpty(fileDirectoryName))
                {
                    if (excludeDirectoryNames.Contains(fileDirectoryName))
                    {
                        Console.WriteLine($"Exclude {fileDirectoryName} Directory\nSkip {file.FullName}");
                        continue;
                    }

                    var fileName = file.FullName.Replace(directoryFullName, string.Empty);

                    if (excludeFileNames.Contains(fileName))
                    {
                        Console.WriteLine($"Exclude {fileName} File\nSkip {file.FullName}");
                        continue;
                    }
                }

                filesDictionary.Add(
                    file.FullName.Replace(directory.Parent.FullName, "BepInEx\\plugins"),
                    file.FullName);
            }

            using (var stream = File.Create(Path.Combine(directory.Parent.FullName, $"{directory.Name}.7z")))
            {
                compressor.CompressFileDictionary(filesDictionary, stream);
            }
        }

        private static void Copy(string toPath, string[] dllNames)
        {
            foreach (var dllName in dllNames)
            {
                var dllFullName = $"{dllName}.dll";

                var dllPath = Path.Combine(BaseDirectory, dllFullName);

                var toDLLPath = Path.Combine(toPath, dllFullName);

                Console.WriteLine($"Copy {Path.GetFileName(dllFullName)} to \n{toDLLPath}");

                File.Copy(dllPath, toDLLPath, true);
            }
        }
    }
}