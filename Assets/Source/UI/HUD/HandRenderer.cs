using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// UI element for rendering the actor's current hand.
/// </summary>
public class HandRenderer : MonoBehaviour
{
    [Tooltip("The rune renderers to display the hand.")]
    public List<RuneRenderer> runeRenderers = new List<RuneRenderer>();

    [Tooltip("The rune renderer prefab to instantiate.")]
    [SerializeField] private RuneRenderer runeRendererTemplate;

    [Tooltip("The container holding the runes")]
    [SerializeField] private GameObject runeContainer;

    [Tooltip("The container / renderer for the chord root rune")]
    [SerializeField] private ChordRenderer chordContainer;

    

    

    [Tooltip("Max number of runes to generate")]
    [SerializeField] private int maxHandSize;

    /// <summary>
    /// Instantiate RuneRenderers
    /// </summary>
    private void Start()
    {
        chordContainer = GetComponentInChildren<ChordRenderer>();
        for (int i = 0; i < maxHandSize; i++)
        {
            runeRenderers.Add(Instantiate(runeRendererTemplate, runeContainer.transform).GetComponent<RuneRenderer>());
        }
    }

    /// <summary>
    /// Updates the renders to show the appropriate cards and their preview/cooldown state.
    /// </summary>
    void Update()
    {
        // loop through current deck hand size
        for (int i = 0; i < Deck.playerDeck.handSize; i++)
        {
            Card card = Deck.playerDeck.inHandCards[i];
            if (runeRenderers[i].card != card)
            {
                runeRenderers[i].card = card;
            }
            // Anything that's currently in the player's hand is not greyed out
            runeRenderers[i].greyedOut = false;
            if (!runeRenderers[i].previewing)
            {
                runeRenderers[i].previewing = Deck.playerDeck.previewedCardIndices.Contains(i);
                if (runeRenderers[i].previewing)
                {
                    runeRenderers[i].gameObject.GetComponent<Animator>().Play("A_RuneRenderer_Enlarge");
                }
            }

            // Check for previewing for chording
            if (Deck.playerDeck.previewedCardIndices.Count > 0)
            {
                // Obtain the first card of the chord
                if (Deck.playerDeck.previewedCardIndices.Count == 1 && Deck.playerDeck.previewedCardIndices[0] == i )
                {
                    chordContainer.DisplayChordLevelOne(runeRenderers[i].card);
                }
                // Obtain the second card of the chord
                else if (Deck.playerDeck.previewedCardIndices.Count == 2 && Deck.playerDeck.previewedCardIndices[1] == i)
                {
                    chordContainer.DisplayChordLevelTwo(runeRenderers[i].card);
                }
                // Obtain the third card of the chord
                else if (Deck.playerDeck.previewedCardIndices.Count == 3 && Deck.playerDeck.previewedCardIndices[2] == i)
                {
                    chordContainer.DisplayChordLevelThree(runeRenderers[i].card);
                }
                
            }
            // Reset chord
            else if(Deck.playerDeck.previewedCardIndices.Count <= 0)
            {
                chordContainer.ResetChord();
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
