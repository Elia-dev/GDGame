using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MoveTerritoriesCardsUI : MoveCardsUI {
        [SerializeField] public GameObject imagePrefab; // Prefab dell'immagine da muovere
        [SerializeField] public Transform gridTransform; // Transform del Grid Layout Group
        [SerializeField] private GameObject territoryCardsCanvas;
        [SerializeField] private GameObject clickHandler;
        
        private bool animationDone = false;
        private List<Territory> territories;

        void Start() {
            territories = Player.Instance.Territories;
            AdjustCellsSize();
            CardAnimation();
            imagePrefab.SetActive(false);
            animationDone = true;
        }

        private void AdjustCellsSize()
        {
            RectTransform cardContainer = gridTransform.GetComponent<RectTransform>();
            GridLayoutGroup gridLayoutGroup = gridTransform.GetComponent<GridLayoutGroup>();
            float rapportoAspetto = 4f / 3f; // Rapporto di aspetto originale delle carte
            
            // Ottieni le dimensioni del contenitore
            float containerlength = cardContainer.rect.width - gridLayoutGroup.padding.left - gridLayoutGroup.padding.right;
            float containerHeight = cardContainer.rect.height - gridLayoutGroup.padding.top - gridLayoutGroup.padding.bottom;

            // Calcola il numero di carte
            int cardNumber = territories.Count;

            // Definisci il numero massimo di righe
            int maxRow = 3;

            // Calcola il numero di righe
            int row = Mathf.Min(maxRow, Mathf.CeilToInt((float)cardNumber / 3));

            // Calcola il numero di colonne
            int column = Mathf.CeilToInt((float)cardNumber / row);

            // Calcola la dimensione massima delle celle che mantenga il rapporto di aspetto
            float availableLength = (containerlength - gridLayoutGroup.spacing.x * (column - 1)) / column;
            float availableHeight = (containerHeight - gridLayoutGroup.spacing.y * (row - 1)) / row;

            // Calcola la dimensione delle celle mantenendo il rapporto di aspetto
            float cellLengthDimension = Mathf.Min(availableLength, availableLength * rapportoAspetto);
            float cellHeightDimension = Mathf.Min(availableHeight, availableHeight * rapportoAspetto);

            float cellDimension = Mathf.Min(cellLengthDimension, cellHeightDimension);

            // Imposta la dimensione delle celle nel GridLayoutGroup
            gridLayoutGroup.cellSize = new Vector2(cellDimension / rapportoAspetto, cellDimension);
        }

        private void CardAnimation() {
            for (int i = 0; i < territories.Count; i++) {
                GameObject nuovaCarta = Instantiate(imagePrefab, gridTransform);
                loadSprite("Territories/" + territories[i].id);
                nuovaCarta.GetComponent<Image>().sprite = imgSprite;
        
                // Nascondi inizialmente la carta (o scala piccola)
                nuovaCarta.transform.localScale = Vector3.zero;

                // Anima la carta in modo che appaia
                nuovaCarta.transform.DOScale(Vector3.one, 0.5f).SetDelay(i * 0.2f); // Anima con ritardo progressivo
            }
        }
        private void Update() {
            if (animationDone && Input.GetMouseButtonDown(0)) {
                territoryCardsCanvas.SetActive(false);
                clickHandler.GetComponent<TerritoriesManagerDistrPhaseUI>()
                    .ActivateTerritories();
            }
        }
    }
}