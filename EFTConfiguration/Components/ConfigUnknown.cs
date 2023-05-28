using System;
using EFTConfiguration.Components.Base;
using TMPro;
using UnityEngine;

namespace EFTConfiguration.Components
{
    public class ConfigUnknown : ConfigObject
    {
        [SerializeField] 
        private TMP_InputField unknownValue;

        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced, bool readOnly, object defaultValue, Action<object> onValueChanged, bool hideRest, Func<object> currentValue, Type type)
        { 
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged, hideRest, currentValue, type);

            unknownValue.onEndEdit.AddListener(value =>
            {
                var objectValue = Convert.ChangeType(value, type);

                onValueChanged(objectValue);
            });
            unknownValue.interactable = !readOnly;
        }

        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced, bool readOnly, object defaultValue, Action<object> onValueChanged, bool hideRest, Func<object> currentValue, Type type, Func<object, string> customToString, Func<string, object> customToObject)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged, hideRest, currentValue, type, customToString, customToObject);

            unknownValue.onEndEdit.AddListener(value =>
            {
                var objectValue = customToObject(value);

                onValueChanged(objectValue);
            });
            unknownValue.interactable = !readOnly;
        }

        public override void UpdateCurrentValue()
        {
            var currentValue = GetValue();

            unknownValue.text = UseCustomGetValue ? CustomToString(currentValue) : currentValue.ToString();
        }
    }
}
