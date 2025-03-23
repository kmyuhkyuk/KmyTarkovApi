using System;
using KmyTarkovConfiguration.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if !UNITY_EDITOR
using KmyTarkovConfiguration.Models;
#endif

namespace KmyTarkovConfiguration.Views.Components.Base
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

            resetName.text = LocalizedHelper.Localized(EFTConfigurationModel.Instance.ModName, ResetNameKey);

#endif
        }

        public abstract void UpdateCurrentValue();
    }
}