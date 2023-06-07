using System.Reflection;
using EFT.Quests;
using EFTReflection;
using EFTReflection.Patching;

namespace EFTApi.Helpers
{
    public class QuestHelper
    {
        public static readonly QuestHelper Instance = new QuestHelper();

        public event hook_OnConditionValueChanged OnConditionValueChanged
        {
            add => HookPatch.Add(RefTool.GetEftType(x =>
                    x.GetMethod("OnConditionValueChanged", BindingFlags.DeclaredOnly | RefTool.NonPublic) != null)
                .GetMethod("OnConditionValueChanged", BindingFlags.DeclaredOnly | RefTool.NonPublic), value);
            remove => HookPatch.Remove(RefTool.GetEftType(x =>
                    x.GetMethod("OnConditionValueChanged", BindingFlags.DeclaredOnly | RefTool.NonPublic) != null)
                .GetMethod("OnConditionValueChanged", BindingFlags.DeclaredOnly | RefTool.NonPublic), value);
        }

        public delegate void hook_OnConditionValueChanged(object __instance, object quest, EQuestStatus status,
            Condition condition,
            bool notify);

        private QuestHelper()
        {
        }
    }
}