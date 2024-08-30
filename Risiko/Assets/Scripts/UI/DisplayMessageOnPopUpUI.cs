using TMPro;
using UnityEngine;

namespace UI
{
    public class DisplayMessageOnPopUpUI : MonoBehaviour {
    
        [SerializeField] private TMP_Text errorText;

        public void SetErrorText(string text) {
            errorText.text = text;
        }
    }
}
