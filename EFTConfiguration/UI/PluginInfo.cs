using System;
using EFTConfiguration.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EFTConfiguration.UI
{
    public class PluginInfo : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public bool IsOn
        {
            get => toggle.isOn;
            set => toggle.isOn = value;
        }

        public bool isCore;

        public bool fistPluginInfo;

        public bool hasModUrl;

        public string modName;

        public string modUrl;

        public ToggleGroup toggleGroup;

        private Version _modVersion;

        [SerializeField] private Image background;

        [SerializeField] private Image icon;

        [SerializeField] private TMP_Text pluginName;

        [SerializeField] private TMP_Text version;

        [SerializeField] private TMP_Text downloads;

        [SerializeField] private Button mod;

        [SerializeField] private Button modDownload;

        [SerializeField] private Toggle toggle;

        private bool _isPointer;

        private void Start()
        {
            toggle.group = toggleGroup;

            toggle.onValueChanged.AddListener(value =>
            {
                if (value)
                {
                    Pressed();
                    EFTConfiguration.SwitchPluginInfo(this);
                }
                else
                {
                    Disabled();
                }
            });

            if (hasModUrl)
            {
                mod.onClick.AddListener(() => Application.OpenURL(modUrl));
            }

            if (fistPluginInfo)
            {
                IsOn = true;
            }

            UpdateLocalized();

            CustomLocalizedHelper.LanguageChange += UpdateLocalized;
        }

        private void UpdateLocalized()
        {
            pluginName.text = CustomLocalizedHelper.Localized(modName);
        }

        private void Pressed()
        {
            background.color = new Color(0.772549f, 0.7647059f, 0.6980392f, 0.7333333f);
            pluginName.color = Color.black;
            downloads.color = Color.black;
            modDownload.colors = new ColorBlock
            {
                normalColor = Color.black,
                highlightedColor = new Color(0.9607843f, 0.9607843f, 0.9607843f),
                pressedColor = new Color(0.7843137f, 0.7843137f, 0.7843137f),
                selectedColor = new Color(0.9607843f, 0.9607843f, 0.9607843f),
                disabledColor = new Color(0.7843137f, 0.7843137f, 0.7843137f, 0.5019608f),
                colorMultiplier = 1f,
                fadeDuration = 0.1f
            };
        }

        private void Selected()
        {
            background.color = new Color(0.8897059f, 0.8835083f, 0.8308283f, 0.172f);
        }

        private void Disabled()
        {
            background.color = Color.black;
            pluginName.color = Color.white;
            downloads.color = Color.white;
            modDownload.colors = new ColorBlock
            {
                normalColor = Color.white,
                highlightedColor = new Color(0.9607843f, 0.9607843f, 0.9607843f),
                pressedColor = new Color(0.7843137f, 0.7843137f, 0.7843137f),
                selectedColor = new Color(0.9607843f, 0.9607843f, 0.9607843f),
                disabledColor = new Color(0.7843137f, 0.7843137f, 0.7843137f, 0.5019608f),
                colorMultiplier = 1f,
                fadeDuration = 0.1f
            };
        }

        public void Init(Version modVersion)
        {
            _modVersion = modVersion;

            version.text = modVersion.ToString();
        }

        public void BindWeb(Sprite modIcon, string modDownloadUrl, int modDownloads, Version modVersion)
        {
            icon.sprite = modIcon;
            modDownload.onClick.AddListener(() => Application.OpenURL(modDownloadUrl));
            downloads.text = modDownloads.ToString();

            if (modVersion > _modVersion)
            {
                toggle.onValueChanged.AddListener(value =>
                {
                    version.text = string.Concat("<color=yellow>", _modVersion, "</color>",
                        value ? "<color=black>" : "<color=white>", " < ", "</color>", "<color=green>", modVersion,
                        "</color>");
                });
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isPointer && !IsOn)
            {
                IsOn = true;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isPointer = true;

            if (!IsOn)
            {
                Selected();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isPointer = false;

            if (!IsOn)
            {
                Disabled();
            }
        }
    }
}