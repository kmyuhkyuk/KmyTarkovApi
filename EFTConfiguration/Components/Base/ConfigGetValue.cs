using System;

namespace EFTConfiguration.Components.Base
{
    public abstract class ConfigGetValue<T> : ConfigRest
    {
        protected Func<T> GetValue;

        public virtual void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, T defaultValue, Action<T> onValueChanged, bool hideRest, Func<T> currentValue)
        {
            Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideRest);

            GetValue = currentValue;

            UpdateCurrentValue();
        }

        public void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, object defaultValue, Action<object> onValueChanged, bool hideRest, Func<object> currentValue)
        {
            Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, (T)defaultValue,
                value => onValueChanged(value), hideRest, () => (T)currentValue());
        }
    }
}