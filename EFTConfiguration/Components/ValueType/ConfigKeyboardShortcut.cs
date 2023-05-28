using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BepInEx.Configuration;
using EFTConfiguration.Components.Base;
using EFTConfiguration.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EFTConfiguration.Components.ValueType
{
    public class ConfigKeyboardShortcut : ConfigGetValue<KeyboardShortcut>
    {
        private const string ClearNameKey = "Clear";

        private const string PressNameKey = "Press";

        [SerializeField] 
        private Button bind;

        [SerializeField]
        private TMP_Text bindName;

        [SerializeField]
        private Button clear;

        [SerializeField]
        private TMP_Text clearName;

        private static readonly KeyCode[] AllKeyCodes = KeyboardShortcut.AllKeyCodes.Where(x => x != KeyCode.None && x != KeyCode.Mouse0).ToArray();

        private CancellationTokenSource _cancellationTokenSource;

        public override void Init(string modName, string configNameKey, string descriptionNameKey, bool isAdvanced, bool readOnly, KeyboardShortcut defaultValue, Action<KeyboardShortcut> onValueChanged, bool hideRest, Func<KeyboardShortcut> currentValue)
        {
            base.Init(modName, configNameKey, descriptionNameKey, isAdvanced, readOnly, defaultValue, onValueChanged, hideRest, currentValue);

            bind.onClick.AddListener(() =>
            {
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                    _cancellationTokenSource = null;
                }

                _cancellationTokenSource = new CancellationTokenSource();

                Bind(onValueChanged, _cancellationTokenSource.Token);
            });
            bind.interactable = !readOnly;

            clear.onClick.AddListener(() =>
            {
                onValueChanged(KeyboardShortcut.Empty);

                bindName.text = KeyboardShortcut.Empty.ToString();
            });
            clear.interactable = !readOnly;
        }

        private async void Bind(Action<KeyboardShortcut> onValueChanged, CancellationToken token)
        {
            var bindKeyCode = KeyCode.None;

            bindName.text = CustomLocalizedHelper.Localized(ModName, PressNameKey);

            while (bindKeyCode == KeyCode.None)
            {
                if (token.IsCancellationRequested)
                    return;

                foreach (var keyCode in AllKeyCodes)
                {
                    if (Input.GetKeyUp(keyCode))
                    {
                        bindKeyCode = keyCode;
                    }
                }

                await Task.Yield();
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

            clearName.text = CustomLocalizedHelper.Localized(ModName, ClearNameKey);
        }

        private void OnDisable()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }
    }
}
