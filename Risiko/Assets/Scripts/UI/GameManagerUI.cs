using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManagerUI : MonoBehaviour {
    [SerializeField] private TMP_Text playerName;
    // Start is called before the first frame update
    void Start() {
        playerName.text = Player.Instance.Name;
        //Turno del player
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
