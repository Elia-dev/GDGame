using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class TerritoryHandlerUI : MonoBehaviour {
        private SpriteRenderer _sprite;
        public Color32 startColor = new Color32(0, 0, 0, 0);
        private Color32 _hoverColor = new Color32(205, 185, 52, 100);
        private static Color32 _selectionColor = new Color32(205, 185, 52, 100);
        public bool Selected { get; set; } = false;
        public static Color32 UserColor { get; set; }
        public Color32 StartColor {
            set => startColor = value;
        }

        void Awake() {
            _sprite = GetComponent<SpriteRenderer>();
            //sprite.color = startColor;
            _sprite.color = startColor;
        }

        void OnMouseEnter() {
            if (!Selected) {
                //startColor = sprite.color;
                _sprite.color = _hoverColor;
            }
        }

        void OnMouseExit() {
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
            //sprite.color = startColor;
            _sprite.color = startColor;
        }
    }
}