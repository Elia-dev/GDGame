using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class TerritoriesManagerUI : MonoBehaviour
{
    [FormerlySerializedAs("selectedCountry")] public TerritoryHandlerUI selectedTerritory;
    

    public abstract void SelectState(TerritoryHandlerUI newTerritory);

    public abstract void DeselectState();
}
