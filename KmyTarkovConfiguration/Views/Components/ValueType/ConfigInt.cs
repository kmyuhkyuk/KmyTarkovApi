﻿using System;
using KmyTarkovConfiguration.Views.Components.Base;
using TMPro;
using UnityEngine;

namespace KmyTarkovConfiguration.Views.Components.ValueType
{
    public class ConfigInt : ConfigNum<int>
    {
        [SerializeField] private TMP_InputField intValue;

        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, int defaultValue, Action<int> onValueChanged, bool hideReset, Func<int> currentValue)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset, currentValue);

            intValue.onEndEdit.AddListener(value =>
            {
                if (!int.TryParse(value, out var intNum))
                {
                    intNum = 0;
                }

                onValueChanged(intNum);

                intValue.text = intNum.ToString();
            });
            intValue.interactable = !readOnly;
        }

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
            });
            intValue.interactable = !readOnly;
        }

        public override void UpdateCurrentValue()
        {
            var currentValue = GetValue();

            intValue.text = currentValue.ToString();
        }
    }
}