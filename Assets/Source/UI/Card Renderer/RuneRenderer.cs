using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// A component for rendering runes in UI space.
/// </summary>
[ExecuteInEditMode]
public class RuneRenderer : MonoBehaviour
{
    [Tooltip("The card whose rune will be rendered")]
    [SerializeField] private Card _card;

    public Card card
    {
        set
        {
            _card = value;
            bool shouldEnable = _card != null;

            links.runeSprite.enabled = shouldEnable;
            if (shouldEnable)
            {
                links.runeSprite.sprite = _card.runeImage;
            }
        }
        get { return _card; }
    }
    [Tooltip("The links to the necessary components for rendering.")]
    [SerializeField]
    private ComponentLinks links;


    // Whether or not the preview overlay should be enabled.
    public bool previewing
    {
        set { links.previewOverlay.enabled = value; }
        get { return links.previewOverlay.enabled; }
    }

    // Whether the rune is usable / visible or its greyed out
    public bool greyedOut
    {
        set { links.greyedOutOverlay.enabled = value; }
        get { return links.greyedOutOverlay.enabled; }
    }

    // The cooldown time to display. If <= 0 no cooldown overlay will be rendered.
    public float cooldownTime
    {
        set
        {
            links.cooldownOverlay.enabled = value > 0;
            links.cooldownTimeTextBox.text = (Mathf.Round(value * 10f) / 10f).ToString();
            links.cooldownTimeTextBox.enabled = value > 0;
        }
    }

    // The cooldown time to display.If <= 0 no cooldown overlay will be rendered.
    public float actionTime
    {
        set
        {
            links.actionTimeOverlay.enabled = value > 0;
            links.actionTimeTextBox.text = (Mathf.Round(value * 10f) / 10f).ToString();
            links.actionTimeTextBox.enabled = value > 0;
        }
    }

    /// <summary>
    /// Struct for storing the needed component references.
    /// </summary>
    [System.Serializable]
    struct ComponentLinks
    {

        [Tooltip("The text boxed used to display the current remaining cooldown of the crune.")]
        public TMP_Text cooldownTimeTextBox;

        [Tooltip("The text boxed used to display the current remaining action time of the rune.")]
        public TMP_Text actionTimeTextBox;

        [Tooltip("The image used to render the rune specific sprite.")]
        public Image runeSprite;

        [Tooltip("The overlay to enable when previewing this rune.")]
        public Image previewOverlay;

        [Tooltip("The overlay to enable when this rune is on cooldown.")]
        public Image cooldownOverlay;

        [Tooltip("The overlay to enable when this rune is on acting.")]
        public Image actionTimeOverlay;

        [Tooltip("The overlay to enable when this rune is inactive")]
        public Image greyedOutOverlay;
    }
}
