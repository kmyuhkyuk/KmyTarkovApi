using System;
using System.Reflection;
using EFTReflection;

// ReSharper disable NotAccessedField.Global

namespace EFTApi.Helpers
{
    public class QuestHelper
    {
        private static readonly Lazy<QuestHelper> Lazy = new Lazy<QuestHelper>(() => new QuestHelper());

        public static QuestHelper Instance => Lazy.Value;

        public readonly RefHelper.HookRef OnConditionValueChanged;

        private QuestHelper()
        {
            OnConditionValueChanged = RefHelper.HookRef.Create(RefTool.GetEftType(x =>
                    x.GetMethod("OnConditionValueChanged", BindingFlags.DeclaredOnly | RefTool.NonPublic) != null),
                "OnConditionValueChanged");
        }
    }
}