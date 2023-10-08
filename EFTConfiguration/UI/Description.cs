using TMPro;
using UnityEngine;

namespace EFTConfiguration.UI
{
    public class Description : MonoBehaviour
    {
        [SerializeField] private TMP_Text description;

        public static Description Instance { get; private set; }

        private Description()
        {
            Instance = this;
        }

        private void Update()
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
#if !UNITY_EDITOR

            transform.position = (Vector2)Input.mousePosition +
                                 EFTConfigurationPlugin.SetData.KeyDescriptionPositionOffset.Value;

#endif
        }

        public void Enable(string text)
        {
            description.text = text;
            UpdatePosition();
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            gameObject.SetActive(false);
        }
    }
}