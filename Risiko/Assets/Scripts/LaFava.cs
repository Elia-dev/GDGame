using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaFava : MonoBehaviour
{
   private void Update()
   {
      if (Input.GetMouseButtonDown(0))
      {
         Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
         RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

         if (hit.collider != null)
         {
            Debug.Log("Cliccato: " + hit.collider.name);

            hit.collider.GetComponent<SpriteRenderer>().color = Color.red;
         }
      }
   }
}
