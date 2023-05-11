using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace CardSystem
{
    [ExecuteInEditMode]
    public class CardRenderer : MonoBehaviour
    {
        private Card card;
        public Card Card
        {
            set
            {
                card = value;
                bool shouldEnable = card != null;

                links.nameTextBox.enabled = shouldEnable;
                links.descriptionTextBox.enabled = shouldEnable;
                links.backgroundSprite.enabled = shouldEnable;
                links.cardSprite.enabled = shouldEnable;
                if (shouldEnable)
                {
                    links.nameTextBox.text = card.displayName;
                    links.descriptionTextBox.text = card.GetDescription(!isFlipped);

                    if (isFlipped)
                    {
                        links.cardSprite.sprite = card.effectImage;
                        links.backgroundSprite.sprite = card.effectBackground;
                    }
                    else
                    {
                        links.cardSprite.sprite = card.actionImage;
                        links.backgroundSprite.sprite = card.actionBackground;
                    }
                }
            }
            get { return card; }
        }

        public bool Previewing
        {
            set{ links.previewOverlay.enabled = value; }
            get{ return links.previewOverlay.enabled; }
        }

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
            card = card;
        }

        [System.Serializable]
        private struct ComponentLinks
        {
            public TMP_Text nameTextBox;
            public TMP_Text descriptionTextBox;
            public Image backgroundSprite;
            public Image cardSprite;
            public Image previewOverlay;
        }
    }
}