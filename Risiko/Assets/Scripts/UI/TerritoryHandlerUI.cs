using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class TerritoryHandlerUI : MonoBehaviour {
        private SpriteRenderer _sprite;
        public Color32 startColor = new Color32(0, 0, 0, 0);
        private Color32 _hoverColor = new Color32(205, 185, 52, 100);
        private Color32 _selectionColor = new Color32(205, 185, 52, 100); //private static Color32 _selectionColor = new Color32(205, 185, 52, 100);
        private bool Selected { get; set; } = false;
        public static Color32 UserColor { get; set; }
        public Color32 StartColor {
            set => startColor = value;
        }

        private void Awake() {
            _sprite = GetComponent<SpriteRenderer>();
            _sprite.color = startColor;
        }

        private void OnMouseEnter() {
            if (!Selected) {
                _sprite.color = _hoverColor;
            }
        }

        private void OnMouseExit() {
            if (!Selected) {
                _sprite.color = startColor;
            }
        }

        public void Select() {
            Selected = true;
            _sprite.color = _selectionColor;
        }

        public void Deselect() {
            Selected = false;
            _sprite.color = startColor;
        }
    }
}
