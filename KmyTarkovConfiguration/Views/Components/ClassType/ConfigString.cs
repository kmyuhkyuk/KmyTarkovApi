using System;
using KmyTarkovConfiguration.Views.Components.Base;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace KmyTarkovConfiguration.Views.Components.ClassType
{
    public class ConfigString : ConfigGetValue<string>
    {
        [SerializeField] private TMP_InputField stringValue;

        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, string defaultValue, Action<string> onValueChanged, bool hideReset,
            Func<string> currentValue)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset, currentValue);

            stringValue.onEndEdit.AddListener(new UnityAction<string>(onValueChanged));
            stringValue.readOnly = readOnly;
        }

        public override void UpdateCurrentValue()
        {
            var currentValue = GetValue();

            stringValue.text = currentValue;
        }
    }
}