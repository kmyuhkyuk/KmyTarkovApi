using System;

namespace EFTConfiguration.Components.Base
{
    public abstract class ConfigNum<T> : ConfigGetValue<T> where T : IConvertible
    {
        public virtual void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced, bool readOnly, T defaultValue, Action<T> onValueChanged, bool hideRest, Func<T> currentValue, T min, T max)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged, hideRest, currentValue);
        }

        public void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced, bool readOnly, object defaultValue, Action<object> onValueChanged, bool hideRest, Func<object> currentValue, T min, T max)
        {
            Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, (T)defaultValue, value => onValueChanged(value), hideRest, () => (T)currentValue(), min, max);
        }
    }
}
