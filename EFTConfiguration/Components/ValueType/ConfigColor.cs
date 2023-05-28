using System;
using EFTConfiguration.Components.Base;
using UnityEngine;
using UnityEngine.UI;

namespace EFTConfiguration.Components.ValueType
{
    public class ConfigColor : ConfigGetValue<Color>
    {
        [SerializeField]
        private Slider r;

        [SerializeField]
        private Slider g;

        [SerializeField]
        private Slider b;

        [SerializeField]
        private Slider a;

        [SerializeField] 
        private Image colorImage;

        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced, bool readOnly, Color defaultValue, Action<Color> onValueChanged, bool hideRest, Func<Color> currentValue)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged, hideRest, currentValue);

            r.onValueChanged.AddListener(value =>
            {
                var color = new Color(value, g.value, b.value, a.value);

                onValueChanged(color);

                colorImage.color = color;
            });
            r.interactable = !readOnly;

            g.onValueChanged.AddListener(value =>
            {
                var color = new Color(r.value, value, b.value, a.value);

                onValueChanged(color);

                colorImage.color = color;
            });
            g.interactable = !readOnly;

            b.onValueChanged.AddListener(value =>
            {
                var color = new Color(r.value, g.value, value, a.value);

                onValueChanged(color);

                colorImage.color = color;
            });
            b.interactable = !readOnly;

            a.onValueChanged.AddListener(value =>
            {
                var color = new Color(r.value, g.value, b.value, value);

                onValueChanged(color);

                colorImage.color = color;
            });
            a.interactable = !readOnly;
        }

        public override void UpdateCurrentValue()
        {
            var currentValue = GetValue();

            r.value = currentValue.r;
            g.value = currentValue.g;
            b.value = currentValue.b;
            a.value = currentValue.a;

            colorImage.color = currentValue;
        }
    }
}
