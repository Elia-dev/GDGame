using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PolygonCollider2D))] 
public class ArmySelectionHandlerUI : MonoBehaviour, IPointerClickHandler//, IPointerEnterHandler, IPointerExitHandler
{
    private SpriteRenderer sprite;
    /*public Color32 oldColor;
    public Color32 hoverColor;
    public Color32 startColor;*/
    [SerializeField] private GameObject frame;
    private bool selected = false;
    
    void Awake() {
        sprite = GetComponent<SpriteRenderer>();
        //sprite.color = startColor;
    }
    /*
    void OnMouseEnter()  {
        Debug.Log("Entra?!");
        if (!selected) {
            oldColor = sprite.color;
            sprite.color = hoverColor;
        }
    }

    void OnMouseExit() {
        Debug.Log("Non può entrare!");
        if (!selected) {
            sprite.color = oldColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Entra?!");
        if (!selected) {
            oldColor = sprite.color;
            sprite.color = hoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Non può entrare!");
        if (!selected) {
            sprite.color = oldColor;
        }
    }*/

    public void OnPointerClick(PointerEventData eventData)
    {
        selected = true;
        Debug.Log("Select");
        /// CARICARE DATI STATO
        //sprite.color = hoverColor;
        frame.SetActive(true);
    }
    void OnMouseDown()
    {
        if (!selected)
        {
            Select();
        }
    }
    
    public void Select() {
        selected = true;
        Debug.Log("Select");
        /// CARICARE DATI STATO
        //sprite.color = hoverColor;
        frame.SetActive(true);
    }

    public void Deselect() {
        Debug.Log("Deselect");
        selected = false;
        frame.SetActive(false);
        //sprite.color = startColor;
    }
}
