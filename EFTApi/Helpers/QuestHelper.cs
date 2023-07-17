using System.Reflection;
using EFTReflection;

namespace EFTApi.Helpers
{
    public class QuestHelper
    {
        public static readonly QuestHelper Instance = new QuestHelper();

        public readonly RefHelper.HookRef OnConditionValueChanged;

        private QuestHelper()
        {
            OnConditionValueChanged = new RefHelper.HookRef(RefTool.GetEftType(x =>
                    x.GetMethod("OnConditionValueChanged", BindingFlags.DeclaredOnly | RefTool.NonPublic) != null),
                "OnConditionValueChanged");
        }
    }
}