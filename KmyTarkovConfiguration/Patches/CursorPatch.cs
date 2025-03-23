#if !UNITY_EDITOR

using HarmonyLib;
using KmyTarkovConfiguration.Models;
using UnityEngine;

namespace KmyTarkovConfiguration.Patches
{
    [HarmonyPatch(typeof(Cursor), "lockState", MethodType.Setter)]
    public class CursorLockStatePatch
    {
        private static bool Prefix()
        {
            return !EFTConfigurationModel.Instance.Unlock;
        }
    }

    [HarmonyPatch(typeof(Cursor), "visible", MethodType.Setter)]
    public class CursorVisiblePatch
    {
        private static bool Prefix()
        {
            return !EFTConfigurationModel.Instance.Unlock;
        }
    }
}

#endif