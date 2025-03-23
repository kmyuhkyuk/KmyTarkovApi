using System;
using KmyTarkovConfiguration.Views.Components.Base;

namespace KmyTarkovConfiguration.Views.Components.ClassType
{
    public class ConfigStringDropdown : ConfigDropdownList<string>
    {
        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, string defaultValue, Action<string> onValueChanged, bool hideReset,
            Func<string> currentValue,
            string[] values)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset, currentValue, values);

            dropdown.onValueChanged.AddListener(value =>
            {
                var stringValue = values[value];

                onValueChanged(stringValue);
            });
        }

        public override void UpdateCurrentValue()
        {
            var currentValue = GetValue();

            var options = dropdown.options;

            for (var i = 0; i < options.Count; i++)
            {
                var option = options[i];

                if (option.text != currentValue)
                    continue;

                dropdown.value = i;

                break;
            }
        }
    }
}