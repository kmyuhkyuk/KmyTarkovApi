using System;

namespace KmyTarkovConfiguration.Views.Components.Base
{
    public abstract class ConfigGetValue<T> : ConfigReset
    {
        protected Func<T> GetValue;

        public virtual void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, T defaultValue, Action<T> onValueChanged, bool hideReset, Func<T> currentValue)
        {
            Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset);

            GetValue = currentValue;

            UpdateCurrentValue();
        }

        public void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, object defaultValue, Action<object> onValueChanged, bool hideReset,
            Func<object> currentValue)
        {
            Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, (T)defaultValue,
                value => onValueChanged(value), hideReset, () => (T)currentValue());
        }
    }
}