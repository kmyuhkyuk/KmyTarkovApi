﻿#if !UNITY_EDITOR

using System.Reflection;
using Aki.Reflection.Patching;
using EFTConfiguration.Models;
using UnityEngine;

namespace EFTConfiguration.Patches
{
    public class CursorLockStatePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Cursor)
                .GetProperty("lockState", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                ?.GetSetMethod();
        }

        [PatchPrefix]
        private static bool PatchPrefix()
        {
            return !EFTConfigurationModel.Instance.Unlock;
        }
    }

    public class CursorVisiblePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Cursor)
                .GetProperty("visible", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
                ?.GetSetMethod();
        }

        [PatchPrefix]
        private static bool PatchPrefix()
        {
            return !EFTConfigurationModel.Instance.Unlock;
        }
    }
}

#endif