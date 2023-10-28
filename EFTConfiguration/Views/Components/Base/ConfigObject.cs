using System;

namespace EFTConfiguration.Views.Components.Base
{
    public abstract class ConfigObject : ConfigGetValue<object>
    {
        public virtual void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, object defaultValue, Action<object> onValueChanged, bool hideReset,
            Func<object> currentValue,
            Type type)
        {
            Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset, currentValue);
        }
    }
}