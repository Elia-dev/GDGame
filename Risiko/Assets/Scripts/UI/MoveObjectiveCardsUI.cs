using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MoveObjectiveCardsUI : MoveCardsUI
{
    public float flipDuration = 0.5f; // Durata del flip
    public GameObject cardFront; // Oggetto fronte della carta
    public GameObject cardBack; // Oggetto retro della carta
    private RectTransform rectTransform;
    private bool flipped = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // Imposta la posizione di destinazione al centro dello schermo
        targetPosition = new Vector2(0, 0);

        // Imposta la posizione iniziale appena sotto lo schermo
        startPosition = new Vector2(0, -Screen.height / 2 - rectTransform.rect.height / 2 -10);
        Debug.Log(-Screen.height / 2 - rectTransform.rect.height / 2 -10);
        rectTransform.anchoredPosition = startPosition;
    
        //Carica la sprite della carta missione
        //loadSprite(Player.Instance.ObjectiveCard.CardId);
        loadSprite("Objectives/obj3");
        cardFront.GetComponent<Image>().sprite = imgSprite;
        
        // Inizia la coroutine per muovere l'immagine
        StartCoroutine(MoveCards());
    }

    public void Update() {
        if (flipped) {
            if (Input.GetMouseButtonDown(0)) {
                //CHIAMA ESTRAZIONE TERRITORI
            }
        }
    }

    public override IEnumerator MoveCards() {
        // Movimento della carta
        float elapsedTime = 0f;
        while (elapsedTime < moveDuration) {
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = targetPosition;

        // Flip della carta
        elapsedTime = 0f;
        while (elapsedTime < flipDuration) {
            float rotationY = Mathf.Lerp(0, 180, elapsedTime / flipDuration);
            rectTransform.localRotation = Quaternion.Euler(0, rotationY, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.localRotation = Quaternion.Euler(0, 180, 0);

        // Cambia la visibilità delle immagini del fronte e del retro
        cardFront.SetActive(true);
        rectTransform.anchoredPosition = startPosition;
        rectTransform = cardFront.GetComponent<RectTransform>();
        
        // Flip completo della carta
        elapsedTime = 0f;
        while (elapsedTime < flipDuration) {
            float rotationY = Mathf.Lerp(180, 360, elapsedTime / flipDuration);
            rectTransform.localRotation = Quaternion.Euler(0, rotationY, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
        cardBack.SetActive(false);
    }
}
