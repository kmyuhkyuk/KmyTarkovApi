using System;
using TMPro;
using UnityEngine;

namespace EFTConfiguration.Components.Base
{
    public abstract class ConfigDropdownList<T> : ConfigGetValue<T>
    {
        [SerializeField] protected TMP_Dropdown dropdown;

        protected Array Values;

        public virtual void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, T defaultValue, Action<T> onValueChanged, bool hideRest, Func<T> currentValue, T[] values)
        {
            Values = values;

            foreach (var value in values)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData(value.ToString()));
            }

            dropdown.interactable = !readOnly;

            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideRest, currentValue);
        }

        public virtual void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, object defaultValue, Action<object> onValueChanged, bool hideRest, Func<object> currentValue,
            Array values)
        {
            Values = values;

            foreach (var value in values)
            {
                dropdown.options.Add(new TMP_Dropdown.OptionData(value.ToString()));
            }

            dropdown.interactable = !readOnly;

            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideRest, currentValue);
        }

        public void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, object defaultValue, Action<object> onValueChanged, bool hideRest, Func<object> currentValue,
            T[] values)
        {
            Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, (T)defaultValue,
                value => onValueChanged(value), hideRest, () => (T)currentValue(), values);
        }
    }
}