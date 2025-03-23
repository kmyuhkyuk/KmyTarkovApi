using System;

namespace KmyTarkovConfiguration.Views.Components.Base
{
    public abstract class ConfigObjectCustom : ConfigGetValue<object>
    {
        protected Func<object, string> CustomToString;

        public virtual void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, object defaultValue, Action<object> onValueChanged, bool hideReset,
            Func<object> currentValue, Func<object, string> customToString, Func<string, object> customToObject)
        {
            CustomToString = customToString;

            Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset, currentValue);
        }
    }
}