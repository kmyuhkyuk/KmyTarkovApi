using UnityEngine;
#if !UNITY_EDITOR
using EFTConfiguration.Models;
#endif

namespace EFTConfiguration.Views
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

#if !UNITY_EDITOR

            EFTConfigurationModel.Instance.Unlock = true;

#endif
        }

        private void OnDisable()
        {
#if !UNITY_EDITOR

            EFTConfigurationModel.Instance.Unlock = false;

#endif

            Cursor.lockState = _oldLockState;
            Cursor.visible = _oldVisible;
        }
    }
}