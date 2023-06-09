using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// UI element for rendering the actor's current hand.
/// </summary>
public class HandRenderer : MonoBehaviour
{
    // The rune renderers tp display the hand.
    public List<RuneRenderer> runeRenderers = new List<RuneRenderer>();

    // The rune renderer prefab to instantiate.
    public RuneRenderer runeRendererTemplate;

    // The image representing the root card's Rune
    private Image rootRuneImage;

    // Card that is the "root"
    private Card rootChordCard;

    // Default image to revert root image back to
    [SerializeField] private Sprite defaultRootImage;

    /// <summary>
    /// Instantiate RuneRenderers and get Image component
    /// </summary>
    private void Start()
    {
        rootRuneImage = GetComponent<Image>();
        for (int i = 0; i < Deck.playerDeck.handSize; i++)
        {
            runeRenderers.Add(Instantiate(runeRendererTemplate, transform).GetComponent<RuneRenderer>());
        }
    }

    /// <summary>
    /// Updates the renders to show the appropriate cards and their preview/cooldown state.
    /// </summary>
    void Update()
    {

        for (int i = 0; i < Deck.playerDeck.handSize; i++)
        {
            Card card = Deck.playerDeck.inHandCards[i];
            if (runeRenderers[i].card != card)
            {
                runeRenderers[i].card = card;
            }
            runeRenderers[i].previewing = Deck.playerDeck.previewedCardIndices.Contains(i);

            // Crude way to check for root of a chord. Need to review with Liam!
            if(Deck.playerDeck.previewedCardIndices.Count > 0 && Deck.playerDeck.previewedCardIndices[0] == i)
            {
                rootChordCard = runeRenderers[i].card;
                rootRuneImage.sprite = rootChordCard.runeImage;
            }
            else if(Deck.playerDeck.previewedCardIndices.Count <= 0)
            {
                rootChordCard = null;
                rootRuneImage.sprite = defaultRootImage;
            }

            if(Deck.playerDeck.cardIndicesToActionTimes.ContainsKey(i))
            {
                runeRenderers[i].cooldownTime = 0;
                runeRenderers[i].actionTime = Deck.playerDeck.cardIndicesToActionTimes[i];
            }
            else if (Deck.playerDeck.cardIndicesToCooldowns.ContainsKey(i))
            {
                runeRenderers[i].actionTime = 0;
                runeRenderers[i].cooldownTime = Deck.playerDeck.cardIndicesToCooldowns[i];
            }
            else
            {
                runeRenderers[i].cooldownTime = 0;
                runeRenderers[i].actionTime = 0;
            }
        }
    }
}
