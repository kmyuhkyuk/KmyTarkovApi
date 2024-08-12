using System;
using EFTConfiguration.Views.Components.Base;
using TMPro;
using UnityEngine;

namespace EFTConfiguration.Views.Components.ValueType
{
    public class ConfigIntSlider : ConfigSliderRange<int>
    {
        [SerializeField] private TMP_InputField intValue;

        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, int defaultValue, Action<int> onValueChanged, bool hideReset, Func<int> currentValue,
            int min,
            int max)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset, currentValue, min, max);

            intValue.onEndEdit.AddListener(value =>
            {
                if (!int.TryParse(value, out var intNum))
                {
                    intNum = 0;
                }

                intNum = Mathf.Clamp(intNum, min, max);

                onValueChanged(intNum);

                intValue.text = intNum.ToString();
                slider.value = intNum;
            });
            intValue.interactable = !readOnly;

            slider.onValueChanged.AddListener(value =>
            {
                var intNum = (int)value;

                onValueChanged(intNum);

                intValue.text = intNum.ToString();
                slider.value = intNum;
            });
        }

        public override void UpdateCurrentValue()
        {
            var currentValue = GetValue();

            intValue.text = currentValue.ToString();
            slider.value = currentValue;
        }
    }
}