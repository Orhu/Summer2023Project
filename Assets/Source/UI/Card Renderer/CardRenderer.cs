using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace CardSystem
{
    /// <summary>
    /// A component for rendering cards in UI space.
    /// </summary>
    [ExecuteInEditMode]
    public class CardRenderer : MonoBehaviour
    {
        // The card to render.
        [SerializeField]
        Card card;
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
                    links.descriptionTextBox.text = card.GetDescription(renderActionSide);

                    if (!renderActionSide)
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

        // Whether or not the preview overlay should be enabled.
        public bool Previewing
        {
            set{ links.previewOverlay.enabled = value; }
            get{ return links.previewOverlay.enabled; }
        }

        // The cooldown time to display. If <= 0 no cooldown overlay will be rendered.
        public float CooldownTime 
        { 
            set 
            {
                links.cooldownOverlay.enabled = value > 0;
                links.cooldownTimeTextBox.text = (Mathf.Round(value * 10f) / 10f).ToString();
                links.cooldownTimeTextBox.enabled = value > 0;
            }
        }
        public float ActionTime
        {
            set
            {
                links.actionTimeOverlay.enabled = value > 0;
                links.actionTimeTextBox.text = (Mathf.Round(value * 10f) / 10f).ToString();
                links.actionTimeTextBox.enabled = value > 0;
            }
        }

        // The links to the necessary components for rendering.
        [SerializeField]
        ComponentLinks links;
        // Whether to render the action side or the effect side of the card.
        [SerializeField]
        bool renderActionSide;
        bool RenderActionSide
        {
            set
            {
                renderActionSide = value;
                Card = card;
            }
            get { return renderActionSide; }
        }

        /// <summary>
        /// Used to flip rendered card from effect to action and back
        /// </summary>
        public void FlipRenderActionSide()
        {
            renderActionSide = !renderActionSide;
        }
        
        /// <summary>
        /// Refreshes the rendering on property change.
        /// </summary>
        public void Update()
        {
            RenderActionSide = renderActionSide;
        }

        /// <summary>
        /// Struct for storing the needed component references.
        /// </summary>
        [System.Serializable]
        struct ComponentLinks
        {
            public TMP_Text nameTextBox;
            public TMP_Text descriptionTextBox;
            public TMP_Text cooldownTimeTextBox;
            public TMP_Text actionTimeTextBox;
            public Image backgroundSprite;
            public Image cardSprite;
            public Image previewOverlay;
            public Image cooldownOverlay;
            public Image actionTimeOverlay;
        }
    }
}