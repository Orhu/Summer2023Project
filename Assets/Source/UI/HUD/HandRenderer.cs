using System.Collections.Generic;
using UnityEngine;


namespace Cardificer
{
    /// <summary>
    /// UI element for rendering the actor's current hand.
    /// </summary>
    public class HandRenderer : MonoBehaviour
    {
        [Header("Containers/Templates")]
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

        [Header("Game World Settings")]
        [Tooltip("Whether the hand displays in the game world or in the UI")]
        [SerializeField] private bool handInGameWorld;

        /// <summary>
        /// Instantiate RuneRenderers
        /// </summary>
        private void Start()
        {
            // Instantiate as many RuneRenderers as we have hand size
            for (int i = 0; i < Deck.playerDeck.handSize; i++)
            {
                runeRenderers.Add(Instantiate(runeRendererTemplate, runeContainer.transform).GetComponent<RuneRenderer>());
            }
        }

        /// <summary>
        /// Updates the renders to show the appropriate cards and their preview/cooldown state.
        /// </summary>
        void Update()
        {
            // Ensure the hand renderer stays following the player
            // if it is in the game world
            //if (handInGameWorld)
            //{
            //    transform.position = new Vector3(playerObject.transform.position.x + handInGameWorldOffset.x,
            //        playerObject.transform.position.y + handInGameWorldOffset.y, playerObject.transform.position.z);
            //}

            // loop through current deck hand size
            for (int i = 0; i < Deck.playerDeck.handSize; i++)
            {
                Card card = Deck.playerDeck.inHandCards[i];
                if (runeRenderers[i].card)
                {
                    runeRenderers[i].totalCooldownTime = runeRenderers[i].card.cooldownTime;
                }
                runeRenderers[i].card = card;

                // Set runeRenderers that are currently in the hand to not be greyed out
                runeRenderers[i].greyedOut = false;

                // If rune renderer at index i is not previewing, but it should be
                if (!runeRenderers[i].previewing && Deck.playerDeck.previewedCardIndices.Contains(i))
                {
                    // Set it to be previewing
                    runeRenderers[i].previewing = true;
                    // Play the enlarge animation
                    runeRenderers[i].gameObject.GetComponent<Animator>().Play("A_RuneRenderer_Enlarge");
                }
                // If rune renderer at index i is previewing and it shouldn't be
                else if (runeRenderers[i].previewing && !Deck.playerDeck.previewedCardIndices.Contains(i))
                {
                    // Play the base animation
                    runeRenderers[i].gameObject.GetComponent<Animator>().Play("A_RuneRenderer_Base");
                    // Set previewing to false
                    runeRenderers[i].previewing = false;
                }

                if (Deck.playerDeck.actingCardIndices.Contains(i))
                {
                    runeRenderers[i].currentCooldownTime = 0;
                    runeRenderers[i].actionTime = 1;
                }
                else if (Deck.playerDeck.cardIndicesToCooldowns.ContainsKey(i))
                {
                    runeRenderers[i].actionTime = 0;
                    runeRenderers[i].currentCooldownTime = Deck.playerDeck.cardIndicesToCooldowns[i];
                }
                else
                {
                    runeRenderers[i].currentCooldownTime = 0;
                    runeRenderers[i].actionTime = 0;
                }
            }
        }
    }
}