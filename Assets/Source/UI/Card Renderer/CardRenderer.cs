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

        [Tooltip("The half sword image if a card has half damage.")]
        [SerializeField] private Sprite halfSwordImage;

        [Tooltip("Reference to the card's compendium button")]
        [SerializeField] private Button compendiumButton;

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
                links.flavorTextBox.enabled = shouldEnable;
                if (shouldEnable)
                {
                    // Name of the card
                    links.nameTextBox.text = _card.displayName;

                    // Description of the card
                    links.descriptionTextBox.text = _card.description;

                    // Flavor text of the card
                    links.flavorTextBox.text = _card.flavorText;

                    // Rarity of the card
                    links.rarityImage.enabled = _card.isRare;

                    // Projectile type
                    if (_card.damageTypeSprite != null)
                    {
                        links.projectileTypeImage.sprite = _card.damageTypeSprite;
                    }
                    else
                    {
                        links.projectileTypeImage.enabled = false;
                    }

                    // Cooldown of the card
                    links.coolDownOverlayText.text = _card.cooldownTime.ToString();

                    // Chord effect of the card
                    links.chordEffectText.text = _card.chordEffectText;


                    // filling all full swords
                    for (int i = 0; i < Mathf.Floor(_card.damage); i++)
                    {
                        links.damageContainer.transform.GetChild(i).GetComponent<Image>().color = _card.chordColor; 
                    }

                    // If there is some remainder leftover, assign the last image to be a half sword
                    if (_card.damage % 1 != 0)
                    {
                        int lastImageIndex = Mathf.CeilToInt(_card.damage) - 1;
                        links.damageContainer.transform.GetChild(lastImageIndex).GetComponent<Image>().sprite = halfSwordImage;
                        links.damageContainer.transform.GetChild(lastImageIndex).GetComponent<Image>().color = _card.chordColor;
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
        /// Called when the card is toggled on and off
        /// </summary>
        public void OnToggle()
        {
            // See if the toggle is on or off
            // (this function is called before this value is set)
            bool isToggled = GetComponent<Toggle>().isOn;
            if (!isToggled) // about to turn the toggle on
            {
                // Make the card grow
                GetComponent<Animator>().Play("A_CardRenderer_Enlarge");
                // Turn on the compendium button
                compendiumButton.gameObject.SetActive(false);
            }
            else // about to turn the toggle off
            {
                // Shrink the card
                GetComponent<Animator>().Play("A_CardRenderer_Shrink");
                // turn the compendium button off
                compendiumButton.gameObject.SetActive(true);
            }
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

            [Tooltip("The text box used to display the flavor text of the card.")]
            public TMP_Text flavorTextBox;

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

            [Tooltip("The text displaying the card's chord effect.")]
            public TMP_Text chordEffectText;
        }
    }
}