using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerritoriesCardsUI : MoveCardsUI {
    public GameObject imagePrefab; // Prefab dell'immagine da muovere
    public Transform gridTransform; // Transform del Grid Layout Group

    public int numberOfImages = 4; // Numero di immagini da creare

    private bool animationDone = false;
    private List<Territory> territories;

    void Start() {
        territories = Player.Instance.Territories;
        /*string[] territori =
        {
            "SA_ter1", "SA_ter2", "SA_ter3", "SA_ter4", "SA_ter1", "SA_ter2", "SA_ter3", "SA_ter4",
            "SA_ter1", "SA_ter2", "SA_ter3", "SA_ter4", "SA_ter1", "SA_ter2"
        };
        Debug.Log(Screen.width);
        Debug.Log(Screen.height);
        Debug.Log(Screen.width / Screen.height);*/
        if ((double)Screen.width / (double)Screen.height < (double)1.6)
            transform.GetComponent<GridLayoutGroup>().cellSize =
                new Vector2((Screen.width -10 - (territories.Count / 3)*10) / (territories.Count/3),
                    (float)((Screen.width - 80) / (territories.Count / 3) * 1.33));
        else
            transform.GetComponent<GridLayoutGroup>().cellSize =
                new Vector2((Screen.width -10 - (territories.Count / 2)*10) / (territories.Count / 2),
                    (float)((Screen.width - 120) / (territories.Count / 2) * 1.33));
        
        GameObject[] images = new GameObject[territories.Count];
        for (int i = 0; i < territories.Count; i++) {
            images[i] = Instantiate(imagePrefab, transform);
            loadSprite("Territories/" + territories[i]);
            images[i].GetComponent<Image>().sprite = imgSprite;
            images[i].transform.SetParent(gridTransform);
            //Debug.Log(images[i].GetComponent<Transform>().position.x + ", " +
            //images[i].GetComponent<Transform>().position.y);
        }
        //for (int i = 0; i < territori.Length; i++) 
        //Debug.Log(images[i].GetComponent<Transform>().position.x + ", " +
        //images[i].GetComponent<Transform>().position.y);

        //StartCoroutine(MoveCards());
    }

    private void Update() {
        if (animationDone && Input.GetMouseButtonDown(0)) {
            GameObject.Find("TerritoryCardsCanvas").SetActive(false);
        }
    }

    public override IEnumerator MoveCards() {
        {
            targetPosition = gridTransform.GetComponent<RectTransform>().anchoredPosition;
            Debug.Log(targetPosition.x + ", " + targetPosition.y);
            RectTransform imagePrefabRect = imagePrefab.GetComponent<RectTransform>();
            targetPosition.y = -targetPosition.y - imagePrefabRect.rect.height / 2;
            targetPosition.x = -targetPosition.x + imagePrefabRect.rect.width / 2;
            //Debug.Log((Screen.height / 2 + imagePrefabRect.rect.height / 2 + 10));
            startPosition = new Vector2(Screen.width / 2, -(Screen.height + imagePrefabRect.rect.height / 2 + 10));
            //for (int i = 0; i < territories.Count; i++) {
            string[] territori =
            {
                "SA_ter1", "SA_ter2", "SA_ter3", "SA_ter4", "SA_ter1", "SA_ter2", "SA_ter3", "SA_ter4",
                "SA_ter1", "SA_ter2", "SA_ter3", "SA_ter4", "SA_ter1", "SA_ter2"
            };
            for (int i = 0; i < territori.Length; i++) {
                GameObject newImage = Instantiate(imagePrefab, transform);
                RectTransform rectTransform = newImage.GetComponent<RectTransform>();
                //caricamento immagine
                loadSprite("Territories/" + territori[i]);
                newImage.GetComponent<Image>().sprite = imgSprite;

                // Posiziona l'immagine appena sotto lo schermo
                rectTransform.anchoredPosition = startPosition;

                // Muove l'immagine verso il centro dello schermo
                yield return StartCoroutine(MoveImage(newImage, rectTransform));

                //Aggiornamento posizione prossimo territorio
                targetPosition.x += imagePrefabRect.rect.width +
                                    gridTransform.GetComponent<GridLayoutGroup>().spacing.x;
            }

            animationDone = true;
        }

        IEnumerator MoveImage(GameObject image, RectTransform rectTransform) {
            float elapsedTime = 0f;
            while (elapsedTime < moveDuration) {
                rectTransform.anchoredPosition =
                    Vector2.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Assicurati che l'immagine sia esattamente nella posizione target
            //rectTransform.anchoredPosition = targetPosition;

            // Dopo aver raggiunto il target, aggiungi l'immagine al Grid Layout Group
            //image.transform.SetParent(gridTransform);
        }
    }
}