using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TerritoriesManagerUI : MonoBehaviour
    {
        public TerritoryHandlerUI selectedTerritory;
        [SerializeField] public List<GameObject> territories;
        [SerializeField] public Button endTurnButton;
        public static bool distributionPhase = true;

        public static TerritoriesManagerUI Instance { get; private set; }
    
        public void SelectState(TerritoryHandlerUI newTerritory) {
        
        }

        public void DeselectState() {
        
        }
    }
}
