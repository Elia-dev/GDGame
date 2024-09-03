using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TerritoriesManagerUI : MonoBehaviour
    {
        [SerializeField] public List<GameObject> territories;
        [SerializeField] public Button endTurnButton;
        [NonSerialized] public TerritoryHandlerUI selectedTerritory;
        public static bool distributionPhase = true;

        public static TerritoriesManagerUI Instance { get; private set; }
    
        public void SelectState(TerritoryHandlerUI newTerritory) {
        
        }

        public void DeselectState() {
        
        }
    }
}
