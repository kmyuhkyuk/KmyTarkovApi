﻿using System.Collections.Generic;
using System.Reflection;
using EFT;
using HarmonyLib;
using KmyTarkovApi.Helpers;
using KmyTarkovReflection.Patching;

namespace KmyTarkovApi.Patches
{
    public class GamePlayerOwnerPatchs : ModulePatchs
    {
        protected override IEnumerable<MethodBase> GetTargetMethods()
        {
            yield return typeof(GamePlayerOwner).GetMethod("Init", AccessTools.all);
            yield return typeof(HideoutPlayerOwner).GetMethod("Init", AccessTools.all);
        }

        [PatchPostfix]
        private static void PatchPostfix(GamePlayerOwner __instance)
        {
            PlayerHelper.GamePlayerOwnerData.Instance.GamePlayerOwner = __instance;
        }
    }
}