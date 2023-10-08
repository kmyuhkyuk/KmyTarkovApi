using EFTConfiguration.Helpers;
using TMPro;
using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global

namespace EFTConfiguration.Components.Base
{
    public abstract class ConfigBase : MonoBehaviour
    {
        public bool IsAdvanced { get; private set; }

        public string LocalizedName => configName.text;

        protected string ModName;

        protected string ConfigNameKey;

        [SerializeField] protected TMP_Text configName;

        [SerializeField] protected ConfigDescription description;

        protected void Start()
        {
            LocalizedHelper.LanguageChange += UpdateLocalized;
        }

        public void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced)
        {
            ModName = modName;
            ConfigNameKey = configNameKey;
            IsAdvanced = isAdvanced;

            if (!string.IsNullOrEmpty(descriptionNameKey))
            {
                description.modName = modName;
                description.descriptionNameKey = descriptionNameKey;
            }
            else
            {
                description.enabled = false;
            }

            if (isAdvanced)
            {
                configName.color = Color.yellow;
            }

            UpdateLocalized();
        }

        protected virtual void UpdateLocalized()
        {
            configName.text = LocalizedHelper.Localized(ModName, ConfigNameKey);
        }

        protected virtual void OnDestroy()
        {
            LocalizedHelper.LanguageChange -= UpdateLocalized;
        }
    }
}