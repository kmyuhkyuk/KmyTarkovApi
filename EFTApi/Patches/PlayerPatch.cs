using System.Reflection;
using System.Threading.Tasks;
using Aki.Reflection.Patching;
using EFT;
using EFT.InventoryLogic;
using EFTReflection;
using EFTReflection.Patching;
using UnityEngine;

namespace EFTApi.Patches
{
    public class PlayerPatch : BaseModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("Init", RefTool.NonPublic);
        }

        [PatchPostfix]
        private static async void PatchPostfix(Player __instance, Task __result, Quaternion rotation, string layerName, EPointOfView pointOfView, Profile profile, object inventoryController, IHealthController healthController, object statisticsManager, object questController, object filter, Player.EVoipState voipState, bool aiControlled, bool async)
        {
            await __result;

            EFTHelpers._PlayerHelper.Trigger_Init(__instance, rotation, layerName, pointOfView, profile, inventoryController, healthController, statisticsManager, questController, filter, voipState, aiControlled, async);
        }
    }

    public class PlayerDisposePatch : BaseModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("Dispose", RefTool.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(Player __instance)
        {
            EFTHelpers._PlayerHelper.Trigger_Dispose(__instance);
        }
    }

    public class PlayerOnDeadPatch : BaseModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("OnDead", RefTool.NonPublic);
        }

        [PatchPostfix]
        private static void PatchPostfix(Player __instance, EDamageType damageType)
        {
            EFTHelpers._PlayerHelper.Trigger_OnDead(__instance, damageType);
        }
    }

    public class PlayerApplyDamageInfoPatch : BaseModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("ApplyDamageInfo", RefTool.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(Player __instance, DamageInfo damageInfo, EBodyPart bodyPartType, float absorbed, EHeadSegment? headSegment)
        {
            EFTHelpers._PlayerHelper.Trigger_ApplyDamageInfo(__instance, damageInfo, bodyPartType, absorbed, headSegment);
        }
    }

    public class PlayerOnBeenKilledByAggressorPatch : BaseModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod("OnBeenKilledByAggressor", RefTool.NonPublic);
        }

        [PatchPostfix]
        private static void PatchPostfix(Player __instance, Player aggressor, DamageInfo damageInfo, EBodyPart bodyPart, EDamageType lethalDamageType)
        {
            EFTHelpers._PlayerHelper.Trigger_OnBeenKilledByAggressor(__instance, aggressor, damageInfo, bodyPart, lethalDamageType);
        }
    }
}
