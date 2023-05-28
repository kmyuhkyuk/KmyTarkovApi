using System.Reflection;
using Aki.Reflection.Patching;
using EFT.InventoryLogic;
using EFTReflection;
using EFTReflection.Patching;

namespace EFTApi.Patches
{
    public class ArmorComponentApplyDurabilityDamagePatch : BaseModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(ArmorComponent).GetMethod("ApplyDurabilityDamage", RefTool.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(ArmorComponent __instance, float armorDamage)
        {
            EFTHelpers._PlayerHelper.ArmorComponentHelper.Trigger_ApplyDurabilityDamage(__instance, armorDamage);
        }
    }

    public class ArmorComponentApplyDamagePatch : BaseModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(ArmorComponent).GetMethod("ApplyDamage", RefTool.Public);
        }

        [PatchPostfix]
        private static void PatchPostfix(ArmorComponent __instance, DamageInfo damageInfo, EBodyPart bodyPartType,
            bool damageInfoIsLocal, object lightVestsDamageReduction, object heavyVestsDamageReduction)
        {
            EFTHelpers._PlayerHelper.ArmorComponentHelper.Trigger_ApplyDamage(__instance, damageInfo, bodyPartType,
                damageInfoIsLocal, lightVestsDamageReduction, heavyVestsDamageReduction);
        }
    }
}