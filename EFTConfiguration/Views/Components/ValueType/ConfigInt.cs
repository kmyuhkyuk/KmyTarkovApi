using System;
using EFTConfiguration.Views.Components.Base;
using TMPro;
using UnityEngine;

namespace EFTConfiguration.Views.Components.ValueType
{
    public class ConfigInt : ConfigNum<int>
    {
        [SerializeField] private TMP_InputField intValue;

        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, int defaultValue, Action<int> onValueChanged, bool hideReset, Func<int> currentValue)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset, currentValue);

            intValue.onEndEdit.AddListener(value =>
            {
                var intNum = int.Parse(value);

                onValueChanged(intNum);

                intValue.text = intNum.ToString();
            });
            intValue.interactable = !readOnly;
        }

        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, int defaultValue, Action<int> onValueChanged, bool hideReset, Func<int> currentValue,
            int min,
            int max)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset, currentValue, min, max);

            intValue.onEndEdit.AddListener(value =>
            {
                var intNum = Mathf.Clamp(int.Parse(value), min, max);

                onValueChanged(intNum);

                intValue.text = intNum.ToString();
            });
            intValue.interactable = !readOnly;
        }

        public override void UpdateCurrentValue()
        {
            var currentValue = GetValue();

            intValue.text = currentValue.ToString();
        }
    }
}