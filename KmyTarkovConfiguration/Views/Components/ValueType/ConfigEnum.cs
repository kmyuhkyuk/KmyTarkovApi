﻿using System;
using KmyTarkovConfiguration.Views.Components.Base;

namespace KmyTarkovConfiguration.Views.Components.ValueType
{
    public class ConfigEnum : ConfigObjectDropdownList
    {
        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, object defaultValue, Action<object> onValueChanged, bool hideReset,
            Func<object> currentValue,
            Array values)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset, currentValue, values);

            dropdown.onValueChanged.AddListener(value =>
            {
                var enumValue = values.GetValue(value);

                onValueChanged(enumValue);
            });
        }

        public override void UpdateCurrentValue()
        {
            var currentValue = GetValue().ToString();

            dropdown.value = dropdown.options.FindIndex(x => x.text == currentValue);
        }
    }
}