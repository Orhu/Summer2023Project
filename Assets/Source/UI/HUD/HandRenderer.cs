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

        // Reference to the player game object
        private GameObject playerObject;

        [Tooltip("Max number of runes to generate")]
        [SerializeField] private int maxHandSize;

        [Tooltip("Time it takes to initiate fade out animation")]
        [SerializeField] private float totalFadeOutCooldown = 5f;

        [Header("Game World Settings")]
        [Tooltip("Whether the hand displays in the game world or in the UI")]
        [SerializeField] private bool handInGameWorld;

        [Tooltip("How offset the hand is from the player in the game world")]
        [SerializeField] private Vector2 handInGameWorldOffset = new Vector2(0.7f, 0.3f);

        // NOTE: I would suggest not changing these values unless good reason.
        [Header("Radial Settings")]

        [Tooltip("How far the RuneRenderers are from the base point in game world")]
        [SerializeField] private float gameWorldFDistance = 450f;

        [Tooltip("How far the RuneRenderers are from each other in game world")]
        [SerializeField] private float gameWorldMinAngle = 90f;

        [Tooltip("How far the RuneRenderers start in game world")]
        [SerializeField] private float gameWorldStartAngle = 135f;

        [Tooltip("How far the RuneRenderers are from the base point in UI")]
        [SerializeField] private float uiFDistance = 350f;

        [Tooltip("How far the RuneRenderers are from each other in UI")]
        [SerializeField] private float uiMinAngle = 135f;

        [Tooltip("How far the RuneRenderers start in UI")]
        [SerializeField] private float uiStartAngle = 120f;

        /// <summary>
        /// Instantiate RuneRenderers
        /// </summary>
        private void Start()
        {
            chordContainer = GameObject.FindGameObjectWithTag("HUD").GetComponentInChildren<ChordRenderer>();
            playerObject = Player.Get().gameObject;

            // Instantiate as many RuneRenderers as we have hand size
            for (int i = 0; i < maxHandSize; i++)
            {
                runeRenderers.Add(Instantiate(runeRendererTemplate, runeContainer.transform).GetComponent<RuneRenderer>());
            }

            // Decide if the hand is in the UI or the game world
            if (handInGameWorld)
            {
                MoveRendererToGameWorld();
            }
            else // Game is in UI
            {
                MoveRendererToUI();
            }
        }

        /// <summary>
        /// When we wish to swap the renderer to the Game World, we will need call do this
        /// </summary>
        private void MoveRendererToGameWorld()
        {
            Canvas parentCanvas = GetComponentInParent<Canvas>();

            // Scale the UI to be game world appropriate
            parentCanvas.gameObject.transform.localScale = new Vector3(0.005f, 0.005f, 1);

            // So the UI is in GameWorld render mode
            parentCanvas.renderMode = RenderMode.WorldSpace;

            // Set the UI to be positioned at the chordContainer on screen
            transform.position = new Vector3(playerObject.transform.position.x + handInGameWorldOffset.x,
                    playerObject.transform.position.y + handInGameWorldOffset.y, playerObject.transform.position.z);

            // Change how we're setting the rune's radially (it's different when in game world)
            RadialLayout childRadialLayout = GetComponentInChildren<RadialLayout>();

            childRadialLayout.gameObject.transform.localPosition = Vector3.zero;

            childRadialLayout.fDistance = gameWorldFDistance;

            childRadialLayout.MinAngle = gameWorldMinAngle;

            childRadialLayout.StartAngle = gameWorldStartAngle;
        }

        /// <summary>
        /// When we wish to swap the renderer to the UI, we will need to call this
        /// </summary>
        private void MoveRendererToUI()
        {
            Canvas parentCanvas = GetComponentInParent<Canvas>();

            // When the UI is in the game world, it is scaled way down to fit on screen.
            // When we swap back to UI, we need to scale it back to (1,1,1)
            parentCanvas.gameObject.transform.localScale = Vector3.one;

            // So the UI is not in GameWorld render mode anymore
            parentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // Set the UI to be positioned at the chordContainer on screen
            transform.position = chordContainer.transform.position;

            // Change how we're setting the rune's radially (it's different when not in game world)
            RadialLayout childRadialLayout = GetComponentInChildren<RadialLayout>();

            childRadialLayout.gameObject.transform.localPosition = Vector3.zero;

            childRadialLayout.fDistance = uiFDistance;

            childRadialLayout.MinAngle = uiMinAngle;

            childRadialLayout.StartAngle = uiStartAngle;
        }

        /// <summary>
        /// Allows a player to dynamically change whether the 
        /// HandRenderer is in the GameWorld or in the UI.
        /// </summary>
        /// <param name="inGameWorld">If true, the UI is in game world, else it is in the UI</param>
        public void SetHandRendererMode(bool inGameWorld)
        {
            if (inGameWorld)
            {
                handInGameWorld = true;
                MoveRendererToGameWorld();
            }
            else
            {
                handInGameWorld = false;
                MoveRendererToUI();
            }
        }

        /// <summary>
        /// Updates the renders to show the appropriate cards and their preview/cooldown state.
        /// </summary>
        void Update()
        {
            // Ensure the hand renderer stays following the player
            // if it is in the game world
            if (handInGameWorld)
            {
                transform.position = new Vector3(playerObject.transform.position.x + handInGameWorldOffset.x,
                    playerObject.transform.position.y + handInGameWorldOffset.y, playerObject.transform.position.z);
            }

            // loop through current deck hand size
            for (int i = 0; i < Deck.playerDeck.handSize; i++)
            {
                Card card = Deck.playerDeck.inHandCards[i];
                if (runeRenderers[i].card != card)
                {
                    if (runeRenderers[i].card)
                    {
                        runeRenderers[i].totalCooldownTime = runeRenderers[i].card.cooldownTime;
                    }
                    runeRenderers[i].card = card;
                }
                else // Duplicate card, can keep the new cooldown time
                {
                    runeRenderers[i].totalCooldownTime = card.cooldownTime;
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