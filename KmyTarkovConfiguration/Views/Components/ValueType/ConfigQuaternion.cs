﻿using System;
using KmyTarkovConfiguration.Views.Components.Base;
using TMPro;
using UnityEngine;

namespace KmyTarkovConfiguration.Views.Components.ValueType
{
    public class ConfigQuaternion : ConfigGetValue<Quaternion>
    {
        [SerializeField] private TMP_InputField x;

        [SerializeField] private TMP_InputField y;

        [SerializeField] private TMP_InputField z;

        [SerializeField] private TMP_InputField w;

        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, Quaternion defaultValue, Action<Quaternion> onValueChanged, bool hideReset,
            Func<Quaternion> currentValue)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset, currentValue);

            x.onEndEdit.AddListener(value =>
            {
                if (!float.TryParse(value, out var xNum))
                {
                    xNum = 0;
                }

                onValueChanged(new Quaternion(xNum, float.Parse(y.text), float.Parse(z.text), float.Parse(w.text)));

                x.text = xNum.ToString("F2");
            });
            x.interactable = !readOnly;

            y.onEndEdit.AddListener(value =>
            {
                if (!float.TryParse(value, out var yNum))
                {
                    yNum = 0;
                }

                onValueChanged(new Quaternion(float.Parse(x.text), yNum, float.Parse(z.text), float.Parse(w.text)));

                y.text = yNum.ToString("F2");
            });
            y.interactable = !readOnly;

            z.onEndEdit.AddListener(value =>
            {
                if (!float.TryParse(value, out var zNum))
                {
                    zNum = 0;
                }

                onValueChanged(new Quaternion(float.Parse(x.text), float.Parse(y.text), zNum, float.Parse(w.text)));

                z.text = zNum.ToString("F2");
            });
            z.interactable = !readOnly;

            w.onEndEdit.AddListener(value =>
            {
                if (!float.TryParse(value, out var wNum))
                {
                    wNum = 0;
                }

                onValueChanged(new Quaternion(float.Parse(x.text), float.Parse(y.text), float.Parse(z.text), wNum));

                w.text = wNum.ToString("F2");
            });
            w.interactable = !readOnly;
        }

        public override void UpdateCurrentValue()
        {
            var currentValue = GetValue();

            x.text = currentValue.x.ToString("F2");
            y.text = currentValue.y.ToString("F2");
            z.text = currentValue.z.ToString("F2");
            w.text = currentValue.w.ToString("F2");
        }
    }
}