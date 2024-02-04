using System;
using System.Reflection;
using EFTReflection;
using HarmonyLib;

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
            var questControllerType = RefTool.GetEftType(x =>
                x.GetMethod("IsQuestForCurrentProfile", BindingFlags.DeclaredOnly | RefTool.Public) != null);

            if (EFTVersion.AkiVersion > EFTVersion.Parse("3.7.6"))
            {
                questControllerType = questControllerType.BaseType;
            }

            if (questControllerType == null)
            {
                throw new ArgumentNullException(nameof(questControllerType));
            }

            OnConditionValueChanged =
                RefHelper.HookRef.Create(questControllerType.GetMethod("OnConditionValueChanged", AccessTools.all));
        }
    }
}