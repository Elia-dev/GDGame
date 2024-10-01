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
        [NonSerialized] protected TerritoryHandlerUI SelectedTerritory;
        protected static bool distributionPhase = true;

        void Start() {
            distributionPhase = true;
        }
    }
}
