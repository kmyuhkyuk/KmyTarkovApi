using System;
using EFTConfiguration.Views.Components.Base;
using TMPro;
using UnityEngine;

namespace EFTConfiguration.Views.Components.ValueType
{
    public class ConfigFloat : ConfigNum<float>
    {
        [SerializeField] private TMP_InputField floatValue;

        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, float defaultValue, Action<float> onValueChanged, bool hideReset, Func<float> currentValue)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset, currentValue);

            floatValue.onEndEdit.AddListener(value =>
            {
                if (!float.TryParse(value, out var floatNum))
                {
                    floatNum = 0;
                }

                onValueChanged(floatNum);

                floatValue.text = floatNum.ToString("F2");
            });
            floatValue.interactable = !readOnly;
        }

        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, float defaultValue, Action<float> onValueChanged, bool hideReset, Func<float> currentValue,
            float min, float max)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset, currentValue, min, max);

            floatValue.onEndEdit.AddListener(value =>
            {
                if (!float.TryParse(value, out var floatNum))
                {
                    floatNum = 0;
                }

                floatNum = Mathf.Clamp(floatNum, min, max);

                onValueChanged(floatNum);

                floatValue.text = floatNum.ToString("F2");
            });
            floatValue.interactable = !readOnly;
        }

        public override void UpdateCurrentValue()
        {
            var currentValue = GetValue();

            floatValue.text = currentValue.ToString("F2");
        }
    }
}