using System;
using EFTConfiguration.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EFTConfiguration.Components.Base
{
    public abstract class ConfigReset : ConfigBase
    {
        private const string ResetNameKey = "Reset";

        [SerializeField] protected TMP_Text resetName;

        [SerializeField] protected Button reset;

        public void Init<T>(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, T defaultValue, Action<T> onValueChanged, bool hideReset)
        {
            Init(modName, configNameKey, descriptionNameKey, isAdvanced);

            reset.onClick.AddListener(() => onValueChanged(defaultValue));
            reset.interactable = !readOnly;

            if (hideReset)
            {
                reset.gameObject.SetActive(false);
            }
        }

        protected override void UpdateLocalized()
        {
#if !UNITY_EDITOR

            base.UpdateLocalized();

            resetName.text = LocalizedHelper.Localized(EFTConfigurationPlugin.ModName, ResetNameKey);

#endif
        }

        public abstract void UpdateCurrentValue();
    }
}