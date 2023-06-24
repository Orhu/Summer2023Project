using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Cardificer
{
    /// <summary>
    /// A component for rendering cards in UI space.
    /// </summary>
    public class CardRenderer : MonoBehaviour
    {
        [Tooltip("The card to render.")]
        [SerializeField] private Card _card;
        public Card card
        {
            set
            {
                _card = value;
                bool shouldEnable = _card != null;

                links.nameTextBox.enabled = shouldEnable;
                links.descriptionTextBox.enabled = shouldEnable;
                links.backgroundSprite.enabled = shouldEnable;
                links.cardSprite.enabled = shouldEnable;
                links.projectileTypeImage.enabled = shouldEnable;
                if (shouldEnable)
                {
                    // Name of the card
                    links.nameTextBox.text = _card.displayName;

                    // TODO Description of the card
                    //links.descriptionTextBox.text = _card.GetDescription(renderActionSide);

                    // TODO Rarity of the card
                    //links.rarityImage.enabled = _card.rarity;

                    // TODO Projectile type
                    //links.projectileTypeImage = _card.projectileType;

                    // Cooldown of the card
                    links.coolDownOverlayText.text = _card.cooldownTime.ToString();

                    // TODO For managing damage container colors
                    for (int i = 0; i < 3; i++)
                    {
                        //links.damageContainer;
                    }

                    if (!renderActionSide)
                    {
                        links.cardSprite.sprite = _card.effectImage;
                    }
                    else
                    {
                        links.cardSprite.sprite = _card.actionImage;
                    }
                }
            }
            get { return _card; }
        }

        [Tooltip("The links to the necessary components for rendering.")]
        [SerializeField] private ComponentLinks links;

        [Tooltip("Whether to render the action side or the effect side of the card.")]
        [SerializeField] private bool _renderActionSide;
        bool renderActionSide
        {
            set
            {
                _renderActionSide = value;
                card = _card;
            }
            get { return _renderActionSide; }
        }

        /// <summary>
        /// Refreshes the rendering on property change.
        /// </summary>
        public void Update()
        {
            renderActionSide = _renderActionSide;
        }

        /// <summary>
        /// Used to flip rendered card from effect to action and back
        /// </summary>
        public void FlipRenderActionSide()
        {
            renderActionSide = !renderActionSide;
        }

        /// <summary>
        /// Struct for storing the needed component references.
        /// </summary>
        [System.Serializable]
        struct ComponentLinks
        {
            [Tooltip("The text boxed used to display the name of the card.")]
            public TMP_Text nameTextBox;

            [Tooltip("The text boxed used to display the description of the card.")]
            public TMP_Text descriptionTextBox;

            [Tooltip("The image used to render the background of the card.")]
            public Image backgroundSprite;

            [Tooltip("The image used to render the card specific sprite.")]
            public Image cardSprite;

            [Tooltip("The text displaying the card's cooldown.")]
            public TMP_Text coolDownOverlayText;

            [Tooltip("The gameobject that handles how many damage icons to spawn")]
            public GameObject damageContainer;

            [Tooltip("The image associated with the projectile type.")]
            public Image projectileTypeImage;

            [Tooltip("The image associated with the card's rarity.")]
            public Image rarityImage;
        }
    }
}