using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public abstract class TerritoriesManagerUI : MonoBehaviour
    {
        [SerializeField] public List<GameObject> territories;
        [SerializeField] public Button endTurnButton;
        [NonSerialized] protected TerritoryHandlerUI selectedTerritory;
        protected static bool distributionPhase = true;

        public static TerritoriesManagerUI Instance { get; private set; }

        private void Start() {
            distributionPhase = true;
        }

        public void SelectState(TerritoryHandlerUI newTerritory) {
        
        }

        public void DeselectState() {
        
        }
    }
}
