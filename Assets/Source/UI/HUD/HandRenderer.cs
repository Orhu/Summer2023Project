using System.Collections.Generic;
using UnityEngine;


namespace Cardificer
{
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

        // Reference to the player game object
        private GameObject playerObject;

        [Tooltip("Max number of runes to generate")]
        [SerializeField] private int maxHandSize;

        [Tooltip("Whether the hand displays in the game world or in the UI")]
        [SerializeField] private bool handInGameWorld;

        // Boolean telling whether the runeRenderers are visible
        private bool runeRenderersVisible;

        // Time it takes to initiate cooldown
        private float fadeOutCooldown = 5f;

        /// <summary>
        /// Instantiate RuneRenderers
        /// </summary>
        private void Start()
        {
            chordContainer = GameObject.FindGameObjectWithTag("HUD").GetComponentInChildren<ChordRenderer>();
            playerObject = Player.Get().gameObject;
            runeRenderersVisible = true;
            for (int i = 0; i < maxHandSize; i++)
            {
                runeRenderers.Add(Instantiate(runeRendererTemplate, runeContainer.transform).GetComponent<RuneRenderer>());
            }
            if (handInGameWorld)
            {
                transform.position = new Vector3(playerObject.transform.position.x + 0.7f, playerObject.transform.position.y + 0.7f, playerObject.transform.position.z);
            }
            else // Game is in UI, TODO: a lot needs to change here.
            {
                MoveRendererToUI();
            }
        }
        /// <summary>
        /// When we wish to swap the renderer to the UI, we will need to do this
        /// </summary>
        private void MoveRendererToUI()
        {
            Canvas parentCanvas = GetComponentInParent<Canvas>();

            parentCanvas.gameObject.transform.localScale = Vector3.one;

            parentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            transform.position = chordContainer.transform.position;

            RadialLayout childRadialLayout = GetComponentInChildren<RadialLayout>();

            childRadialLayout.gameObject.transform.localPosition = Vector3.zero;

            childRadialLayout.fDistance = 350f;

            childRadialLayout.MinAngle = 135f;

            childRadialLayout.StartAngle = 120f;
        }

        /// <summary>
        /// Updates the renders to show the appropriate cards and their preview/cooldown state.
        /// </summary>
        void Update()
        {
            if (handInGameWorld)
            {
                transform.position = new Vector3(playerObject.transform.position.x + 0.7f, playerObject.transform.position.y + 0.3f, playerObject.transform.position.z);
            }
            // loop through current deck hand size
            for (int i = 0; i < Deck.playerDeck.handSize; i++)
            {
                Card card = Deck.playerDeck.inHandCards[i];
                if (runeRenderers[i].card != card)
                {
                    runeRenderers[i].card = card;
                }
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

                // Check for previewing for chording
                if (Deck.playerDeck.previewedCardIndices.Count > 0)
                {
                    // Obtain the first card of the chord
                    if (Deck.playerDeck.previewedCardIndices.Count == 1 && Deck.playerDeck.previewedCardIndices[0] == i)
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
                else if (Deck.playerDeck.previewedCardIndices.Count <= 0)
                {
                    chordContainer.ResetChord();
                }

                if (Deck.playerDeck.actingCardIndices.Contains(i))
                {
                    runeRenderers[i].cooldownTime = 0;
                    runeRenderers[i].actionTime = 1;
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


            if (Deck.playerDeck.previewedCardIndices.Count > 0 && !runeRenderersVisible) // One of the buttons has been pressed
            {
                runeRenderersVisible = true;
                fadeOutCooldown = 5f;
                for (int i = 0; i < Deck.playerDeck.handSize; i++)
                {
                    runeRenderers[i].GetComponent<Animator>().Play("A_RuneRenderer_FadeIn");
                }
            }
            else if (Deck.playerDeck.previewedCardIndices.Count <= 0 && runeRenderersVisible)
            {
                fadeOutCooldown -= Time.deltaTime;
                if (fadeOutCooldown <= 0)
                {
                    runeRenderersVisible = false;
                    for (int i = 0; i < Deck.playerDeck.handSize; i++)
                    {
                        runeRenderers[i].GetComponent<Animator>().Play("A_RuneRenderer_FadeOut");
                    }
                }
            }
        }
    }
}