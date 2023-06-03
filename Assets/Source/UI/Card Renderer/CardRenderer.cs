using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// A component for rendering cards in UI space.
/// </summary>
[ExecuteInEditMode]
public class CardRenderer : MonoBehaviour
{
    [Tooltip("The card to render.")]
    [SerializeField] private Card card;
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

    // The cooldown time to display.If <= 0 no cooldown overlay will be rendered.
    public float ActionTime
    {
        set
        {
            links.actionTimeOverlay.enabled = value > 0;
            links.actionTimeTextBox.text = (Mathf.Round(value * 10f) / 10f).ToString();
            links.actionTimeTextBox.enabled = value > 0;
        }
    }

    [Tooltip("The links to the necessary components for rendering.")]
    [SerializeField] private ComponentLinks links;

    [Tooltip("Whether to render the action side or the effect side of the card.")]
    [SerializeField] private bool renderActionSide;
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
        [Tooltip("The text boxed used to display the name of the card.")]
        public TMP_Text nameTextBox;

        [Tooltip("The text boxed used to display the description of the card.")]
        public TMP_Text descriptionTextBox;

        [Tooltip("The text boxed used to display the current remaining cooldown of the card.")]
        public TMP_Text cooldownTimeTextBox;

        [Tooltip("The text boxed used to display the current remaining action time of the card.")]
        public TMP_Text actionTimeTextBox;

        [Tooltip("The image used to render the background of the card.")]
        public Image backgroundSprite;

        [Tooltip("The image used to render the card specific sprite.")]
        public Image cardSprite;

        [Tooltip("The overlay to enable when previewing this card.")]
        public Image previewOverlay;

        [Tooltip("The overlay to enable when this card is on cooldown.")]
        public Image cooldownOverlay;

        [Tooltip("The overlay to enable when this card is on acting.")]
        public Image actionTimeOverlay;
    }
}