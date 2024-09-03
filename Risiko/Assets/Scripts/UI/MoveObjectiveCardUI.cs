using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MoveObjectiveCardUI : MoveCardsUI {
        [SerializeField] public GameObject cardFront; // Oggetto fronte della carta
        [SerializeField] public GameObject cardBack; // Oggetto retro della carta
        [SerializeField] private GameObject territoriesCardCanvas;
        
        private Vector2 _startPosition;
        private Vector2 _targetPosition;
        private float _moveDuration = 2f; // Durata del movimento in secondi
        private float _flipDuration = 0.5f; // Durata del flip
        private RectTransform _rectTransform;
        private bool _flipped = false;

        void Start() {
            _rectTransform = GetComponent<RectTransform>();

            // Imposta la posizione di destinazione al centro dello schermo
            _targetPosition = new Vector2(0, 0);

            // Imposta la posizione iniziale appena sotto lo schermo
            _startPosition = new Vector2(0, -Screen.height / 2 - _rectTransform.rect.height / 2 - 10);
            _rectTransform.anchoredPosition = _startPosition;

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
            while (elapsedTime < _moveDuration) {
                _rectTransform.anchoredPosition = Vector2.Lerp(_startPosition, _targetPosition, elapsedTime / _moveDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _rectTransform.anchoredPosition = _targetPosition;

            // Flip della carta
            elapsedTime = 0f;
            while (elapsedTime < _flipDuration) {
                float rotationY = Mathf.Lerp(0, 180, elapsedTime / _flipDuration);
                _rectTransform.localRotation = Quaternion.Euler(0, rotationY, 0);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _rectTransform.localRotation = Quaternion.Euler(0, 180, 0);

            // Cambia la visibilità delle immagini del fronte e del retro
            cardFront.SetActive(true);
            _rectTransform.anchoredPosition = _startPosition;
            _rectTransform = cardFront.GetComponent<RectTransform>();

            // Flip completo della carta
            elapsedTime = 0f;
            while (elapsedTime < _flipDuration) {
                float rotationY = Mathf.Lerp(180, 360, elapsedTime / _flipDuration);
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