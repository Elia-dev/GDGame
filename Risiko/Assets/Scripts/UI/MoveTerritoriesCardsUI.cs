using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class MoveTerritoriesCardsUI : MoveCardsUI {
        [SerializeField] public GameObject imagePrefab;
        [SerializeField] public Transform gridTransform;
        [SerializeField] private GameObject territoryCardsCanvas;
        [SerializeField] private GameObject clickHandler;
        private float _animationTime = 15f;

        private bool _animationDone = false;
        private List<Territory> _territories;

        private void Start() {
            _territories = Player.Instance.Territories;
            AdjustCellsSize();
            CardAnimation();
            imagePrefab.SetActive(false);
        }

        private void AdjustCellsSize() {
            //RectTransform cardContainer = gridTransform.GetComponent<RectTransform>();
            GridLayoutGroup gridLayoutGroup = gridTransform.GetComponent<GridLayoutGroup>();
            float aspectRatio = 4f / 3f;
            float containerWidth = Screen.width - gridLayoutGroup.padding.left - gridLayoutGroup.padding.right;
            float containerHeight = Screen.height - 60;
            int cardNumber = _territories.Count;
            int maxRows = Mathf.CeilToInt(Mathf.Sqrt(cardNumber));
            int maxColumns = Mathf.CeilToInt((float)cardNumber / maxRows);
            float availableWidth = (containerWidth - gridLayoutGroup.spacing.x * (maxColumns - 1)) / maxColumns;
            float availableHeight = (containerHeight - gridLayoutGroup.spacing.y * (maxRows - 1)) / maxRows;
            float cellHeight = Mathf.Min(availableHeight, availableWidth * aspectRatio);
            float cellWidth = cellHeight / aspectRatio;
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
            gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
            gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
        }

        private void CardAnimation() {
            for (int i = 0; i < _territories.Count; i++) {
                GameObject newCard = Instantiate(imagePrefab, gridTransform);
                LoadSprite("Territories/" + _territories[i].id);
                newCard.GetComponent<Image>().sprite = ImgSprite;
                newCard.transform.localScale = Vector3.zero;
                newCard.transform.DOScale(Vector3.one, 0.5f).SetDelay(i * 0.2f);
            }
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