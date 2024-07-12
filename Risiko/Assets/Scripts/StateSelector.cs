using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSelector : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log("Mouse Position: " + mousePos);

            Collider2D hitCollider = Physics2D.OverlapPoint(mousePos);
            if (hitCollider != null)
            {
                Debug.Log("Hai cliccato su: " + hitCollider.name);

                SpriteRenderer spriteRenderer = hitCollider.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.red;
                }
                else
                {
                    Debug.LogWarning("Nessun SpriteRenderer trovato sul collider: " + hitCollider.name);
                }
            }
            else
            {
                Debug.Log("Nessun collider colpito");
            }
        }
    }
}