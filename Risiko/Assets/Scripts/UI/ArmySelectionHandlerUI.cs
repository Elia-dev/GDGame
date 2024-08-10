using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ArmySelectionHandlerUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject frame;
    [SerializeField] private Color32 armyColor;

    public Color32 ArmyColor {
        get => armyColor;
    }

    private bool selected = false;
    
    void Awake() {
        //sprite = GetComponent<SpriteRenderer>();
        //sprite.color = startColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Entra OnPoint?!");
        if (!selected) {
            /*oldColor = sprite.color;
            sprite.color = hoverColor;*/
            frame.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Non può entrare!");
        if (!selected) {
            //sprite.color = oldColor;
            frame.SetActive(false);
        }
    }

   public void OnPointerClick(PointerEventData eventData)
    {
        Select();
        //selected = true;
        //Debug.Log("Select");
        /// CARICARE DATI STATO
        
        //sprite.color = hoverColor;
        //frame.SetActive(true);
    }
    
    public void Select() {
        selected = true;
        //Debug.Log("Select");
        /// CARICARE DATI STATO
        //sprite.color = hoverColor;
        frame.SetActive(true);
    }

    public void Deselect() {
        //Debug.Log("Deselect");
        selected = false;
        frame.SetActive(false);
        //sprite.color = startColor;
    }
}
