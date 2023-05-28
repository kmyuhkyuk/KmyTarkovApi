using EFTConfiguration.Helpers;
using EFTConfiguration.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EFTConfiguration.Components.Base
{
    public class ConfigDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string modName;

        public string descriptionNameKey;

        public void OnPointerEnter(PointerEventData eventData)
        {
            Description.Instance.Enable(CustomLocalizedHelper.Localized(modName, descriptionNameKey));
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Description.Instance.Disable();
        }
    }
}