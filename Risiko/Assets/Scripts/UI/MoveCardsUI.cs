using System;
using UnityEngine;

namespace UI
{
    public abstract class MoveCardsUI : MonoBehaviour
    {
        [NonSerialized] protected Sprite ImgSprite;

        protected void LoadSprite(string spriteName) {
            ImgSprite = Resources.Load<Sprite>(spriteName);
        }
    }
}
