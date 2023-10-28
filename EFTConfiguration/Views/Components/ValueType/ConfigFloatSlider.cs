using System;
using EFTConfiguration.Views.Components.Base;
using TMPro;
using UnityEngine;

namespace EFTConfiguration.Views.Components.ValueType
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
                var floatNum = Mathf.Clamp(float.Parse(value), min, max);

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