using UnityEngine;

namespace EFTConfiguration.UI
{
    public class UnlockCursor : MonoBehaviour
    {
        private void Update()
        {
            Unlock();
        }

        private void LastUpdate()
        {
            Unlock();
        }

        private static void Unlock()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
