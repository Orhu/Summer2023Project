using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace Cardificer
{
    /// <summary>
    /// A component for rendering cards in UI space.
    /// </summary>
    public class CardRenderer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler // Handlers function as UI OnMouseEnter
    {
        [Tooltip("The card to render.")]
        [SerializeField] private Card _card;

        [Tooltip("The half sword image if a card has half damage.")]
        [SerializeField] private Sprite halfSwordImage;

        [Tooltip("Reference to the card's compendium button")]
        [SerializeField] private Button compendiumButton;

        // To remember the original scale of the card
        private Vector3 originalScale;

        [Tooltip("Scaling factor of the scaling animation")]
        public float scaleFactor = 1.5f;

        [Tooltip("Duration of the scaling animation")]
        public float scaleDuration = 0.25f;

        [Tooltip("Set by the CardRenderer's manager when the card is selected.")]
        public bool isSelected;

        // Simple flag for knowing when the mouse is hovering over
        // the CardRenderer
        private bool isHovered;

        public Card card
        {
            set
            {
                _card = value;
                bool shouldEnable = _card != null;

                links.nameTextBox.enabled = shouldEnable;
                links.descriptionTextBox.enabled = shouldEnable;
                links.backgroundSprite.enabled = shouldEnable;
                links.outlineSprite.enabled = shouldEnable;
                links.chordEffectText.enabled = shouldEnable;
                links.rarityImage.enabled = shouldEnable;
                links.damageContainer.SetActive(shouldEnable);
                links.cardSprite.enabled = shouldEnable;
                links.projectileTypeImage.enabled = shouldEnable;
                links.flavorTextBox.enabled = shouldEnable;
                links.coolDownOverlayText.enabled = shouldEnable;
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
        /// Assign member variable values.
        /// </summary>
        private void Start()
        {
            originalScale = transform.localScale;
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
            bool isToggled = GetComponent<Toggle>().isOn;

            if (isToggled) // We've toggled the card
            {
                // turn on the outline
                links.outlineSprite.gameObject.SetActive(true);
            }
            else // deactivated toggle
            {
                // Turn off the outline
                links.outlineSprite.gameObject.SetActive(false);
                // Shrink the card if it's being hovered
                if (isHovered)
                {
                    StartCoroutine(ScaleCardRenderer(false));
                    isHovered = false;
                }
            }
        }

        /// <summary>
        /// Called when the mouse hovers over the card.
        /// </summary>
        /// <param name="eventData">Not used</param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isHovered)
            {
                // Make the card grow
                StartCoroutine(ScaleCardRenderer(true));
                // Turn on the compendium button
                compendiumButton.gameObject.SetActive(true);
                // Set hovered to true
                isHovered = true;
            }
        }

        /// <summary>
        /// Called when the mouse stops hovering over the card
        /// </summary>
        /// <param name="eventData">Not used</param>
        public void OnPointerExit(PointerEventData eventData)
        {
            if (isHovered)
            {
                // Shrink the card
                StartCoroutine(ScaleCardRenderer(false));
                // Turn off the compendium button
                compendiumButton.gameObject.SetActive(false);
                // Set hovered to false
                isHovered = false;
            }
        }

        /// <summary>
        /// Reset the toggle after the menu was closed
        /// and opened back up again
        /// </summary>
        private void OnEnable()
        {
            if (!isSelected)
            {
                GetComponent<Toggle>().isOn = false;
            }
        }

        /// <summary>
        /// Make sure no cards are selected
        /// when the menu is closed
        /// </summary>
        private void OnDisable()
        {
            isSelected = false; 
        }

        /// <summary>
        /// Small function that dynamically animates
        /// the scaling of the CardRenderer up or down by two
        /// </summary>
        /// <param name="scaleUp">Whether we are scaling up or down</param>
        private IEnumerator ScaleCardRenderer(bool scaleUp)
        {
            float timePassed = 0f;
            Vector3 targetScale = originalScale * scaleFactor;
            // Scaling up
            if (scaleUp)
            {
                while (timePassed < scaleDuration)
                {
                    transform.localScale = Vector3.Lerp(originalScale, targetScale, timePassed / scaleDuration);
                    timePassed += Time.unscaledDeltaTime;
                    yield return null;
                }

                transform.localScale = targetScale;
            }
            // Scaling down
            else
            {
                while (timePassed < scaleDuration)
                {
                    transform.localScale = Vector3.Lerp(targetScale, originalScale, timePassed / scaleDuration);
                    timePassed += Time.unscaledDeltaTime;
                    yield return null;
                }

                transform.localScale = originalScale;
            }
        }

        public void SetScale(Vector3 theScale)
        {
            transform.localScale = theScale;
            originalScale = theScale;
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

            [Tooltip("The image used to show when a card is selected by outlining it.")]
            public Image outlineSprite;

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