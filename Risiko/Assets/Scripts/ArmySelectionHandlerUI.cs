using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PolygonCollider2D))] 
public class ArmySelectionHandlerUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private SpriteRenderer sprite;
    public Color32 oldColor;
    public Color32 hoverColor;
    public Color32 startColor;
    private bool selected = false;
    
    void Awake() {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = startColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!selected) {
            oldColor = sprite.color;
            sprite.color = hoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!selected) {
            sprite.color = oldColor;
        }
    }
    /*void OnMouseEnter()  {
        if (!selected) {
            oldColor = sprite.color;
            sprite.color = hoverColor;
        }
    }

    void OnMouseExit() {
        if (!selected) {
            sprite.color = oldColor;
        }
    }*/

    void OnMouseDown()
    {
        if (!selected)
        {
            Select();
        }
    }
    
    public void Select() {
        selected = true;
        /// CARICARE DATI STATO
        sprite.color = hoverColor;
    }

    public void Deselect() {
        selected = false;
        sprite.color = startColor;
    }
}
