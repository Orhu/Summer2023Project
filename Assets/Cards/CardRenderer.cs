using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CardSystem
{
    [ExecuteInEditMode]
    public class CardRenderer : MonoBehaviour
    {
        [SerializeField]
        public Card card;

        [SerializeField]
        private ComponentLinks links;
        [SerializeField]
        private bool isFlipped;

        public void Update()
        {
            SetFlipped(isFlipped);
        }

        public void SetFlipped(bool newIsFlipped)
        {
            isFlipped = newIsFlipped;

            //links.nameTextBox.text = card.displayName;
            //links.descriptionTextBox.text = card.GetDescription(isFlipped);

            //if (isFlipped)
            //{
            //    links.cardSprite.sprite = card.effectImage;
            //    links.backgroundSprite.sprite = card.effectBackground;
            //}
            //else
            //{
            //    links.cardSprite.sprite = card.actionImage;
            //    links.backgroundSprite.sprite = card.actionBackground;
            //}
        }

        [System.Serializable]
        private struct ComponentLinks
        {
            public TextMeshPro nameTextBox;
            public TextMeshPro descriptionTextBox;
            public SpriteRenderer backgroundSprite;
            public SpriteRenderer cardSprite;
        }
    }
}