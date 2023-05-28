using System;
using EFTConfiguration.Components.Base;
using TMPro;
using UnityEngine;

namespace EFTConfiguration.Components.ValueType
{
    public class ConfigFloat : ConfigNum<float>
    {
        [SerializeField]
        private TMP_InputField floatValue;

        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced, bool readOnly, float defaultValue, Action<float> onValueChanged, bool hideRest, Func<float> currentValue)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged, hideRest, currentValue);

            floatValue.onEndEdit.AddListener(value =>
            {
                var floatNum = float.Parse(value);

                onValueChanged(floatNum);

                floatValue.text = floatNum.ToString("F2");
            });
            floatValue.interactable = !readOnly;
        }

        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced, bool readOnly, float defaultValue, Action<float> onValueChanged, bool hideRest, Func<float> currentValue, float min, float max)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged, hideRest, currentValue, min, max);

            floatValue.onEndEdit.AddListener(value =>
            {
                var floatNum = Mathf.Clamp(float.Parse(value), min, max);

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
