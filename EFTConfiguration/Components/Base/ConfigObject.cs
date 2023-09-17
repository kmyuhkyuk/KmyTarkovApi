using System;

namespace EFTConfiguration.Components.Base
{
    public abstract class ConfigObject : ConfigGetValue<object>
    {
        protected Func<object, string> CustomToString;

        protected bool UseCustomGetValue;

        public virtual void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, object defaultValue, Action<object> onValueChanged, bool hideReset,
            Func<object> currentValue,
            Type type)
        {
            Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset, currentValue);
        }

        public virtual void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, object defaultValue, Action<object> onValueChanged, bool hideReset,
            Func<object> currentValue,
            Type type, Func<object, string> customToString, Func<string, object> customToObject)
        {
            UseCustomGetValue = true;
            CustomToString = customToString;

            Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset, currentValue);
        }
    }
}