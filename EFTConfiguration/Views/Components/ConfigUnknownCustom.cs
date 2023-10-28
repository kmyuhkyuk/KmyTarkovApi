using System;
using EFTConfiguration.Views.Components.Base;
using TMPro;
using UnityEngine;

namespace EFTConfiguration.Views.Components
{
    public class ConfigUnknownCustom : ConfigObjectCustom
    {
        [SerializeField] private TMP_InputField unknownCustomValue;

        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, object defaultValue, Action<object> onValueChanged, bool hideReset,
            Func<object> currentValue, Func<object, string> customToString, Func<string, object> customToObject)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset, currentValue, customToString, customToObject);

            unknownCustomValue.onEndEdit.AddListener(value =>
            {
                var objectValue = customToObject(value);

                onValueChanged(objectValue);
            });
            unknownCustomValue.interactable = !readOnly;
        }

        public override void UpdateCurrentValue()
        {
            var currentValue = GetValue();

            unknownCustomValue.text = CustomToString(currentValue);
        }
    }
}