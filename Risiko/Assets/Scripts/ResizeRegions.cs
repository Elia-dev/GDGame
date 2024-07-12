using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeRegions : MonoBehaviour
{
    public RectTransform region1; // Riferimento al RectTransform della regione SA_ter4
    public RectTransform region2; // Riferimento al RectTransform della regione AF_ter1

    void Start()
    {
        Resize();
    }

    void Resize()
    {
        // Ottieni le dimensioni del Canvas
        RectTransform canvasRect = GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;

        // Ridimensiona le regioni in base alle dimensioni del Canvas
        region1.sizeDelta = new Vector2(canvasWidth * 0.5f, canvasHeight * 0.5f);
        region2.sizeDelta = new Vector2(canvasWidth * 0.3f, canvasHeight * 0.3f);
    }
}