using System;
using System.Collections;
using System.Linq;
using BepInEx.Configuration;
using KmyTarkovConfiguration.Helpers;
using KmyTarkovConfiguration.Views.Components.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if !UNITY_EDITOR
using KmyTarkovConfiguration.Models;
#endif

namespace KmyTarkovConfiguration.Views.Components.ValueType
{
    public class ConfigKeyboardShortcut : ConfigGetValue<KeyboardShortcut>
    {
        private const string ClearNameKey = "Clear";

        private const string PressNameKey = "Press";

        [SerializeField] private Button bind;

        [SerializeField] private TMP_Text bindName;

        [SerializeField] private Button clear;

        [SerializeField] private TMP_Text clearName;

        private static readonly KeyCode[] AllKeyCodes =
            KeyboardShortcut.AllKeyCodes.Where(x => x != KeyCode.None && x != KeyCode.Mouse0).ToArray();

        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced,
            bool readOnly, KeyboardShortcut defaultValue, Action<KeyboardShortcut> onValueChanged, bool hideReset,
            Func<KeyboardShortcut> currentValue)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged,
                hideReset, currentValue);

            bind.onClick.AddListener(() => StartCoroutine(Bind(onValueChanged)));
            bind.interactable = !readOnly;

            clear.onClick.AddListener(() =>
            {
                onValueChanged(KeyboardShortcut.Empty);

                bindName.text = KeyboardShortcut.Empty.ToString();
            });
            clear.interactable = !readOnly;
        }

        private IEnumerator Bind(Action<KeyboardShortcut> onValueChanged)
        {
            var bindKeyCode = KeyCode.None;

#if !UNITY_EDITOR

            bindName.text = LocalizedHelper.Localized(EFTConfigurationModel.Instance.ModName, PressNameKey);

#endif

            while (bindKeyCode == KeyCode.None)
            {
                foreach (var keyCode in AllKeyCodes)
                {
                    if (Input.GetKeyUp(keyCode))
                    {
                        bindKeyCode = keyCode;
                    }
                }

                yield return new WaitForEndOfFrame();
            }

            onValueChanged(new KeyboardShortcut(bindKeyCode));

            bindName.text = bindKeyCode.ToString();
        }

        public override void UpdateCurrentValue()
        {
            var currentValue = GetValue();

            bindName.text = currentValue.ToString();
        }

        protected override void UpdateLocalized()
        {
            base.UpdateLocalized();

#if !UNITY_EDITOR

            clearName.text = LocalizedHelper.Localized(EFTConfigurationModel.Instance.ModName, ClearNameKey);

#endif
        }
    }
}