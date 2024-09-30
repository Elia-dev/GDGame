using UnityEngine;

namespace UI
{
    public class PopUpSettingsUI : MonoBehaviour
    {
        public void Close() {
            gameObject.SetActive(false);
        }
    }
}
