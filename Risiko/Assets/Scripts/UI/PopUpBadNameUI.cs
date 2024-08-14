using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpBadNameUI : MonoBehaviour {
    
    [SerializeField] private TMP_Text errorText;

    public void SetErrorText(string text) {
        errorText.text = text;
    }
}
