using System;
using EFT.Quests;

namespace EFTApi.Helpers
{
    public class QuestHelper
    {
        public event Action<object, object, EQuestStatus, Condition, bool> OnConditionValueChanged;

        internal void Trigger_OnConditionValueChanged(object questController, object quest, EQuestStatus status, Condition condition, bool notify)
        {
            OnConditionValueChanged?.Invoke(questController, quest, status, condition, notify);
        }
    }
}
