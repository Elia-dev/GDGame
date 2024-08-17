using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(PolygonCollider2D))]
public class TerritoryHandlerUI : MonoBehaviour {
    private SpriteRenderer sprite;
    public Color32 startColor = new Color32(0, 0, 0, 0);
    private Color32 hoverColor = new Color32(205, 185, 52, 100);
    private static Color32 selectionColor = new Color32(205, 185, 52, 100);
    private static Color32 userColor;
    //private static Color32 distributionPhaseColor = new Color32(205, 185, 52, 100);
    private bool selected = false;

    public bool Selected {
        get => selected;
        set => selected = value;
    }

    public static Color32 UserColor {
        get => userColor;
        set => userColor = value;
    }
    
    public Color32 StartColor {
        set => startColor = value;
    }

    /*public static void ArmyDistributionPhase() {
        selectionColor = distributionPhaseColor;
    }*/

    public static void gamePhase() {
        selectionColor = userColor;
    }
    void Awake() {
        sprite = GetComponent<SpriteRenderer>();
        //sprite.color = startColor;
        sprite.color = startColor;
    }

    void OnMouseEnter() {
        if (!selected) {
            //startColor = sprite.color;
            sprite.color = hoverColor;
        }
    }

    void OnMouseExit() {
        if (!selected) {
            sprite.color = startColor;
        }
    }

    /*void OnMouseDown() {
        if (!selected) {
            Select();
        }
    }*/

    public void Select() {
        selected = true;
        /// CARICARE DATI STATO
        sprite.color = selectionColor;
    }

    public void Deselect() {
        selected = false;
        //sprite.color = startColor;
        sprite.color = startColor;
    }
}