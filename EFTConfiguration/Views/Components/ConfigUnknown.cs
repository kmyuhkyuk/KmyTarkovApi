using System;
using EFTConfiguration.Views.Components.Base;
using TMPro;
using UnityEngine;

namespace EFTConfiguration.Views.Components
{
    public class ConfigUnknown : ConfigObject
    {
        [SerializeField] private TMP_InputField unknownValue;

        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, object defaultValue, Action<object> onValueChanged, bool hideReset,
            Func<object> currentValue,
            Type type)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset, currentValue, type);

            unknownValue.onEndEdit.AddListener(value =>
            {
                var objectValue = Convert.ChangeType(value, type);

                onValueChanged(objectValue);
            });
            unknownValue.interactable = !readOnly;
        }

        public override void UpdateCurrentValue()
        {
            var currentValue = GetValue();

            unknownValue.text = currentValue.ToString();
        }
    }
}