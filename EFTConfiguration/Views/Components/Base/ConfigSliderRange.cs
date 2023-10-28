using System;
using UnityEngine;
using UnityEngine.UI;

namespace EFTConfiguration.Views.Components.Base
{
    public abstract class ConfigSliderRange<T> : ConfigNum<T> where T : IConvertible
    {
        [SerializeField] protected Slider slider;

        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, T defaultValue, Action<T> onValueChanged, bool hideReset, Func<T> currentValue, T min, T max)
        {
            slider.minValue = min.ToSingle(null);
            slider.maxValue = max.ToSingle(null);
            slider.interactable = !readOnly;

            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset, currentValue);
        }
    }
}