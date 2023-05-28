using System;
using EFTConfiguration.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EFTConfiguration.Components.Base
{
    public abstract class ConfigRest : ConfigBase
    {
        private const string RestNameKey = "Rest";

        [SerializeField] protected TMP_Text restName;

        [SerializeField] protected Button rest;

        public void Init<T>(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, T defaultValue, Action<T> onValueChanged, bool hideRest)
        {
            Init(modName, configNameKey, descriptionNameKey, isAdvanced);

            rest.onClick.AddListener(() => onValueChanged(defaultValue));
            rest.interactable = !readOnly;

            if (hideRest)
            {
                rest.gameObject.SetActive(false);
            }
        }

        protected override void UpdateLocalized()
        {
#if !UNITY_EDITOR
            base.UpdateLocalized();

            restName.text = CustomLocalizedHelper.Localized(EFTConfigurationPlugin.ModName, RestNameKey);
#endif
        }

        public abstract void UpdateCurrentValue();
    }
}