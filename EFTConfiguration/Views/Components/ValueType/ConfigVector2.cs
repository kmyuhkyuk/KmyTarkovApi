using System;
using EFTConfiguration.Views.Components.Base;
using TMPro;
using UnityEngine;

namespace EFTConfiguration.Views.Components.ValueType
{
    public class ConfigVector2 : ConfigGetValue<Vector2>
    {
        [SerializeField] private TMP_InputField x;

        [SerializeField] private TMP_InputField y;

        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, Vector2 defaultValue, Action<Vector2> onValueChanged, bool hideReset,
            Func<Vector2> currentValue)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset, currentValue);

            x.onEndEdit.AddListener(value =>
            {
                if (!float.TryParse(value, out var xNum))
                {
                    xNum = 0;
                }

                ;

                onValueChanged(new Vector2(xNum, float.Parse(y.text)));

                x.text = xNum.ToString("F2");
            });
            x.interactable = !readOnly;

            y.onEndEdit.AddListener(value =>
            {
                if (!float.TryParse(value, out var yNum))
                {
                    yNum = 0;
                }

                onValueChanged(new Vector2(float.Parse(x.text), yNum));

                y.text = yNum.ToString("F2");
            });
            y.interactable = !readOnly;
        }

        public override void UpdateCurrentValue()
        {
            var currentValue = GetValue();

            x.text = currentValue.x.ToString("F2");
            y.text = currentValue.y.ToString("F2");
        }
    }
}