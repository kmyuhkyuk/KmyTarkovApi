using System.Reflection;
using Aki.Reflection.Patching;
using EFT;
using EFTReflection;
using EFTReflection.Patching;
using UnityEngine;

namespace EFTApi.Patches
{
    public class FirearmControllerInitiateShotPatch : BaseModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player.FirearmController).GetMethod("InitiateShot", RefTool.NonPublic);
        }

        [PatchPostfix]
        private static void PatchPostfix(Player.FirearmController __instance, Player ____player, BulletClass ammo, Vector3 shotPosition, Vector3 shotDirection, Vector3 fireportPosition, int chamberIndex, float overheat)
        {
            EFTHelpers._PlayerHelper.FirearmControllerHelper.Trigger_InitiateShot(__instance, ____player, ammo, shotPosition, shotDirection, fireportPosition, chamberIndex, overheat);
        }
    }
}
