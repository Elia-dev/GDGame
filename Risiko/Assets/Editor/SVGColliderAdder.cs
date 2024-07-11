using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class SVGColliderAdder : AssetPostprocessor
{
    // Metodo chiamato quando un file SVG viene importato nella progetto
    private void OnPostprocessSVG(GameObject obj)
    {
        // Controlla se l'oggetto appena importato è un GameObject
        if (obj is GameObject)
        {
            GameObject svgObject = (GameObject)obj;

            // Recupera tutti i figli dell'oggetto SVG
            List<Transform> childTransforms = new List<Transform>();
            GetAllChildren(svgObject.transform, ref childTransforms);

            // Aggiungi un componente Collider2D a ogni figlio se non esiste già
            foreach (Transform child in childTransforms)
            {
                if (child.GetComponent<Collider2D>() == null)
                {
                    child.gameObject.AddComponent<PolygonCollider2D>(); // Puoi scegliere il tipo di collider adatto al tuo SVG
                }
            }
        }
    }

    // Funzione ricorsiva per ottenere tutti i figli di un oggetto
    private void GetAllChildren(Transform parent, ref List<Transform> children)
    {
        foreach (Transform child in parent)
        {
            children.Add(child);
            GetAllChildren(child, ref children);
        }
    }
}