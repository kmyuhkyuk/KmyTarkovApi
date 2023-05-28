using System.Reflection;
using Aki.Reflection.Patching;
using EFT.Quests;
using EFTReflection;
using EFTReflection.Patching;

namespace EFTApi.Patches
{
    public class OnConditionValueChangedPatch : BaseModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            var flags = BindingFlags.DeclaredOnly | RefTool.NonPublic;

            return RefTool.GetEftType(x => x.GetMethod("OnConditionValueChanged", flags) != null).GetMethod("OnConditionValueChanged", flags);
        }

        [PatchPostfix]
        private static void PatchPostfix(object __instance, object quest, EQuestStatus status, Condition condition, bool notify)
        {
            EFTHelpers._QuestHelper.Trigger_OnConditionValueChanged(__instance, quest, status, condition, notify);
        }
    }
}
