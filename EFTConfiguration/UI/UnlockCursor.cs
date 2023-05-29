using UnityEngine;

namespace EFTConfiguration.UI
{
    public class UnlockCursor : MonoBehaviour
    {
        private CursorLockMode _oldLockState;
        private bool _oldVisible;

        private void OnEnable()
        {
            _oldLockState = Cursor.lockState;
            _oldVisible = Cursor.visible;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            EFTConfigurationPlugin.Unlock = true;
        }

        private void OnDisable()
        {
            EFTConfigurationPlugin.Unlock = false;

            Cursor.lockState = _oldLockState;
            Cursor.visible = _oldVisible;
        }
    }
}