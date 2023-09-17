using System;
using EFTConfiguration.Components.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EFTConfiguration.Components.ValueType
{
    public class ConfigToggle : ConfigGetValue<bool>
    {
        [SerializeField] private Toggle toggle;

        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, bool defaultValue, Action<bool> onValueChanged, bool hideReset, Func<bool> currentValue)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset, currentValue);

            toggle.onValueChanged.AddListener(new UnityAction<bool>(onValueChanged));
            toggle.interactable = !readOnly;
        }

        public override void UpdateCurrentValue()
        {
            toggle.isOn = GetValue();
        }
    }
}