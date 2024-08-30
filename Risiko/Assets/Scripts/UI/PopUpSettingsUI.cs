using UnityEngine;

namespace UI
{
    public class PopUpSettingsUI : MonoBehaviour
    {
        public void Open() {
            gameObject.SetActive(true);
        }
        public void Close() {
            gameObject.SetActive(false);
        }
    }
}
