using EFTConfiguration.Components.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EFTConfiguration.Components.ClassType
{
    public class ConfigStringAction : ConfigAction, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private Image background;

        private bool _isPointer;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isPointer)
            {
                ButtonAction();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isPointer = true;

            background.color = new Color(0.772549f, 0.7647059f, 0.6980392f, 0.7333333f);
            configName.color = Color.black;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isPointer = false;

            background.color = Color.black;
            configName.color = IsAdvanced ? Color.yellow : Color.white;
        }
    }
}
