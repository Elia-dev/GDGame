using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class MoveTerritoriesCardsUI : MoveCardsUI {
        [SerializeField] public GameObject imagePrefab; // Prefab dell'immagine da muovere
        [SerializeField] public Transform gridTransform; // Transform del Grid Layout Group
        [SerializeField] private GameObject territoryCardsCanvas;
        [SerializeField] private GameObject clickHandler;
        private float _animationTime = 15f;

        private bool _animationDone = false;
        private List<Territory> _territories;

        void Start() {
            if (clickHandler == null) {
                Debug.LogError("clickHandler is not assigned in the Inspector");
            }
            else {
                Debug.Log("clickHandler is assigned correctly");
            }

            _territories = Player.Instance.Territories;
            AdjustCellsSize();
            CardAnimation();
            imagePrefab.SetActive(false);
        }

        private void AdjustCellsSize() {
            //RectTransform cardContainer = gridTransform.GetComponent<RectTransform>();
            GridLayoutGroup gridLayoutGroup = gridTransform.GetComponent<GridLayoutGroup>();
            float aspectRatio = 4f / 3f; // Rapporto di aspetto originale delle carte

            // Ottieni le dimensioni del contenitore
            float containerWidth = Screen.width - gridLayoutGroup.padding.left - gridLayoutGroup.padding.right;
            float containerHeight = Screen.height - 60;

            // Calcola il numero di carte
            int cardNumber = _territories.Count;

            // Definisci il numero massimo di righe e colonne
            int maxRows = Mathf.CeilToInt(Mathf.Sqrt(cardNumber));
            int maxColumns = Mathf.CeilToInt((float)cardNumber / maxRows);

            // Calcola la dimensione massima delle celle che mantenga il rapporto di aspetto
            float availableWidth = (containerWidth - gridLayoutGroup.spacing.x * (maxColumns - 1)) / maxColumns;
            float availableHeight = (containerHeight - gridLayoutGroup.spacing.y * (maxRows - 1)) / maxRows;

            // Calcola la dimensione delle celle mantenendo il rapporto di aspetto 4:3
            float cellHeight = Mathf.Min(availableHeight, availableWidth * aspectRatio);
            float cellWidth = cellHeight / aspectRatio;

            // Aumenta il numero di righe e colonne per massimizzare la dimensione delle celle
            while ((cellHeight * maxRows < containerHeight && cellWidth * maxColumns < containerWidth) &&
                   (maxRows * maxColumns <= 14)) {
                if (maxColumns * cellWidth < containerWidth) {
                    maxColumns++;
                }
                else if (maxRows * cellHeight < containerHeight) {
                    maxRows++;
                }
                else {
                    break;
                }

                availableWidth = (containerWidth - gridLayoutGroup.spacing.x * (maxColumns - 1)) / maxColumns;
                availableHeight = (containerHeight - gridLayoutGroup.spacing.y * (maxRows - 1)) / maxRows;

                cellHeight = Mathf.Min(availableHeight, availableWidth * aspectRatio);
                cellWidth = cellHeight / aspectRatio;
            }

            // Imposta la dimensione delle celle nel GridLayoutGroup
            gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);

            // Imposta la dimensione delle celle nel GridLayoutGroup
            gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
        }

        private void CardAnimation() {
            for (int i = 0; i < _territories.Count; i++) {
                GameObject nuovaCarta = Instantiate(imagePrefab, gridTransform);
                LoadSprite("Territories/" + _territories[i].id);
                nuovaCarta.GetComponent<Image>().sprite = imgSprite;

                // Nascondi inizialmente la carta (o scala piccola)
                nuovaCarta.transform.localScale = Vector3.zero;

                // Anima la carta in modo che appaia
                nuovaCarta.transform.DOScale(Vector3.one, 0.5f).SetDelay(i * 0.2f); // Anima con ritardo progressivo
            }
            // Calcola la durata totale dell'animazione
            _animationTime = _territories.Count * 0.2f + 0.5f;
        }

        private void Update() {
            if (_animationTime > 0) {
                _animationTime -= Time.deltaTime;
            }
            else {
                _animationDone = true;
            }

            if (_animationDone && Input.GetMouseButtonDown(0)) {
                territoryCardsCanvas.SetActive(false);
                clickHandler.GetComponent<TerritoriesManagerDistrPhaseUI>()
                    .ActivateTerritories();
            }
        }
    }
}