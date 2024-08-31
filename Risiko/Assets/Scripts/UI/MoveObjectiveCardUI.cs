using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MoveObjectiveCardUI : MoveCardsUI {
        public float flipDuration = 0.5f; // Durata del flip
        [SerializeField] public GameObject cardFront; // Oggetto fronte della carta
        [SerializeField] public GameObject cardBack; // Oggetto retro della carta
        [SerializeField] private GameObject territoriesCardCanvas;
        private RectTransform _rectTransform;
        private bool _flipped = false;

        void Start() {
            _rectTransform = GetComponent<RectTransform>();

            // Imposta la posizione di destinazione al centro dello schermo
            targetPosition = new Vector2(0, 0);

            // Imposta la posizione iniziale appena sotto lo schermo
            startPosition = new Vector2(0, -Screen.height / 2 - _rectTransform.rect.height / 2 - 10);
            _rectTransform.anchoredPosition = startPosition;

            //Carica la sprite della carta missione
            loadSprite("Objectives/" + Player.Instance.ObjectiveCard.id);
            cardFront.GetComponent<Image>().sprite = imgSprite;
        
            // Inizia la coroutine per muovere l'immagine
            StartCoroutine(MoveCards());
        }

        public void Update() {
            if (Input.GetMouseButtonDown(0)) {
                if (_flipped) {
                    GameObject.Find("ObjectiveCardCanvas").SetActive(false);
                    territoriesCardCanvas.SetActive(true);
                    cardBack.SetActive(false);
                    GameManagerUI.SettingGame = false;
                }
            }
        
        }

        private IEnumerator MoveCards() {
            // Movimento della carta
            float elapsedTime = 0f;
            while (elapsedTime < moveDuration) {
                _rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _rectTransform.anchoredPosition = targetPosition;

            // Flip della carta
            elapsedTime = 0f;
            while (elapsedTime < flipDuration) {
                float rotationY = Mathf.Lerp(0, 180, elapsedTime / flipDuration);
                _rectTransform.localRotation = Quaternion.Euler(0, rotationY, 0);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _rectTransform.localRotation = Quaternion.Euler(0, 180, 0);

            // Cambia la visibilità delle immagini del fronte e del retro
            cardFront.SetActive(true);
            _rectTransform.anchoredPosition = startPosition;
            _rectTransform = cardFront.GetComponent<RectTransform>();

            // Flip completo della carta
            elapsedTime = 0f;
            while (elapsedTime < flipDuration) {
                float rotationY = Mathf.Lerp(180, 360, elapsedTime / flipDuration);
                _rectTransform.localRotation = Quaternion.Euler(0, rotationY, 0);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        
            _flipped = true;
            _rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
            cardBack.GetComponent<Image>().enabled = false;
        }
    }
}