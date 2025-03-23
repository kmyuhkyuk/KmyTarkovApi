using TMPro;
using UnityEngine;
#if !UNITY_EDITOR
using KmyTarkovConfiguration.Models;
#endif

namespace KmyTarkovConfiguration.Views
{
    public class Description : MonoBehaviour
    {
        [SerializeField] private TMP_Text description;

        private void Update()
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
#if !UNITY_EDITOR

            transform.position = (Vector2)Input.mousePosition +
                                 SettingsModel.Instance.KeyDescriptionPositionOffset.Value;

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