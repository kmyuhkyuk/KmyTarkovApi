using System;
using EFTConfiguration.Components.Base;
using TMPro;
using UnityEngine;

namespace EFTConfiguration.Components.ValueType
{
    public class ConfigVector3 : ConfigGetValue<Vector3>
    {
        [SerializeField] private TMP_InputField x;

        [SerializeField] private TMP_InputField y;

        [SerializeField] private TMP_InputField z;

        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, Vector3 defaultValue, Action<Vector3> onValueChanged, bool hideReset,
            Func<Vector3> currentValue)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset, currentValue);

            x.onEndEdit.AddListener(value =>
            {
                var xNum = float.Parse(value);

                onValueChanged(new Vector3(xNum, float.Parse(y.text), float.Parse(z.text)));

                x.text = xNum.ToString("F2");
            });
            x.interactable = !readOnly;

            y.onEndEdit.AddListener(value =>
            {
                var yNum = float.Parse(value);

                onValueChanged(new Vector3(float.Parse(x.text), yNum, float.Parse(z.text)));

                y.text = yNum.ToString("F2");
            });
            y.interactable = !readOnly;

            z.onEndEdit.AddListener(value =>
            {
                var zNum = float.Parse(value);

                onValueChanged(new Vector3(float.Parse(x.text), float.Parse(y.text), zNum));

                z.text = zNum.ToString("F2");
            });
            z.interactable = !readOnly;
        }

        public override void UpdateCurrentValue()
        {
            var currentValue = GetValue();

            x.text = currentValue.x.ToString("F2");
            y.text = currentValue.y.ToString("F2");
            z.text = currentValue.z.ToString("F2");
        }
    }
}