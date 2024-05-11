using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

// ReSharper disable NotAccessedField.Global
// ReSharper disable UnusedMember.Global

namespace EFTUtils
{
    public static class FileDialog
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class OpenFileName
        {
            public int structSize;
            public IntPtr dlgOwner;
            public IntPtr instance;
            public string filter;
            public string customFilter;
            public int maxCustFilter;
            public int filterIndex;
            public IntPtr file;
            public int maxFile;
            public string fileTitle;
            public int maxFileTitle;
            public string initialDir;
            public string title;
            public int flags;
            public short fileOffset;
            public short fileExtension;
            public string defExt;
            public IntPtr custData;
            public IntPtr hook;
            public string templateName;
            public IntPtr reservedPtr;
            public int reservedInt;
            public int flagsEx;
        }

        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);

        public static string OpenFileDialog(string title, bool multipleFile = false)
        {
            return OpenFileDialog(title, new[] { new FileFilter("All Files", "*.*") }, multipleFile);
        }

        public static string OpenFileDialog(string title, FileFilter[] filter, bool multipleFile = false)
        {
            return OpenFileDialog(title, Application.dataPath, filter, multipleFile);
        }

        public static string OpenFileDialog(string title, string initialDir, bool multipleFile = false)
        {
            return OpenFileDialog(title, initialDir, new[] { new FileFilter("All Files", "*.*") }, multipleFile);
        }

        public static string OpenFileDialog(string title, string initialDir, FileFilter[] filter,
            bool multipleFile = false)
        {
            var flags = 0x00080000 | 0x00001000 | 0x00000800; //OFN_EXPLORER | OFN_FILEMUSTEXIST | OFN_PATHMUSTEXIST
            if (multipleFile)
            {
                flags |= 0x00000200; //OFN_ALLOWMULTISELECT
            }

            return OpenFileDialog(title, initialDir, filter, string.Empty, flags);
        }

        public static string OpenFileDialog(string title, string initialDir, FileFilter[] filter, string defExt,
            int flags)
        {
            var ofn = new OpenFileName();

            ofn.structSize = Marshal.SizeOf(ofn);
            ofn.filter = string.Join(string.Empty, filter.Select(x => $"{x.Name}\0{x.Extension}\0"));
            ofn.fileTitle = new string(new char[64]);
            ofn.maxFileTitle = ofn.fileTitle.Length;
            ofn.initialDir = initialDir;
            ofn.title = title;
            ofn.defExt = defExt;
            ofn.flags = flags;

            var fileString = new string(new char[2048]);
            ofn.file = Marshal.StringToBSTR(fileString);
            ofn.maxFile = fileString.Length;

            if (!GetOpenFileName(ofn))
                return string.Empty;

            var pointer = (long)ofn.file;
            var file = Marshal.PtrToStringAuto(ofn.file);

            var fileNameList = new List<string>();

            while (file?.Length > 0)
            {
                fileNameList.Add(file);

                pointer += file.Length * Marshal.SystemDefaultCharSize + Marshal.SystemDefaultCharSize;
                ofn.file = (IntPtr)pointer;
                file = Marshal.PtrToStringAuto(ofn.file);
            }

            switch (fileNameList.Count)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return fileNameList[0];
            }

            var directoryFullName = fileNameList[0];

            var stringBuilder = new StringBuilder();

            for (var i = 1; i < fileNameList.Count; i++)
            {
                var fileName = fileNameList[i];

                stringBuilder.Append(Path.Combine(directoryFullName, fileName));

                if (i < fileNameList.Count - 1)
                {
                    stringBuilder.Append('|');
                }
            }

            return stringBuilder.ToString();
        }

        public static string SaveFileDialog(string title)
        {
            return SaveFileDialog(title, new[] { new FileFilter("All Files", "*.*") });
        }

        public static string SaveFileDialog(string title, FileFilter[] filter)
        {
            return SaveFileDialog(title, Application.dataPath, filter);
        }

        public static string SaveFileDialog(string title, string initialDir)
        {
            return SaveFileDialog(title, initialDir, new[] { new FileFilter("All Files", "*.*") });
        }

        public static string SaveFileDialog(string title, string initialDir, FileFilter[] filter)
        {
            const int
                flags = 0x00080000 | 0x00000002 | 0x00000004; //OFN_EXPLORER | OFN_OVERWRITEPROMPT | OFN_HIDEREADONLY;

            return SaveFileDialog(title, initialDir, filter, string.Empty, flags);
        }

        public static string SaveFileDialog(string title, string initialDir, FileFilter[] filter, string defExt,
            int flags)
        {
            var ofn = new OpenFileName();

            ofn.structSize = Marshal.SizeOf(ofn);
            ofn.filter = string.Join(string.Empty, filter.Select(x => $"{x.Name}\0{x.Extension}\0"));
            ofn.fileTitle = new string(new char[64]);
            ofn.maxFileTitle = ofn.fileTitle.Length;
            ofn.initialDir = initialDir;
            ofn.title = title;
            ofn.defExt = defExt;
            ofn.flags = flags;

            var fileString = new string(new char[256]);
            ofn.file = Marshal.StringToBSTR(fileString);
            ofn.maxFile = fileString.Length;

            return GetSaveFileName(ofn) ? Marshal.PtrToStringAuto(ofn.file) : string.Empty;
        }

        public struct FileFilter
        {
            public string Name;

            public string Extension;

            public FileFilter(string name, string extension)
            {
                Name = name;
                Extension = extension;
            }
        }
    }
}