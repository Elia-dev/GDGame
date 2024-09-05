using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class ArmySelectionHandlerUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private GameObject frame;
        [SerializeField] private Color32 armyColor;
        private bool _selected = false;

        public Color32 ArmyColor {
            get => armyColor;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_selected) {
                /*oldColor = sprite.color;
            sprite.color = hoverColor;*/
                frame.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_selected) {
                //sprite.color = oldColor;
                frame.SetActive(false);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Select();
        }
    
        public void Select() {
            _selected = true;
            //Debug.Log("Select");
            /// CARICARE DATI STATO
            //sprite.color = hoverColor;
            frame.SetActive(true);
        }

        public void Deselect() {
            _selected = false;
            frame.SetActive(false);
            //sprite.color = startColor;
        }
    }
}
