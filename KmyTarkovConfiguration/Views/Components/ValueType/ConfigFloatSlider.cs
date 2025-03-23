using System;
using KmyTarkovConfiguration.Views.Components.Base;
using TMPro;
using UnityEngine;

namespace KmyTarkovConfiguration.Views.Components.ValueType
{
    public class ConfigFloatSlider : ConfigSliderRange<float>
    {
        [SerializeField] private TMP_InputField floatValue;

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
                slider.value = floatNum;
            });
            floatValue.interactable = !readOnly;

            slider.onValueChanged.AddListener(value =>
            {
                var floatNum = value;

                onValueChanged(floatNum);

                floatValue.text = floatNum.ToString("F2");
                slider.value = floatNum;
            });
        }

        public override void UpdateCurrentValue()
        {
            var currentValue = GetValue();

            floatValue.text = currentValue.ToString("F2");
            slider.value = currentValue;
        }
    }
}