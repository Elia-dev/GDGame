using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MoveCardsUI : MonoBehaviour
{
    public Sprite imgSprite;
    public Vector2 startPosition;
    public Vector2 targetPosition;
    public float moveDuration = 2f; // Durata del movimento in secondi
    public void loadSprite(string spriteName) {
        imgSprite = Resources.Load<Sprite>(spriteName);
    }

    public abstract IEnumerator MoveCards();
}
