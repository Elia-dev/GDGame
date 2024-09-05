using System;
using System.Collections;
using UnityEngine;

namespace UI
{
    public abstract class MoveCardsUI : MonoBehaviour
    {
        [NonSerialized] public Sprite imgSprite;
        public void LoadSprite(string spriteName) {
            imgSprite = Resources.Load<Sprite>(spriteName);
        }
    }
}
