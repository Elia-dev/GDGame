using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))] 

public class StateHandlerUI : MonoBehaviour  {
    private SpriteRenderer sprite;
    public Color32 oldColor;
    public Color32 hoverColor;
    public Color32 startColor;
    public Color32 userColor;
    private bool selected = false;
    
    void Awake() {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = startColor;
    }

    void OnMouseEnter()  {
        if (!selected) {
            oldColor = sprite.color;
            sprite.color = hoverColor;
        }
    }

    void OnMouseExit() {
        if (!selected) {
            sprite.color = oldColor;
        }
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
        /// CARICARE DATI STATO
        sprite.color = userColor;
    }

    public void Deselect() {
        selected = false;
        sprite.color = startColor;
    }
    /*
    private void Update()
    {
        if (selected && Input.GetMouseButtonDown(0))
        {
            RaycastHit2D  hit = new RaycastHit2D ();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
           hit = Physics2D.GetRayIntersection(ray);
            if (hit.collider != null)
            {
                GameObject state = hit.transform.gameObject;
                if (state != this.gameObject)
                {
                    selected = false;
                    sprite.color = oldColor;
                }
            }
            else
            {
                selected = false;
                sprite.color = oldColor;
            }
        }
    }*/
}